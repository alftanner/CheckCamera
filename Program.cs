using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CameraCheck.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CameraCheck.Service;
using CameraCheck.Telegram;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;



namespace CameraCheck;

class Program
{
    private static Logger logger = LogManager.GetCurrentClassLogger();

    static async Task Main(string[] args)
    {


        var builder = new HostBuilder()
            .ConfigureAppConfiguration(confBuilder =>
            {
                confBuilder.AddJsonFile("appsettings.json");
                confBuilder.AddCommandLine(args);
            })
            .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty)
            .ConfigureLogging((configLogging) =>
            {
                configLogging.AddConsole();
                configLogging.AddDebug();
            })
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<TelegramSettings>(options =>
                    configuration.GetSection(TelegramSettings.ConfigureAdminsList).Bind(options));
                services.AddSingleton<ITelegramBotClient>(x =>
                {
                    var bot = new TelegramBotClient(configuration["botToken"])
                    {
                        Timeout = Timeout.InfiniteTimeSpan
                    };
                    return bot;
                });
                services.AddSingleton<IUpdateHandler, UpdateHandler>();
                services.Configure<CameraSettings>(options =>
                    configuration.GetSection(CameraSettings.ConfigureSettingsCameras).Bind(options));
                services.AddTransient<PingCommand>();
                services.AddHostedService<TelegramService>();
                services.AddHostedService<CheckCamerasMessageService>();
                services.AddHostedService<ChekCamerasMessageServiceDiurnal>();
                
                

            });

        await builder.RunConsoleAsync();
    }
}