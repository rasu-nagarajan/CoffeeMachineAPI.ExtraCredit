using System.Threading.Tasks;

namespace ReadyTech.Test2.CoffeeMachineAPI.Services
{
    public interface IWeatherService
    {
        /// Gets the current temperature (in Celsius) for the given latitude and longitude.
        /// <param name="latitude">Latitude value.</param>
        /// <param name="longitude">Longitude value.</param>
        /// <returns>Temperature in Celsius.</returns>
        Task<double> GetCurrentTemperatureAsync(double latitude, double longitude);
    }
}
