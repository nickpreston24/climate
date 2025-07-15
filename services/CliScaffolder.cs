using System.Reflection;
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
    private string name = string.Empty;

    public CliScaffolder(ArgsMap arguments, Logger logger) : base(arguments)
    {
        this.logger = logger;
        steps.Add(GenerateCLITool);
        steps.Add(InstallBaseDependencies);
    }

    private async Task InstallBaseDependencies()
    {
        // Console.WriteLine(nameof(InstallBaseDependencies));
        //throw new NotImplementedException();
        return;
    }

    private async Task GenerateCLITool()
    {
        try
        {
            (_, name) = arguments.WithFlags("-n");

            string tool_name = name.NotEmpty()
                ? name
                : Prompt.Input<string>(
                    "What should the tool name (this will be used to run the tool from terminal)?");

            logger.Information(
                $"Creating basic boilerplate for tool {tool_name}");

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

        new SaveFile(install_text)
            .To(cwd, tool_name)
            .As($"uninstall.sh");
    }

    private async Task GenerateAppFile(string tool_name)
    {
        var replacements = new Dictionary<string, string>()
        {
            [@"\$namespace\$"] = tool_name,
        };

        var text =
            await GenerateFileFromTemplate(tool_name, "Application.template",
                replacements);

        new SaveFile(text)
            .To(cwd, tool_name)
            .As($"Application.cs");
    }

    private async Task GenerateProgramFile(string tool_name)
    {
        string program_template = await my_ass.ReadFile("Program.template");
        logger.Information(program_template);

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