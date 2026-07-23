using CodeMechanic.FileSystem;
using CodeMechanic.Types;

namespace climate;

/// <summary>
/// Settings defined in the dotfolder of a given tool.
/// </summary>
public record ToolSettings
{
    private readonly string tools_folder = "~/.dotnet/tools".AsUnixPath();
    public string name { get; init; }

    public string settings_filename { get; init; } // "settings.json"

    public string dotfolder_name => $".{name}";

    public string dotfolder =>
        name.IsEmpty() ? throw new ArgumentNullException(nameof(name)) : Path.Combine(tools_folder, $".{name}");

    public string tool_settings_path => name.IsEmpty()
        ? throw new ArgumentNullException(nameof(dotfolder))
        : Path.Combine(dotfolder, settings_filename);

    // Primary constructor with optional parameters (your favorite!)
    public ToolSettings(string name, string settingsFilename = "settings.json")
    {
        this.name = name;
        settings_filename = settingsFilename;
    }

    // Parameterless constructor for maximum convenience
    public ToolSettings()
        : this("fubar", settingsFilename: "settings.json")
    {
    }

    public static ToolSettings Create(
        string name, string settingsFilename = "settings.json")
    {
        return new ToolSettings(
            name, settingsFilename
        );
    }
}