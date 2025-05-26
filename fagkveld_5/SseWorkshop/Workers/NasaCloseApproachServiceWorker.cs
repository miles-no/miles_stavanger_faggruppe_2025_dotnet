using SseWorkshop.Services;

namespace SseWorkshop.Workers
{
    public class NasaCloseApproachServiceWorker(NasaCloseApproachService closeApproachService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {

                await closeApproachService.UpdateCloseApproachDataAsync(cancellationToken);
                await Task.Delay(60*1000, cancellationToken);
            }
        }
    }
}
