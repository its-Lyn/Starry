using CommandLine;
using Starry.Source.Client.Colour;
using Starry.Source.Config;
using Starry.Source.Config.Models;

namespace Starry.Source.Client;

public class StarParser
{
    [Verb("backup", HelpText = "Backup the directories from your configuration file.")]
    private class BackupOpts
    {
        [Option('o', "output", Required = false, HelpText = "The path to output to, not using the default")]
        public string? Output { get; set; }
    }

    [Verb("config", HelpText = "Create and edit the configuration file.")]
    private class ConfigOpts
    {
        [Option('a', "add-path", Required = false, HelpText = "Add a path to a folder or a file to backup.")]
        public string? Path { get; set; }

        [Option('i', "ignore-path", Required = false, HelpText = "Ignore a specific path from being backed up.")]
        public string? PathIgnore { get; set; }

        [Option('r', "remove-path", Required = false, HelpText = "Remove a backup path from your config. Type all to remove all of the paths.")]
        public string? PathRemove { get; set; }

        [Option('e', "remove-ignore", Required = false, HelpText = "Remove an ignore path from your config. Type all to remove all of the paths.")]
        public string? PathIgnoreRemove { get; set; }

        [Option('l', "list-config", Required = false, HelpText = "Show all the paths in your config.")]
        public bool ShowConfig { get; set; }


        [Option('o', "default-output", Required = false, HelpText = "Set the default path Starry will send backups to. Default is <working direcory>/Backups. Type reset to set it back to null.")]
        public string? OutPath { get; set; }

        [Option('d', "zip-directories", Required = false, HelpText = "Wether or not to zip up the copied directories.")]
        public bool? ZipDirs { get; set; }

        [Option('p', "zip-parent", Required = false, HelpText = "Wether or not to zip up the parent directory. For example <dir>/Backups will be <dir>/Backups.zip")]
        public bool? ZipParent { get; set; }
    }

    [Verb("history", HelpText = "Manage your history.")]
    private class HistoryOpts 
    {
        [Option('c', "clear", Required = false, HelpText = "Clear out your history.")]
        public bool HistoryClear { get; set; }

        [Option('l', "list", Required = false, HelpText = "List out your history.")]
        public bool HistoryList { get; set; }
    }

