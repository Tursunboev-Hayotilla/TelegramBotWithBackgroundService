using Microsoft.Bot.Configuration;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBotWithBackgroundService.Bot.Models;
using TelegramBotWithBackgroundService.Bot.Persistance;
using TelegramBotWithBackgroundService.Bot.Services.BackgroundServices;
using TelegramBotWithBackgroundService.Bot.Services.Handlers;
using TelegramBotWithBackgroundService.Bot.Services.UserRepositories;

namespace TelegramBotWithBackgroundService.Bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddDbContext<AppBotDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=BackService;User Id=postgres;Password=coder;");
            });
            builder.Services.AddSingleton(p => new TelegramBotClient("6726665639:AAEzGElAxUHe7js1j5qJTMVDRaXUexIFfvI"));

            builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();
            var botConfig = builder.Configuration.GetSection("BotConfiguration")
   .Get<BotConfig>();

            builder.Services.AddHttpClient("webhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(botConfig.Token, httpClient));

            builder.Services.AddHostedService<BotBackgroundService>();
            builder.Services.AddHostedService<NmaGap>();

            var app = builder.Build();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
