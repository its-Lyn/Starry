using System.Text.Json.Serialization;

namespace Starry.Source.Config.Models;

public class ConfigModel
{
    [JsonPropertyName("paths")]
    public required List<string> Paths { get; set; }
    
    [JsonPropertyName("ignore_paths")]
    public required List<string> IgnorePaths { get; set; }

    [JsonPropertyName("default_output")]
    public string? DefaultOut { get; set; }

    [JsonPropertyName("zip_directories")]
    public bool ZipDirs { get; set; }

    [JsonPropertyName("zip_parent")]
    public bool ZipParent { get; set; }
}
