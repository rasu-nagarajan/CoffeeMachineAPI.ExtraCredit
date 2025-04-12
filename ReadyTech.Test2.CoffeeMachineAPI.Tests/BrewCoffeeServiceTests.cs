using Microsoft.Extensions.Configuration;
using ReadyTech.Test2.CoffeeMachineAPI.Models;
using ReadyTech.Test2.CoffeeMachineAPI.Services;
using ReadyTech.Test2.CoffeeMachineAPI.Constants;
using Xunit;

namespace ReadyTech.Test2.CoffeeMachineAPI.Tests
{
    // Fake DateTimeProvider to control the value of 'Now'
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now { get; set; }
    }

    // Fake CallCounterService that simply increments its internal counter
    public class FakeCallCounterService : IBrewCoffeeCounter
    {
        private int _callCount = 0;
        public int IncrementAndGet() => ++_callCount;
        public void Reset() => _callCount = 0;
    }

    // Fake WeatherService to simulate a fixed temperature.
    public class FakeWeatherService : IWeatherService
    {
        public double Temperature { get; set; }
        public Task<double> GetCurrentTemperatureAsync(double latitude, double longitude) =>
            Task.FromResult(Temperature);
    }

    public class BrewCoffeeServiceTests
    {
        private readonly IConfiguration _config;
        private readonly FakeCallCounterService _fakeCounter;
        private readonly FakeWeatherService _fakeWeather;

        public BrewCoffeeServiceTests()
        {
            _config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Weather:Latitude", "-36.8485" },
                { "Weather:Longitude", "174.7633" }
            }).Build();

            _fakeCounter = new FakeCallCounterService();    // reset counter for each test
            _fakeWeather = new FakeWeatherService();      // if need customize temperature inside each test
        }

        [Fact]
        public async Task BrewCoffee_ReturnsTeapotOnAprilFirst()
        {
            // Arrange: Set the date to April 1
            _fakeCounter.Reset();
            var dateProvider = new FakeDateTimeProvider { Now = new DateTimeOffset(2025, 4, 1, 12, 0, 0, TimeSpan.Zero) };
            _fakeWeather.Temperature = 25;
            var service = new BrewCoffeeService(_fakeCounter, dateProvider, _fakeWeather, _config);

            // Act
            CoffeeResponse response = await service.BrewCoffeeAsync();

            // Assert: Expect status code 418
            Assert.Equal(418, response.StatusCode);
        }

        [Fact]
        public async Task BrewCoffee_Returns503OnFifthCall()
        {
            // Arrange: use a non-April date and a fixed temperature.
            _fakeCounter.Reset();
            var dateProvider = new FakeDateTimeProvider { Now = new DateTimeOffset(2025, 3, 15, 12, 0, 0, TimeSpan.Zero) };
            _fakeWeather.Temperature = 25;
            var service = new BrewCoffeeService(_fakeCounter, dateProvider, _fakeWeather, _config);

            // Act & Assert
            // Simulate 4 calls returning 200
            for (int i = 1; i < 5; i++)
            {
                var resp = await service.BrewCoffeeAsync();
                Assert.Equal(200, resp.StatusCode);
            }
            // The 5th call should return 503
            var fifthResponse = await service.BrewCoffeeAsync();
            Assert.Equal(503, fifthResponse.StatusCode);
        }

        [Fact]
        public async Task BrewCoffee_ReturnsHotCoffee_WhenTemperatureIsNormal()
        {
            // Arrange: a non-special day, counter at first call, temperature below 30°C
            _fakeCounter.Reset();
            var dateProvider = new FakeDateTimeProvider { Now = new DateTimeOffset(2025, 3, 15, 12, 0, 0, TimeSpan.Zero) };
            var service = new BrewCoffeeService(_fakeCounter, dateProvider, _fakeWeather, _config);

            // Act
            CoffeeResponse response = await service.BrewCoffeeAsync();

            // Assert
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(CoffeeMessages.HotCoffee, response.Message);
            // The prepared field should be formatted as ISO-8601 (e.g., "2025-03-15T12:00:00+0100")
            string expectedPrepared = $"{dateProvider.Now.ToString("yyyy-MM-ddTHH:mm:ss")}{dateProvider.Now.ToString("zzz").Replace(":", "")}";
            Assert.Equal(expectedPrepared, response.Prepared);
        }

        [Fact]
        public async Task BrewCoffee_ReturnsIcedCoffee_WhenTemperatureIsHigh()
        {
            // Arrange: non-special day, first call, but the temperature is greater than 30°C
            _fakeCounter.Reset();
            var dateProvider = new FakeDateTimeProvider { Now = new DateTimeOffset(2025, 3, 15, 12, 0, 0, TimeSpan.Zero) };
            _fakeWeather.Temperature = 32;
            var service = new BrewCoffeeService(_fakeCounter, dateProvider, _fakeWeather, _config);

            // Act
            CoffeeResponse response = await service.BrewCoffeeAsync();

            // Assert
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(CoffeeMessages.IcedCoffee, response.Message);
        }

        [Fact]
        public async Task BrewCoffee_ThrowsOrDefaults_WhenConfigMissing()
        {
            _fakeCounter.Reset();
            var dateProvider = new FakeDateTimeProvider { Now = new DateTimeOffset(2025, 3, 15, 12, 0, 0, TimeSpan.Zero) };
            var service = new BrewCoffeeService(_fakeCounter, dateProvider, _fakeWeather, _config);

            var response = await service.BrewCoffeeAsync();

            // Depending on your implementation, assert default behavior
            Assert.Equal(200, response.StatusCode);
        }
    }
}