    public void Parse(string[] args)
    {
        ConfigModel conf = StarConfig.Fetch();
        HistoryModel hist = StarConfig.History();

        Parser.Default.ParseArguments<BackupOpts, ConfigOpts, HistoryOpts>(args)
            .WithParsed<BackupOpts>(backup =>
            {
                // There are three possible ways to fetch the out path
                // I tried to simplify it as best as I could.
                string path = backup.Output
                    ?? conf.DefaultOut
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "Backups");

                Handlers.CreateBackups(conf, path);
            })
            .WithParsed<ConfigOpts>(config =>
            {
                if (config.Path is not null)
                {
                    if (!File.Exists(config.Path) && !Directory.Exists(config.Path))
                    {
                        Console.Write($"{Starry.Colour.ColourText("HALT!", Colours.Yellow)} \"{config.Path}\" does not exist. Do you wish to add it to your config anyway? [y/N] ");
                        string action = Console.ReadLine()!;
                        action = action.Trim().ToLower();

                        if (action != "y" && action != "yes" && !string.IsNullOrEmpty(action))
                        {
                            return;
                        }
                    }

                    if (conf.Paths.Contains(config.Path))
                    {
                        Console.WriteLine($"{Starry.Colour.ColourText("ERROR:", Colours.Red)} \"{config.Path}\" is already inside your config.");
                        return;
                    }

                    conf.Paths.Add(config.Path);
                    StarConfig.Update(conf);
                }

                if (config.PathIgnore is not null)
                {
                    if (!File.Exists(config.PathIgnore) && !Directory.Exists(config.PathIgnore))
                    {
                        Console.Write($"{Starry.Colour.ColourText("HALT!", Colours.Yellow)} \"{config.PathIgnore}\" does not exist. Do you wish to add it to your config anyway? [y/N] ");
                        string action = Console.ReadLine()!;
                        action = action.Trim().ToLower();

                        if (action != "y" && action != "yes" && !string.IsNullOrEmpty(action))
                        {
                            return;
                        }
                    }

                    conf.IgnorePaths.Add(config.PathIgnore);
                    StarConfig.Update(conf);
                }

                if (config.ShowConfig)
                {
                    conf = StarConfig.Fetch();

                    int count = 0;

                    Console.WriteLine($"{Starry.Colour.ColourText("Backup folders:", Colours.Cyan)}");
                    if (conf.Paths.Count() == 0)
                    {
                        Console.WriteLine(Starry.Colour.ColourText("    No Backup Paths added yet.\n", Colours.Red));
                    }
                    else
                    {

                        foreach (string path in conf.Paths)
                        {
                            count++;
                            Console.WriteLine($"{Starry.Colour.ColourText($"[{count}]", Colours.Magenta)}    {Starry.Colour.ColourText($"\"{path}\"", Colours.Green)}");
                        }
                        Console.WriteLine();
                    }

                    int ignoreCount = 0;

                    Console.WriteLine($"{Starry.Colour.ColourText("Ignore folders:", Colours.Cyan)}");
                    if (conf.IgnorePaths.Count() == 0)
                    {
                        Console.WriteLine(Starry.Colour.ColourText("    No Ignored Folders added yet! Horray!\n", Colours.Green));
                    }
                    else
                    {

                        foreach (string path in conf.IgnorePaths)
                        {
                            ignoreCount++;
                            Console.WriteLine($"{Starry.Colour.ColourText($"[{ignoreCount}]", Colours.Magenta)}    {Starry.Colour.ColourText($"\"{path}\"", Colours.Green)}");
                        }
                        Console.WriteLine();
                    }

                    Console.WriteLine($"{Starry.Colour.ColourText("Default Backup Path:", Colours.Cyan)}  {Starry.Colour.ColourText($"\"{conf.DefaultOut ?? "None"}\"", Colours.Green)}");
                    Console.WriteLine($"{Starry.Colour.ColourText("Zip Directories:", Colours.Cyan)}      {Starry.Colour.ColourText($"{conf.ZipDirs}", Colours.Magenta)}");
                    Console.WriteLine($"{Starry.Colour.ColourText("Zip Parent Directory:", Colours.Cyan)} {Starry.Colour.ColourText($"{conf.ZipParent}", Colours.Magenta)}");
                }

                if (config.ZipParent is not null)
                {
                    conf.ZipParent = (bool)config.ZipParent;
                    StarConfig.Update(conf);
                }

                if (config.ZipDirs is not null)
                {
                    conf.ZipDirs = (bool)config.ZipDirs;
                    StarConfig.Update(conf);
                }

                if (config.OutPath is not null)
                {
                    if (config.OutPath.ToLower() == "reset")
                    {
                        conf.DefaultOut = null;
                        StarConfig.Update(conf);

                        return;
                    }

                    if (Path.HasExtension(config.OutPath))
                    {
                        Console.WriteLine($"{Starry.Colour.ColourText("ERROR:", Colours.Red)} Your backup folder path {Starry.Colour.ColourText("cannot", Colours.Red)} be a file.");
                    }

                    if (File.Exists(config.OutPath) || Directory.Exists(config.OutPath))
                    {
                        Console.Write($"{Starry.Colour.ColourText("STOP!", Colours.Red)} \"{config.OutPath}\" already exists. Do you wish to add it to your config anyway? [y/N] ");
                        string action = Console.ReadLine()!;
                        action = action.Trim().ToLower();

                        if (action != "y" && action != "yes" && !string.IsNullOrEmpty(action))
                        {
                            return;
                        }
                    }

                    conf.DefaultOut = config.OutPath;
                    StarConfig.Update(conf);
                }

                if (config.PathRemove is not null)
                {
                    if (config.PathRemove.ToLower() == "all")
                    {
                        conf.Paths.Clear();
                        StarConfig.Update(conf);

                        return;
                    }

                    Console.Write($"Removing {config.PathRemove}... ");

                    bool success = conf.Paths.Remove(config.PathRemove);
                    Console.WriteLine($"{(success ? Starry.Colour.ColourText("OK", Colours.Green) : Starry.Colour.ColourText("ERR", Colours.Red))}");

                    if (!success)
                    {
                        Console.WriteLine("Failed to remove path. No such path in the config file.");
                        return;
                    }

                    StarConfig.Update(conf);
                }
                
                if (config.PathIgnoreRemove is not null)
                {
                    if (config.PathIgnoreRemove.ToLower() == "all")
                    {
                        conf.IgnorePaths.Clear();
                        StarConfig.Update(conf);

                        return;
                    }

                    Console.Write($"Removing {config.PathIgnoreRemove}... ");

                    bool success = conf.IgnorePaths.Remove(config.PathIgnoreRemove);
                    Console.WriteLine($"{(success ? Starry.Colour.ColourText("OK", Colours.Green) : Starry.Colour.ColourText("ERR", Colours.Red))}");

                    if (!success)
                    {
                        Console.WriteLine("Failed to remove ignore path. No such path in the config file.");
                        return;
                    }

                    StarConfig.Update(conf);
                }
            })
            .WithParsed<HistoryOpts>(history =>
            {
                if (history.HistoryClear)
                {
                    hist.History.Clear();
                    StarConfig.Update(hist);

                    return;
                }
            });
    }
}
