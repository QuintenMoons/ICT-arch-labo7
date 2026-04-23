using System.Reflection;
using PluginContracts;

public class PluginLoader
{
    public static List<IPlugin> LoadPlugins(string folder)
    {
        List<IPlugin> plugins = new List<IPlugin>();
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        foreach (string dll in Directory.GetFiles(folder, "*.dll"))
        {
            try {
                Assembly assembly = Assembly.LoadFrom(dll);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (Type type in pluginTypes)
                {
                    if (Activator.CreateInstance(type) is IPlugin plugin)
                    {
                        plugins.Add(plugin);
                    }
                }
            } catch { /* Skip bestanden die geen .NET assemblies zijn */ }
        }
        return plugins;
    }
}