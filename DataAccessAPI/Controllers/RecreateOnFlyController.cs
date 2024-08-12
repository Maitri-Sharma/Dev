using System;
using System.Text;
using System.Threading.Tasks;
using DataAccessAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecreateOnFlyController : ControllerBase
    {
        #region Variables
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecreateOnFlyController> _logger;
        #endregion
        public RecreateOnFlyController(IConfiguration configuration, ILogger<RecreateOnFlyController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendSelectionForRecreation([FromBody] RecreateQueueMessageItem recreateQueueMessageItem)
        {
            _logger.BeginScope("Inside into SendSelectionForRecreation");
            IQueueClient queueClient = new QueueClient(_configuration.GetSection("QueueEndpoint").Value, _configuration.GetSection("recreatequeue-dev").Value);
            var selectionJSON = JsonConvert.SerializeObject(recreateQueueMessageItem);
            var selectionMessage = new Message(Encoding.UTF8.GetBytes(selectionJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            await queueClient.SendAsync(selectionMessage).ConfigureAwait(false);
            await queueClient.CloseAsync();

            return Ok("Selection for recreation has been successfully pushed to queue");
        }
    }
}
