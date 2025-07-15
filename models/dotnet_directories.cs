using Vogen;

namespace climate;

[ValueObject<string>()]
[Instance("Unspecified", "")]
[Instance("dotnet_tools", ".dotnet/tools")]
[Instance("tool_folder_name", $".{nameof(climate)}")]
public partial class dotnet_directories
{
}