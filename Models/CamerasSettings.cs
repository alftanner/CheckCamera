using System.Collections.Generic;

namespace CameraCheck.Models;

public class CameraSettings
{
    public const string ConfigureSettingsCameras = "camerasSettings";
    public List<CameraSummary> Settings { get; set; }
    
}

public class CameraSummary
{
    public string LocationName { get; set; }
    public string Ip { get; set; }
    public int CameraPort { get; set; }
}