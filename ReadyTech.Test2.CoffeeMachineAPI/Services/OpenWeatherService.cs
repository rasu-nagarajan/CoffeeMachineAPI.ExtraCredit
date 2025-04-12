using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReadyTech.Test2.CoffeeMachineAPI.Services
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // The API key should be stored in your configuration
            _apiKey = configuration["OpenWeatherApiKey"];
        }

        public async Task<double> GetCurrentTemperatureAsync(double latitude, double longitude)
        {
            // Construct the API endpoint URL
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            double temperature = document.RootElement
                                         .GetProperty("main")
                                         .GetProperty("temp")
                                         .GetDouble();

            return temperature;
        }
    }
}
