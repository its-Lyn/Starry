using System.Text.Json.Serialization;

namespace Starry.Source.Config.Models;

public class Item
{
    [JsonPropertyName("files")]
    public required List<string> Backed { get; set; }

    [JsonPropertyName("date")]
    public required string Date { get; set; }
}

public class HistoryModel
{
    [JsonPropertyName("backup_history")]
    public required List<Item> History { get; set; }
}
