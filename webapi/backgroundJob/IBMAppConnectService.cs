using System;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InitApp.Base;

namespace InitApp.BackgroundJob
{
    public class IBMAppConnectService : IAppConnectSideCar
    {
        private int executionCount = 0;
        private int counter = 0;
        private readonly ILogger<IBMAppConnectService> _logger;      
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;   

        public IBMAppConnectService(ILogger<IBMAppConnectService> logger,
                                    IHttpClientFactory clientFactory,
                                    IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public void PrepAppConnect(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            Login(null).Wait();

            Upload(null).Wait();

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count} Lodalcounter: {counter}", count, counter++);
        }

        private string GetSessionId(string XmlResponse)
        {
            XmlDocument xmldoc = new XmlDocument();   
            xmldoc.LoadXml(XmlResponse);  
            XmlNamespaceManager ns = new XmlNamespaceManager(xmldoc.NameTable);
            ns.AddNamespace("S", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("ns2", "http://www.approuter.com/schemas/2008/1/security");

            XmlNode node = xmldoc.SelectSingleNode("//ns2:sessionId", ns); 
            string sessionId = node.InnerText;  
            
            _logger.LogInformation("The sessionId: " + sessionId);
            _logger.LogInformation("The XmlResponse: " + XmlResponse);
            return sessionId;
        }

        string SessionId;
        string loginXml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sec=\"http://www.approuter.com/schemas/2008/1/security\">    <soapenv:Header/>    <soapenv:Body>        <sec:login>          <sec:username>admin</sec:username>          <sec:password>!n0r1t5@C</sec:password> </sec:login>    </soapenv:Body> </soapenv:Envelope>";
        public async Task<string> Login(string ipaddressP)
        {
            string toReturn = string.Empty;
            try 
            {
                string ipAddress =  _configuration.GetValue<string>("AppConnectServerIP");
                if(ipaddressP != null) ipAddress = ipaddressP; 

                var url = "https://" + ipAddress + ":8443/ws/security";  
                _logger.LogInformation("The url: " + url);
                Console.WriteLine("The url: " + url);
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                    
                var httpContent = new StringContent(loginXml, Encoding.UTF8, "text/xml");
                request.Content = httpContent;
                    
                var client = _clientFactory.CreateClient("AppConnectClient");   
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    toReturn = await response.Content.ReadAsStringAsync();    
                    SessionId = GetSessionId(toReturn);
                    Console.WriteLine("The SessionId: " + SessionId);
                    _logger.LogInformation("The SessionId: " + SessionId);
                }
                else
                {
                    toReturn = response.StatusCode + "" + response.ReasonPhrase;
                    Console.WriteLine("The Error: " + toReturn);
                    _logger.LogInformation("The Error: " + toReturn);
                }
                return toReturn;
            }
            catch(Exception ex)
            {
               Console.WriteLine(ex);
               throw ex;
            }
        }

        string deploymentXml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:dep=\"http://www.approuter.com/schemas/2008/1/deployment\"> <soapenv:Header>      <dep:sessionId>SESSIONID</dep:sessionId>   </soapenv:Header> <soapenv:Body>      <dep:publishProject>      <dep:projectName>ff</dep:projectName>  <dep:version>4</dep:version>   <dep:content>cid:AddTwoNumbers.par</dep:content>  </dep:publishProject> </soapenv:Body> </soapenv:Envelope>";
        public async Task Upload(string ipaddressP)
        {            
            deploymentXml  = deploymentXml.Replace("SESSIONID", SessionId);            
            var client = _clientFactory.CreateClient("AppConnectClient");

            string toReturn = string.Empty;
            //try 
            {
                string ipAddress =  _configuration.GetValue<string>("AppConnectServerIP");
                if(ipaddressP != null) ipAddress = ipaddressP; 
                var url = "https://" + ipAddress + ":8443/ws/deployment";  
                 
                var request = new HttpRequestMessage(HttpMethod.Post, url);                 
                byte[] fileContents = File.ReadAllBytes("filepath/AddTwoNumbers.par");

                //request.Headers.Add("Content-Type", "application/octet-stream");
                //request.Headers.Add("ContentLength", fileContents.Length.ToString());
                //using (var stream = new FileStream("filepath/AddTwoNumbers.par", FileMode.Open))
                {
                   // await request.Content.ReadAsByteArrayAsync().CopyToAsync(stream);
                }
                 
                MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
                //ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
                //byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                //byteArrayContent.Headers.Add("Type","Content");
                //byteArrayContent.Headers.Add("ContentLength", fileContents.Length.ToString());

                var httpContent = new StringContent(deploymentXml, Encoding.UTF8, "application/octet-stream");
                multiPartContent.Add(httpContent);
                //multiPartContent.Add(byteArrayContent, "this is the name of the content", "AddTwoNumbers.par");
                
                request.Content = multiPartContent;
                 
                var response = await client.SendAsync(request);
                Console.WriteLine();
                Console.WriteLine("###################################");
                Console.WriteLine();

                if (response.IsSuccessStatusCode)
                {
                    toReturn = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    toReturn = response.StatusCode + "" + response.ReasonPhrase;                    
                }
                Console.WriteLine("The response: " + toReturn);
                 _logger.LogInformation("The response: " + toReturn);
                Console.WriteLine();
                Console.WriteLine("###################################");
                Console.WriteLine();
                
            }
            //catch(Exception ex)
            {
               //Console.WriteLine(ex);
               //throw ex;
            }

            
        }
    }
}