using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using SseWorkshop.External.Nasa;

namespace SseWorkshop.Services
{
    public class NasaCloseApproachService : INotifyPropertyChanged
    {
        // Not the best practice to use a static HttpClient ;D but for simplicity in this example
        private static readonly HttpClient _httpClient = new();
        private string NasaApiUrl = $"https://ssd-api.jpl.nasa.gov/cad.api?date-min={DateTime.Now:yyyy-MM-dd}&date-max={DateTime.Now.AddDays(7):yyyy-MM-dd}&dist-max=0.05&nea=true";

        public event PropertyChangedEventHandler? PropertyChanged;

        private NasaCloseApproachData Current
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public async IAsyncEnumerable<NasaCloseApproachData> GetCurrent(
            [EnumeratorCancellation] CancellationToken ct)
        {
            // TODO: Implement IAsyncEnumerable for server sent events
        }

        public async Task RefreshCloseApproachDataAsync(CancellationToken cancellationToken)
        {
            var nasaCloseApproachData = await FetchDataFromNasaAsync(cancellationToken);
            if (nasaCloseApproachData is null)
            {
                return;
            }

            Current = nasaCloseApproachData;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task<NasaCloseApproachData?> FetchDataFromNasaAsync(CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(NasaApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<NasaCloseApproachData>(jsonString);
        }
    }
}
