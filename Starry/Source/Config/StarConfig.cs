using System.Text.Json;
using Starry.Source.Config.Models;

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

    private static string ConfigFile { get; } = Path.Combine(ConfigDir, "config.json");
    private static string HistoryFile { get; } = Path.Combine(ConfigDir, "history.json");

    public static void Update(ConfigModel config)
        => File.WriteAllText(ConfigFile, JsonSerializer.Serialize(config));

    public static void Update(HistoryModel history)
        => File.WriteAllText(HistoryFile, JsonSerializer.Serialize(history));

    public static void EnsureExists()
    {
        // Checking for the main configuration file.
        if (!File.Exists(ConfigFile))
        {
            ConfigModel defaultConfig = new ConfigModel
            {
                Paths = new List<string>(),
                IgnorePaths = new List<string>(),
                DefaultOut = null,
                ZipDirs = false,
                ZipParent = true,
            };
        
            File.WriteAllText(ConfigFile, JsonSerializer.Serialize(defaultConfig));
        }

        // Checking for the history file.
        if (!File.Exists(HistoryFile))
        {
            HistoryModel history = new HistoryModel
            {
                History = new List<Item>()
            };

            File.WriteAllText(HistoryFile, JsonSerializer.Serialize(history));
        }
    }

    public static ConfigModel Fetch()
        => JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText(ConfigFile))
               ?? throw new NullReferenceException("Config file could not be found, what did you do?");

    public static HistoryModel History()
        => JsonSerializer.Deserialize<HistoryModel>(File.ReadAllText(HistoryFile))
             ?? throw new NullReferenceException("History file could not be found, what did you do?");
}
