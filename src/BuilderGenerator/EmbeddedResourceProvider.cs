using System.IO;
using System.Linq;
using System.Reflection;

namespace BuilderGenerator;

internal static class EmbeddedResourceProvider
{
    internal static string GetResourceByName(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();

        string resourcePath = name;

        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
        if (!name.StartsWith(nameof(BuilderGenerator)))
        {
            resourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));
        }

        using var stream = assembly.GetManifestResourceStream(resourcePath)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
