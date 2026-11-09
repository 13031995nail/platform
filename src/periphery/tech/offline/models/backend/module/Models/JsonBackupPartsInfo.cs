namespace Tmobis.Offline.Models;
using System.Collections.Generic;
using System.IO;

public class JsonBackupPartsInfo
{
    public string? SubDir { get; set; }

    public Dictionary<int, string> PartsLocalPaths { get; set; } = [];

    public JsonBackupPartsInfo() { }

    public JsonBackupPartsInfo(string subDir)
    {
        SubDir = subDir;
    }
}
