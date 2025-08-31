using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.BackgroundServices
{

    public class StartupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StartupService> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public StartupService(ILogger<StartupService> logger,
            IHostApplicationLifetime appLifetime,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken _)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                // fire-and-forget (log exceptions inside the method)
                Task.Run(OnStartedAsync);
            });

            return Task.CompletedTask;
        }

        private async Task OnStartedAsync()
        {
            _logger.LogInformation("Application started/restarted");
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var priceRebates = await unitOfWork.PriceRebates.GetByConditions(pr => pr.Status == PriceRebateStatus.PROCCESSING);

                foreach (var priceRebate in priceRebates)
                {
                    priceRebate.Status = PriceRebateStatus.PENDING;
                    unitOfWork.PriceRebates.Update(priceRebate);
                }

                await unitOfWork.CompleteAsync();
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("Application is stopping");
        }

        private void OnStopped()
        {
            _logger.LogInformation("Application stopped");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}