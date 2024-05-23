using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotWithBackgroundService.Bot.Models;
using TelegramBotWithBackgroundService.Bot.Persistance;
using TelegramBotWithBackgroundService.Bot.Services.UserRepositories;

namespace TelegramBotWithBackgroundService.Bot.Services.BackgroundServices
{
    public class NmaGap : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramBotClient _client;

        public NmaGap(IServiceScopeFactory serviceScopeFactory, TelegramBotClient client)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var users = await userRepository.GetAllUsers();

                    foreach (var user in users)
                    {
                        await SendNotification(user, stoppingToken);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private Task SendNotification(UserModel user, CancellationToken token)
        {
            try
            {
                return _client.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "Nima gaplar",
                    cancellationToken: token);
            }
            catch (Exception ex)
            {
                return Task.CompletedTask;
            }
        }
    }
}