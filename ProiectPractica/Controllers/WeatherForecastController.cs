using Microsoft.AspNetCore.Mvc;
using ProiectPractica;
using ProiectPractica.Data;

namespace Practica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly SocialMediaDB _db;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, SocialMediaDB db)
        {
            _logger = logger;
            _db = db;

        }

        [HttpGet("PreziVremeaCumetre")]
        public IEnumerable<WeatherForecast> Get(int numberOfDaysToPredict, bool isHoliday)
        {
            var toReturn = Enumerable.Range(1, numberOfDaysToPredict).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return toReturn;
        }
    }
}