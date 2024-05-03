using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LifeLineApi.Controllers;
using LifeLineApi.Models;

namespace LifeLineApi.Models
{
    public class ThreadImplementation : BackgroundService
    {
        private readonly ILogger<ThreadImplementation> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Thread _backgroundThread;

        public ThreadImplementation(ILogger<ThreadImplementation> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _backgroundThread = new Thread(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<LifeLinedbContext>();
                        var patternController = scope.ServiceProvider.GetRequiredService<patient_cr>();

                        var todayAppointments = dbContext.Realtimewaitingtbs
                            .Where(appointment => appointment.PDate != null && appointment.PDate.Value.Date == DateTime.Today && appointment.Status != "sent")
                            .ToList();

                        foreach (var appointment in todayAppointments)
                        {
                            patternController.Realtimewaitingupdate();
                        }
                    }

                  
                }
            });

            _backgroundThread.Start();

            return Task.CompletedTask;
        }
    }
}
