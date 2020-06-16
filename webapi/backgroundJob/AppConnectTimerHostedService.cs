using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using InitApp.Base;


namespace InitApp.BackgroundJob
{
    public class TimerHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimerHostedService> _logger;

        private IAppConnectSideCar _IAppConnectSideCar;
        private Timer _timer;       

        public TimerHostedService(ILogger<TimerHostedService> logger,
                                   IAppConnectSideCar _iAppConnectSideCar)
        {
            _logger = logger;
            _IAppConnectSideCar = _iAppConnectSideCar;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            //_timer = new Timer(_IAppConnectSideCar.PrepAppConnect, null, TimeSpan.Zero, 
              //  TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}