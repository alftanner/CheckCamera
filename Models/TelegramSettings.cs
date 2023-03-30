using System.Collections.Generic;

namespace CameraCheck.Models;

public class TelegramSettings
{
    public const string ConfigureAdminsList = "telegramSettings";
    public List<long> Admins { get; set; }
}
