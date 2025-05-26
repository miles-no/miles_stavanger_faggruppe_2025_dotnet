using SseWorkshop.External.NasaWeather;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SseWorkshop.Services
{
    public class NasaCloseApproachService : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new HttpClient();
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
            while (ct is not { IsCancellationRequested: true })
            {
                yield return Current;
                var tcs = new TaskCompletionSource();
                PropertyChangedEventHandler handler = (_, _) => tcs.SetResult();
                PropertyChanged += handler;
                try
                {
                    await tcs.Task.WaitAsync(ct);
                }
                finally
                {
                    PropertyChanged -= handler;
                }
            }
        }

        public async Task UpdateCloseApproachDataAsync(CancellationToken cancellationToken)
        {
            Current = await FetchWeatherFromNasaAsync(cancellationToken);
        }

        public async Task<NasaCloseApproachData?> FetchWeatherFromNasaAsync(CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(NasaApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<NasaCloseApproachData>(jsonString);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
