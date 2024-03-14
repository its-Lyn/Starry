using Starry.Source.Client.Colour;
using Starry.Source.Config;
using System.IO.Compression;

namespace Starry.Source.Client;

public static class Handlers
{

    private static readonly ConfigModel Config = StarConfig.Fetch();

    private static void CopyDir(string path, string output)
    {
        if (Config.IgnorePaths.Contains(path))
        {
            return;
        }

        // Start the process of copying over directory
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
            Console.WriteLine($"No such Directory: {path}. Ignoring.");

            return;
        }

        // Get directories and files from base Directory
        DirectoryInfo[] dirs = dir.GetDirectories();
        FileInfo[] files = dir.GetFiles();

        // Create new directory
        string dest = Path.Combine(output, dir.Name);
        Directory.CreateDirectory(dest);

        foreach (FileInfo file in files)
        {
            if (Config.IgnorePaths.Contains(file.FullName))
            {
                continue;
            }

            file.CopyTo(Path.Combine(dest, file.Name));
        }

        foreach (DirectoryInfo subDir in dirs)
            CopyDir(subDir.FullName, dest);
    }

    public static void CreateBackups(ConfigModel config, string output)
    {
        if (File.Exists(output) || Directory.Exists(output))
        {
            Console.Write($"The path {output} already exists. Do you wish to overwrite it? [Y/n] ");
            string action = Console.ReadLine()!;
            action = action.Trim().ToLower();

            if (action == "y" || action == "yes" || string.IsNullOrEmpty(action))
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
                else if (File.Exists(output))
                    File.Delete(output);
            }
        }

        Console.Write("Checking file paths... ");
        if (config.Paths.Count == 0)
        {
            Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
            Console.WriteLine("There are no paths to backup. Please run `starry config help`");

            return;
        }

        Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));

        Console.Write("Creating parent directory... ");
        if (Path.HasExtension(output))
        {
            Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
            Console.WriteLine("Please input a valid Directory path, not a File path.");

            return;
        }

        Directory.CreateDirectory(output);

        Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));

        foreach (string path in config.Paths)
        {
            if (Config.IgnorePaths.Contains(path))
            {
                Console.WriteLine($"{path} is set to be ignored. Skipping.");
                continue;
            }

            Console.Write($"Working on {Path.GetFileName(path)}... ");

            if (config.ZipDirs)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Copy(path, Path.Combine(output, Path.GetFileName(path)));
                    }
                    else if (Directory.Exists(path))
                    {
                        ZipFile.CreateFromDirectory(path, Path.Combine(output, $"{Path.GetFileName(path)!}.zip"));
                    }
                    else
                    {
                        Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
                        Console.WriteLine($"No such file name or directory: {path}. Ignoring.");

                        continue;
                    }

                    Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));
                }
                catch (Exception e)
                {
                    Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
                    Console.WriteLine($"Failed to compress {path} with: {e.Message}. Ignoring.");
                }

                continue;
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Copy(path, Path.Combine(output, Path.GetFileName(path)));
                    Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));

                    continue;
                }

                CopyDir(path, output);

                Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));
            }
            catch (Exception e)
            {
                Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
                Console.WriteLine($"Failed to copy {path} with: {e.Message}. Ignoring.");
            }
        }

        if (config.ZipParent)
        {
            Console.Write($"Compressing {output}... ");

            try
            {
                ZipFile.CreateFromDirectory(output, $"{output}.zip");

                Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));
            }
            catch (Exception e)
            {
                Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
                Console.WriteLine($"Failed to compress {output} with: {e.Message}.");

                return;
            }

            Console.Write("Cleaning up... ");

            try
            {
                Directory.Delete(output, true);
                Console.WriteLine(Starry.Colour.ColourText("OK", Colours.Green));
            }
            catch (Exception e)
            {
                Console.WriteLine(Starry.Colour.ColourText("ERR", Colours.Red));
                Console.WriteLine($"Failed to clean up: {e}.");
            }
        }
    }
}
