using System.Reflection;
using CodeMechanic.Types;

namespace climate;

internal static class EmbeddedResourceExtensions
{
    public static async Task<string> ReadFile(
        this Assembly ass,
        string file_hint,
        bool debug = false)
    {
        string resourcePath = ass
            .GetManifestResourceNames()
            // .Dump("resources")
            .FirstOrDefault(resource =>
                resource.Contains(file_hint));

        if (debug)
            Console.WriteLine("resource: " + resourcePath);

        if (resourcePath.IsEmpty())
        {
            if (debug)
                Console.WriteLine(
                    $"Resource '{resourcePath}' could not be found!");

            return string.Empty;
        }

        using (Stream stream = ass.GetManifestResourceStream(resourcePath))
        using (StreamReader reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}