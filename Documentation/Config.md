# Configuration

When running Starry, it will create a configuration folder. <br>
Starry will first look for the XDG_CONFIG_HOME environment variable, if it can't find it, it will create the configuration folder in $HOME/.config/Starry

Starry is configured through it's CLI, but, it uses JSON under the hood.

## Configuration Structure
| Name            | Type         | Description                                                                                                                | Default Value |
|-----------------|--------------|----------------------------------------------------------------------------------------------------------------------------|---------------|
| paths           | List<string> | A list of the paths Starry will backup to.                                                                                 | Empty         |
| ignore_paths    | List<string> | A list of the paths Starry will ignore when Backing up files.                                                              | Empty         |
| default_output  | string?      | Defiens where Starry will put the backup folder, if left null, it will put it in <WorkingDir>/Backups                      | null          |
| zip_directories | bool         | Defines wether Starry should create zip archives for each backed up folder.                                                | false         |
| zip_parent      | bool         | Defines wether Starry should create a zip archive for the parent folder of the backup. Eg. Backups will become Backups.zip | true          |

## How-To Configure
By running `starry config --help` you'll get a pretty good idea of how configration is done.

```
Copyright (C) 2024 Evelyn Serra

  -a, --add-path           Add a path to a folder or a file to backup.

  -i, --ignore-path        Ignore a specific path from being backed up.

  -r, --remove-path        Remove a backup path from your config. Type all to remove all of the paths.

  -e, --remove-ignore      Remove an ignore path from your config. Type all to remove all of the paths.

  -l, --list-config        Show all the paths in your config.

  -o, --default-output     Set the default path Starry will send backups to. Default is <working direcory>/Backups. Type
                           reset to set it back to null.

  -d, --zip-directories    Wether or not to zip up the copied directories.

  -p, --zip-parent         Wether or not to zip up the parent directory. For example <dir>/Backups will be
                           <dir>/Backups.zip

  --help                   Display this help screen.

  --version                Display version information.
```

A few quirks are that when running `--remove-path` you can do `--remove-path all` and all the backup paths will be removed, while the same stands true for it's `--remove-ignore` counterpart. For `--default-output` you can do `--default-output reset` and its value will be reset back to `null`

