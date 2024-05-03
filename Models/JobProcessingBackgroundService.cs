using LifeLineApi.Controllers;



    public class JobProcessingBackgroundService : BackgroundService
    {
        private readonly ILogger<JobProcessingBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobProcessingBackgroundService(ILogger<JobProcessingBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var patternController = scope.ServiceProvider.GetRequiredService<patient_cr>();
                     patternController.Realtimewaitingupdate();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
             }
        }
    }

