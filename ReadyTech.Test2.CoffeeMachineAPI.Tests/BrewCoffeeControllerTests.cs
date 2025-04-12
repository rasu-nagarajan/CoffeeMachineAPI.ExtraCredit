using ReadyTech.Test2.CoffeeMachineAPI.Controllers;
using ReadyTech.Test2.CoffeeMachineAPI.Models;
using ReadyTech.Test2.CoffeeMachineAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ReadyTech.Test2.CoffeeMachineAPI.Tests
{
    // A fake IBrewCoffeeService implementation for controller tests.
    public class FakeBrewCoffeeService : IBrewCoffeeService
    {
        private readonly CoffeeResponse _response;
        public FakeBrewCoffeeService(CoffeeResponse response) => _response = response;

        public Task<CoffeeResponse> BrewCoffeeAsync() => Task.FromResult(_response);
    }

    public class BrewCoffeeControllerTests
    {
        [Fact]
        public async Task Controller_ReturnsTeapotWhenServiceReturns418()
        {
            // Arrange: Simulate a teapot scenario.
            var coffeeResponse = new CoffeeResponse { StatusCode = 418 };
            IBrewCoffeeService fakeService = new FakeBrewCoffeeService(coffeeResponse);
            var controller = new BrewCoffeeController(fakeService);

            // Act
            IActionResult result = await controller.BrewCoffee();

            // Assert: Verify a StatusCodeResult with 418.
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(418, statusResult.StatusCode);
        }

        [Fact]
        public async Task Controller_ReturnsServiceUnavailableWhenServiceReturns503()
        {
            // Arrange: Simulate a 503 scenario.
            var coffeeResponse = new CoffeeResponse { StatusCode = 503 };
            IBrewCoffeeService fakeService = new FakeBrewCoffeeService(coffeeResponse);
            var controller = new BrewCoffeeController(fakeService);

            // Act
            IActionResult result = await controller.BrewCoffee();

            // Assert: A 503 response is produced.
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, statusResult.StatusCode);
        }
    }
}
