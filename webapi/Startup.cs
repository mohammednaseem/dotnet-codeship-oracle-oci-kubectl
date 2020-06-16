using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using InitApp.BackgroundJob;
using InitApp.Base;

namespace InitApp
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {    
           var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                                .AddEnvironmentVariables()
                                .Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string AppConnectServerName = Configuration.GetValue<string>("AppConnectClientName");
            string ipAddress = Configuration.GetValue<string>("AppConnectServerIP");
            
            Console.WriteLine("The ipAddress of this Pod: " + ipAddress);  
            services.AddControllers();
            services.AddHealthChecks()
                    .AddCheck<ExHealthCheck>("ex_health_check");
            services.AddSingleton<IAppConnectSideCar, IBMAppConnectService>();

            services.AddHttpClient(AppConnectServerName)
                .ConfigureHttpMessageHandlerBuilder(builder =>
                {
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/Health");
            });
        }
    }
}
