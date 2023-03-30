using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;


namespace CameraCheck.Telegram
{
    public class TelegramService : BackgroundService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramBotClient _bot;
        private readonly ILogger<TelegramService> _logger;
        private readonly IUpdateHandler _updateHandler;

        public TelegramService(ITelegramBotClient bot, ILogger<TelegramService> logger,
            IUpdateHandler updateHandler, IServiceProvider serviceCollection)
        {
            
            _bot = bot;
            _logger = logger;
            _updateHandler = updateHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //var bot = new TelegramBotClient(Configuration.BotToken);
                var me = await _bot.GetMeAsync();
                if (me.Username != null)
                {
                    Console.Title = me.Username;

                    using var cts = new CancellationTokenSource();
                    // ReSharper disable once AccessToDisposedClosure
                    Console.CancelKeyPress += (_, _) => cts.Cancel(
                    ); //  ctrl+C, sigterm, sigkill, etc

                    //var handler = new UpdateHandler();
                    var receiverOptions = new ReceiverOptions();
                    // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                    _bot.StartReceiving(_updateHandler, receiverOptions, cancellationToken: cts.Token);
                    logger.Debug("Bot started.");
                    
                    await Task.Delay(-1,
                        cancellationToken: cts
                            .Token); 
                    logger.Debug("Bot stopped.");
                    Console.WriteLine($"Start listening for @{me.Username}");
                    Console.ReadLine();

                    // Send cancellation request to stop bot
                    cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}