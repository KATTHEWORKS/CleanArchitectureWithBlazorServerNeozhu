using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem;

public class VoteSummaryBackgroundService : BackgroundService
{
    public static bool DatabaseChanged { get; set; } = false;
    private readonly IServiceProvider _serviceProvider;
    // private readonly ILogger<DataSynchronizationService> _logger;

    public VoteSummaryBackgroundService(IServiceProvider serviceProvider)//, ILogger<DataSynchronizationService> logger)
    {
        _serviceProvider = serviceProvider;
        // _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check if it's time to synchronize data and there are database changes
            if (IsTimeToSyncData() && DatabaseChanged)
            {
                await SynchronizeData();
                //await _services.RefreshSummary();
                DatabaseChanged = false; // Reset the flag
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Check every 5 minutes
        }
    }

    private async Task SynchronizeData()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var summaryService = scope.ServiceProvider.GetRequiredService<IVoteSummaryService>();
            try
            {
                await summaryService.RefreshSummary();
                // _logger.LogInformation("Data synchronization completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // _logger.LogError(ex, "Error occurred during data synchronization.");
            }
        }
    }

    private bool IsTimeToSyncData()
    {
        var currentTime = DateTime.Now.TimeOfDay;
        var startTime = new TimeSpan(7, 0, 0); // 7:00 AM
        var endTime = new TimeSpan(23, 0, 0);  // 11:00 PM
        return currentTime >= startTime && currentTime <= endTime;
    }
}
