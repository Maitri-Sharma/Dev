using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Route("[action]")]
        [HttpGet]
        public DateTime GetDate()
        {
            _logger.LogTrace("Logged into GetDate Method using Tracing");

            _logger.LogInformation("Logged into GetDate Method");
            return DateTime.Now;
        }
        
        [HttpPost]
        [Route("[action]")]
        public DateTime DateusingPOST([FromBody] TestClass value)
        {
            _logger.LogInformation("Logged into DateusingPOST Method and Parameter {value}", value);
            if (value != null && value.IsUTCDateTimerequired)
                return DateTime.UtcNow;
            return DateTime.Now;
        }

        [Route("[action]")]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Route("[action]")]
        [HttpGet]
        public IEnumerable<WeatherForecast> GetWeather(int minTemp, int maxTemp)
        {

            _logger.LogInformation("Logged into GetWeather Method");
            var rng = new Random();

            IEnumerable<WeatherForecast> result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(minTemp, maxTemp),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("Result is ",result);

            return result;
        }

        [Route("[action]")]
        [HttpPost]
        public IEnumerable<WeatherForecast> PostWeather(int minTemp, int maxTemp)
        {

            _logger.LogInformation("Logged into PostWeather Method");
            var rng = new Random();

            IEnumerable<WeatherForecast> result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(minTemp, maxTemp),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("Result is ", result);

            return result;
        }
    }
}
