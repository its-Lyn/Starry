using System.Text.Json.Serialization;

namespace Starry.Source.Config.Models;

public class Item
{
    [JsonPropertyName("folders")]
    public required List<string> Backed { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class HistoryModel
{
    [JsonPropertyName("backup_history")]
    public required List<Item> History { get; set; }
}
