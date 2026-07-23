using System.Reflection;
using CodeMechanic.Bash;
using CodeMechanic.FileSystem;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Serilog.Core;
using Sharprompt;

namespace climate;

public class CliScaffolder : CliQueuedService
{
    private readonly Logger logger;
    private Assembly my_ass = Assembly.GetExecutingAssembly();
    private string cwd = Directory.GetCurrentDirectory();

    private readonly string app_name = string.Empty; // aka. the namespace
    private readonly string tool_name = string.Empty;
    private readonly bool is_daemon;

    public CliScaffolder(ArgsMap arguments, Logger logger) : base(arguments)
    {
        this.logger = logger;

        (_, app_name) = arguments.WithFlags("-n");
        this.is_daemon = arguments.HasCommand("daemon") || arguments.HasFlag("--create-daemon");
        this.tool_name = app_name.NotEmpty()
            ? app_name.Trim()
            : Prompt.Input<string>(
                "What should the tool name be (this will be used to run the tool from terminal)?");

        logger.Information(
            $"Creating basic boilerplate for tool {tool_name}");

        steps.Add(GenerateCLITool);
        if (is_daemon) steps.Add(InstallDaemonSupport);
        steps.Add(InstallBaseDependencies);
    }

    private async Task InstallDaemonSupport()
    {
        var basic_deps = new[]
        {
            "Coravel",
            "Microsoft.Extensions.Hosting"
        };

        logger.Information(nameof(InstallBaseDependencies));

        bool use_no_retore = false;

        string restore = use_no_retore ? "--no-restore" : "";

        foreach (var package in basic_deps)
        {
            // Removed '--no-build' as it is not a valid flag for 'dotnet add package'
            // Kept '--no-restore' to prevent immediate restore/download for every single package
            await $"dotnet add package {restore} {package} --project {app_name}"
                .Bash(verbose: true);
        }


        string[] worker_Files = new[]
        {
            "BgWorker.cs",
            "Listener.cs",
            "Queue.cs",
            "ToolSettings.cs",
            "FSAccessExtensions.cs" // temporary.  todo: test out cm.filesystem's copy and then remove this.
        };

        var replacements = new Dictionary<string, string>()
        {
            [@"namespace\s+[\w\.]+"] = $"namespace {tool_name}",
            [@"\$namespace\$"] = tool_name,
        };

        foreach (var filename in worker_Files)
        {
            string source_path = $"workers.{filename}";
            string code = await my_ass.ReadFile(source_path);

            string updated_code = code.AsArray().ReplaceAll(replacements).FlattenText();
            logger.Information($"{nameof(updated_code)} :>> {updated_code}");

            var outfile = new SaveFile(updated_code).To(cwd, tool_name, "workers").As(filename);
            logger.Information($"'{outfile.FullName}' created");
        }
    }

    private async Task InstallBaseDependencies()
    {
        var basic_deps = new[]
        {
            "CodeMechanic.Async",
            "CodeMechanic.Logging",
            "CodeMechanic.Bash", "CodeMechanic.Diagnostics", "CodeMechanic.Embeds",
            "CodeMechanic.FileSystem", "CodeMechanic.Reflection", "CodeMechanic.RegularExpressions",
            "CodeMechanic.Shargs", "CodeMechanic.Types", "Microsoft.Extensions.DependencyInjection", "Serilog",
            "Serilog.Sinks.Console", "Serilog.Sinks.File", "Sharprompt", "Spectre.Console", "Spectre.Console.Cli",
            "JsonFlatFileDataStore",
        };

        logger.Information(nameof(InstallBaseDependencies));

        bool use_no_retore = false;

        string restore = use_no_retore ? "--no-restore" : "";

        foreach (var package in basic_deps)
        {
            // Removed '--no-build' as it is not a valid flag for 'dotnet add package'
            // Kept '--no-restore' to prevent immediate restore/download for every single package
            await $"dotnet add package {restore} {package} --project {app_name}"
                .Bash(verbose: true);
        }

        // todo: change the C# working dir (not cd, that doesnt' work).
        // forces the latest versions to download after we initially skip them.
        // await $"dotnet restore --force --project {app_name}".Bash(verbose: true);

        // This single command performs the restore (downloading all packages) and the build (compiling)
        if (use_no_retore)
            await $"dotnet build {restore} --project {app_name}"
                .Bash(verbose: true);
    }

    private async Task GenerateCLITool()
    {
        try
        {
            await GenerateCsprojFile(tool_name);
            await GenerateAppFile(tool_name);
            await GenerateProgramFile(tool_name);
            await GenerateInstallScripts(tool_name);


            string nuget_text = await my_ass.ReadFile("nuget_config.template");
            new SaveFile(nuget_text).To(cwd, tool_name).As("nuget.config");

            logger.Information($"basic boilerplate created!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            logger.Error(e.ToString());
            throw;
        }
    }

    private async Task GenerateInstallScripts(string tool_name)
    {
        var replacements = new Dictionary<string, string>()
        {
            [@"\$namespace\$"] = tool_name,
        };

        var install_text =
            await GenerateFileFromTemplate(tool_name, "install.template",
                replacements);

        new SaveFile(install_text)
            .To(cwd, tool_name)
            .As($"install.sh");

        var uninstall_text =
            await GenerateFileFromTemplate(tool_name, "uninstall.template",
                replacements);

        new SaveFile(uninstall_text)
            .To(cwd, tool_name)
            .As($"uninstall.sh");
    }

    private async Task GenerateAppFile(string tool_name)
    {
        var replacements = new Dictionary<string, string>()
        {
            [@"\$namespace\$"] = tool_name,
        };

        string application_template = "Application.template";

        var text =
            await GenerateFileFromTemplate(tool_name, application_template,
                replacements);

        new SaveFile(text)
            .To(cwd, tool_name)
            .As($"Application.cs");
    }

    private async Task GenerateProgramFile(string tool_name)
    {
        string[] program_templates = new[] { "Program.template", "Daemon.template" };

        string template_name = is_daemon
            ? "Daemon.template"
            : Prompt.Select($"Which Program.cs template should be used?", program_templates);

        string program_template = await my_ass.ReadFile(template_name);
        logger.Information($"{nameof(template_name)} :>> {template_name}");

        var replacements = new Dictionary<string, string>()
        {
            [@"\$namespace\$"] = tool_name,
        };

        string text = program_template
            .Split('\n')
            .ReplaceAll(replacements)
            .Rollup();

        new SaveFile(text)
            .To(cwd, tool_name)
            .As($"Program.cs");
    }

    private async Task GenerateCsprojFile(
        string tool_name
    )
    {
        var replacements = new Dictionary<string, string>()
        {
            [@"\$command\$"] = tool_name,
            [@"\$frameworks\$"] = "net8.0", // todo: vogenize or smartenum this
        };

        var text =
            await GenerateFileFromTemplate(tool_name, "console.csproj.template",
                replacements);

        new SaveFile(text)
            .To(cwd, tool_name)
            .As($"{tool_name}.csproj");
    }

    private async Task<string> GenerateFileFromTemplate(string tool_name,
        string template_name, Dictionary<string, string> replacements)
    {
        string template =
            await my_ass.ReadFile(template_name);

        string text = template
            .Split('\n')
            .ReplaceAll(replacements)
            .Rollup();

        return text;
    }
}