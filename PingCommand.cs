using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CameraCheck.Models;
using Microsoft.Extensions.Options;
using NLog;

namespace CameraCheck;

public class PingCommand
{
    private static Logger logger = LogManager.GetCurrentClassLogger();

    private readonly List<CameraSummary> _cameraSummary;

    public PingCommand(IOptions<CameraSettings> mSummary)
    {
        _cameraSummary = mSummary.Value.Settings;
    }

    #region RunPingMethod

    public Task<List<PingResult>> RunPing()
    {
        var ping = new Ping();
        var result = new List<PingResult>();


        foreach (var t in _cameraSummary)
        {
            var pResult = new PingResult();
            var sb = new StringBuilder();
            try
            {
                var pingReply = ping.Send(t.Ip);

                if (pingReply.Status != IPStatus.TimedOut)
                {
                    sb.AppendLine($"Локация: {t.LocationName}");
                    sb.AppendLine($"Адрес: {t.Ip}");
                    sb.AppendLine($"Состояние: {pingReply.Status.ToString()}");
                    sb.AppendLine();
                    pResult.IsSuccess = true;
                }
                else
                {
                    sb.AppendLine($"Локация: {t.LocationName}");
                    sb.AppendLine($"Адрес: {t.Ip}");
                    sb.AppendLine($"Состояние: {pingReply.Status.ToString()}");
                    sb.AppendLine();
                    logger.Debug($"Недоступна камера:{t.LocationName}");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            pResult.Message = sb.ToString();
            result.Add(pResult);
        }


        return Task.FromResult(result);
    }

    #endregion
}