using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace InitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitAppController : ControllerBase
    {
        private readonly ILogger<InitAppController> _logger;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
  
        public InitAppController(IConfiguration configuration,                                  
                                ILogger<InitAppController> logger)
        {    
            _logger = logger;
        }

        [HttpGet]
        [Route("/InitApp")]               
        public void Get([FromRoute]string ipaddressP)
        {
            
        }
    }
}
