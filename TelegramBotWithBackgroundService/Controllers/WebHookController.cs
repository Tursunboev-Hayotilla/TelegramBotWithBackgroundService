using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotWithBackgroundService.Bot.Services.Handlers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TelegramBotWithBackgroundService.Bot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly BotUpdateHandler _handler;
        private readonly ITelegramBotClient _client;

        public WebHookController(BotUpdateHandler handler, ITelegramBotClient client)
        {
            _handler = handler;
            _client = client;
        }

        [HttpPost]
        public async Task<IActionResult> Connector([FromBody] Telegram.Bot.Types.Update update, CancellationToken cancellation)
        {
            await _handler.HandleUpdateAsync(_client, update, cancellation);

            return Ok();
        }
    }
}
