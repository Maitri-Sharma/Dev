using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        #region Variables
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrdersController> _logger;
        #endregion
        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            _logger.BeginScope("Inside into CreateOrderAsync");
            IQueueClient queueClient = new QueueClient("Endpoint=sb://sb-puma-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/6iG+tI/TDtOvuW0L/TWnsZVecFVrW/ZWUDLH4FOv+E=", "recreatequeue-dev");
            var orderJSON = JsonConvert.SerializeObject(order);
            var orderMessage = new Message(Encoding.UTF8.GetBytes(orderJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            await queueClient.SendAsync(orderMessage).ConfigureAwait(false);
            await queueClient.CloseAsync();

            return Ok("Create order message has been successfully pushed to queue");
        }
    }
}
