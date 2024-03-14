using CommandLine;
using Starry.Source.Config;

namespace Starry.Source.Client;

public class StarParser
{
    [Verb("backup", HelpText = "Backup the directories from your configuration file.")]
    private class BackupOpts
    {
        [Option('o', "output", Required = false, HelpText = "The path to output to, not using the default")]
        public string? Output { get; set; }
    }

    // TODO
    [Verb("config", HelpText = "Create and edit the configuration file.")]
    private class ConfigOpts { }

    public void Parse(string[] args)
    {
        ConfigModel conf = StarConfig.Fetch();
        
        Parser.Default.ParseArguments<BackupOpts>(args)
            .WithParsed<BackupOpts>(backup =>
            {
                // There are three possible ways to fetch the out path
                // I tried to simplify it as best as I could.
                string path = backup.Output
                    ?? conf.DefaultOut
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "Backups");

                Handlers.CreateBackups(conf, path);
            });
    }
}
