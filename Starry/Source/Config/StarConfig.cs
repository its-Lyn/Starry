using System.Text.Json;

namespace Starry.Source.Config;

public static class StarConfig
{
    // Here, we use a property
    // That is because, with this method, we only need to compute the Dir once.
    private static string ConfigDir { get; } = FindConfigDir();
    private static string FindConfigDir()
    {
        string? xdgConfig = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (xdgConfig is not null)
        {
            string fullPath = Path.Combine(xdgConfig, "Starry");
            Directory.CreateDirectory(fullPath);
        
            return fullPath;
        }

        string configPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".config", "Starry");
        Directory.CreateDirectory(configPath);

        return configPath;
    }

    public static void Update(ConfigModel config)
    {
        string configString = JsonSerializer.Serialize<ConfigModel>(config);
        File.WriteAllText(Path.Combine(ConfigDir, "config.json"), configString);
    }

    public static void EnsureExists()
    {
        string configPath = Path.Combine(ConfigDir, "config.json");
        if (!File.Exists(configPath))
        {
            ConfigModel defaultConfig = new ConfigModel
            {
                Paths = new List<string>(),
                IgnorePaths = new List<string>(),
                DefaultOut = null,
                ZipDirs = false,
                ZipParent = true,
            };

            string configString = JsonSerializer.Serialize<ConfigModel>(defaultConfig);
            File.WriteAllText(configPath, configString);
        }
    }

    public static ConfigModel Fetch()
        => JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText(Path.Combine(ConfigDir, "config.json")))
               ?? throw new NullReferenceException("Config file could not be found, what did you do?");
}
