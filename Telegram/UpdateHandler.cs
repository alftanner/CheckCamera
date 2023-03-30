using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Fluent;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CameraCheck.Telegram
{
    
    public class UpdateHandler : IUpdateHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly PingCommand _worker;
        // private readonly Settings _settings;

        public UpdateHandler(PingCommand worker)
        {
            _worker = worker;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            switch (update)
            {
                case
                    {
                        Type: UpdateType.Message,
                        Message: { Text: { } text, Chat: { } },
                    }
                    when text.Equals("/status", StringComparison.OrdinalIgnoreCase):

                {
                    var result = await _worker.RunPing();
                    var sb = new StringBuilder();


                    foreach (var element in result)
                    {
                        if (element.IsSuccess == false)
                        {
                            sb.AppendLine(element.Message);
                        }
                        else
                        {
                            logger.Debug("Camera не секс!");
                            sb.AppendLine(element.Message);
                        }
                    }

                    var messageToSend = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(messageToSend))
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                            $"Ответ на ваш запрос, Черный Властелин: \n \n{sb}",
                            cancellationToken: cancellationToken);
                    }
                   
                    break;
                }

            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}