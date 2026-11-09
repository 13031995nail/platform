namespace Tmobis.Offline.Models;
using System;
using System.Collections.Generic;

public class JsonBackupInfo
{
    public Dictionary<Guid, JsonBackupPartsInfo> 
    SubParts  { get; set; } = [];

    public JsonBackupPartsInfo? Parts { get; set; }
}
