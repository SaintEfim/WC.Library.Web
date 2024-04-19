using System.Diagnostics;
using System.Reflection;

namespace WC.Library.Web.Helpers;

public static class AssemblyHelpers
{
    public static Assembly[] GetApplicationAssemblies()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var referencedAssemblies = entryAssembly?.GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Union(new[] { entryAssembly });

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

        var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase))
            .ToList();
        toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

        Debug.Assert(referencedAssemblies != null, nameof(referencedAssemblies) + " != null");
        return loadedAssemblies.Union(referencedAssemblies).ToArray();
    }
}