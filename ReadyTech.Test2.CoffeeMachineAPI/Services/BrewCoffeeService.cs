using ReadyTech.Test2.CoffeeMachineAPI.Constants;
using ReadyTech.Test2.CoffeeMachineAPI.Models;

namespace ReadyTech.Test2.CoffeeMachineAPI.Services
{
    public class BrewCoffeeService : IBrewCoffeeService
    {
        private readonly IBrewCoffeeCounter _callCounter;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IWeatherService _weatherService;
        private readonly double _latitude;
        private readonly double _longitude;

        public BrewCoffeeService(IBrewCoffeeCounter callCounter,
                                    IDateTimeProvider dateTimeProvider,
                                    IWeatherService weatherService,
                                    IConfiguration config)
        {
            _callCounter = callCounter;
            _dateTimeProvider = dateTimeProvider;
            _weatherService = weatherService;
            _latitude = config.GetValue<double>("Weather:Latitude");
            _longitude = config.GetValue<double>("Weather:Longitude");
        }

        public async Task<CoffeeResponse> BrewCoffeeAsync()
        {
            // Obtain the current date/time.
            DateTimeOffset now = _dateTimeProvider.Now;

            // Requirement #3: On April 1, always return 418 (I'm a teapot).
            if (now.Month == 4 && now.Day == 1)
                return new CoffeeResponse { StatusCode = 418 };

            // Increment the call counter.
            int currentCall = _callCounter.IncrementAndGet();

            // Requirement #2: Every fifth call returns 503 Service Unavailable.
            if (currentCall % 5 == 0)
                return new CoffeeResponse { StatusCode = 503 };

            // Check the weather asynchronously (using a sample location).
            double temperature = await _weatherService.GetCurrentTemperatureAsync(-36.8485, 174.7633);

            // Determine the message based on temperature.
            string message = temperature > 30
                ? CoffeeMessages.IcedCoffee
                : CoffeeMessages.HotCoffee;

            // Return the successful response.
            return new CoffeeResponse
            {
                StatusCode = 200,
                Message = message,
                Prepared = $"{now:yyyy-MM-ddTHH:mm:ss}{now.ToString("zzz").Replace(":", "")}"
            };
        }
    }
}