using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http; 
using Microsoft.Extensions.Configuration;
using System.Text;

namespace InitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitAppController : ControllerBase
    {
        
        private readonly IHttpClientFactory _clientFactory;
        public IConfiguration _configuration { get; }
        private readonly ILogger<InitAppController> _logger;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

  
        public InitAppController(IConfiguration configuration, 
                                IHttpClientFactory clientFactory, 
                                ILogger<InitAppController> logger)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration  = configuration;
        }
        string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sec=\"http://www.approuter.com/schemas/2008/1/security\">    <soapenv:Header/>    <soapenv:Body>        <sec:login>          <sec:username>admin</sec:username>          <sec:password>!n0r1t5@C</sec:password> </sec:login>    </soapenv:Body> </soapenv:Envelope>";
      
        [HttpGet]
        public async Task<string> Get()
        {
            string toReturn = string.Empty;
            try {

                    var url = "https://127.0.0.1:8443/ws/security";// _configuration["AppConnectServer"];
                    _logger.LogInformation("The url: " + url);
                    Console.WriteLine("The url: " + url);
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    
                    //request.Headers.Add("Content-Type", "text/xml");
                    var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                    request.Content = httpContent;
                    
                    var client = _clientFactory.CreateClient("AppConnectClient");   
                                  
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        toReturn = await response.Content.ReadAsStringAsync();    
                    }
                    else
                    {
                        toReturn = response.StatusCode + "" + response.ReasonPhrase;
                    }
                    Console.WriteLine("The toReturn: " + toReturn);
                    _logger.LogInformation("The toReturn: " + toReturn);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return ex.ToString();
            }
            return toReturn;
        }
    }
}
