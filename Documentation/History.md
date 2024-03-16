# History
Starry will create a `history.json` file inside your config directory.

This file will keep track of when and what you backed up. This feature can be seen by using `starry history --help`.

## Structure

### History 
| Name           | Type       | Description                                   |
|----------------|------------|-----------------------------------------------|
| backup_history | List<Item> | The list of backed up data Starry reads from. |

### Item
| Name  | Type         | Description                              |
|-------|--------------|------------------------------------------|
| files | List<string> | The files and folders Starry backed up   |
| date  | string       | The date and time the backup took place. |

## Usage
Once you've started backing up, you can use `starry history --list` to list your history.

You can also use `starry history --clear` to clear the history.