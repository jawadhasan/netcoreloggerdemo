using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreLogger.Web.CoreLogger;

namespace NetCoreLogger.Web.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //Diagnostic Log: Only logs when Env_Variable is Set for this type of log
            WebHelper.LogWebDiagnostic("NetCoreLogger.Web", "WebApi", "Just checking in...", HttpContext,
                new Dictionary<string, object> { { "Very", "Important" } });


            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }



        [HttpGet]
        [Route("[action]")]
        [TrackUsage("NetCoreLogger.Web", "WebApi", "SimulateWork")]
        public IEnumerable<WeatherForecast> SimulateWork()
        {
           var rng = new Random();
            return Enumerable.Range(1, 5000).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }


        [HttpGet]
        [Route("[action]")]
        public IEnumerable<WeatherForecast> SimulateError()
        {
            //Simulating an error
            var ex = new Exception("Something bad has happened!");
            ex.Data.Add("input param", "nothing to see here");
            throw ex;
        }
    }
}
