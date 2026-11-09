namespace Tmobis.Files.Provider.Remote;

public class RemoteFileProviderOptions
{
    public const string RemoteFile = "RemoteFile";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiKeyHeaderName { get; set; } = string.Empty;

    /// <summary>
    /// URL получения файла, где
    /// {fileName} - имя файла,
    /// {fileId} - ID файла.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    public string FileNameParam { get; set; } = "fileName";
    
    public string FileIdParam { get; set; } = "fileUploadKey";
}