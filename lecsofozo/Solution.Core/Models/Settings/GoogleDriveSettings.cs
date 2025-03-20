namespace Solution.Core.Models.Settings;

public class GoogleDriveSettings: JsonCredentialParameters
{
    [JsonPropertyName("custom_folder")]
    public string CustomFolder {  get; set; }

    [JsonPropertyName("RootFolderId")]
    public string RootFolderId { get; set; }

    [JsonPropertyName("FolderId")]
    public string FolderId { get; set; }
}
