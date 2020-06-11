using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitAppController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<InitAppController> _logger;

        public InitAppController(ILogger<InitAppController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            var rng = new Random();
            return "Health InitApp, Life InitApp, Dental InitApp, Vision InitApp"; 
        }
    }
}
