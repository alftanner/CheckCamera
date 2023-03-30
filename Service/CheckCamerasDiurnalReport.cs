using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CameraCheck;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CameraCheck.Models;
using Telegram.Bot;
using Telegram.Bots.Types.Stickers;

namespace CameraCheck.Service;

public class ChekCamerasMessageServiceDiurnal : BackgroundService
{
    private readonly PingCommand _pingCommand;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly List<long> _admins;

    public ChekCamerasMessageServiceDiurnal(PingCommand pingCommand, ITelegramBotClient telegramBotClient,
        IOptions<TelegramSettings> admins)
    {
        _admins = admins.Value.Admins;
        _pingCommand = pingCommand;
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(23));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var messageResult = await _pingCommand.RunPing();
            var sb = new StringBuilder();

            foreach (var element in messageResult)
            {
                if (element.IsSuccess == false)
                {
                    sb.AppendLine(element.ToString());
                }
            }

            var messageToSend = sb.ToString();
            if (!string.IsNullOrWhiteSpace(messageToSend))
            {
                foreach (var adminChat in _admins)
                {
                    await _telegramBotClient.SendTextMessageAsync(adminChat, sb.ToString(),
                        cancellationToken: stoppingToken);
                }
            }
        }
    }
}