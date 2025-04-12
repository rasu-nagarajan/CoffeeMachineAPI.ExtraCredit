# Coffee Machine API

This project simulates an imaginary internet-connected coffee machine with HTTP endpoints. It is implemented in **.NET 6 Web API**, using clean architecture, dependency injection, and unit testing.

---

## Base Requirements

### Endpoint: `GET /brew-coffee`

Returns a hot coffee message and timestamp, with the following logic:

### Weather-Based Iced Coffee Behavior

1. **Normal Call**

- If the temperature (from OpenWeatherMap API: https://openweathermap.org/api) is **greater than 30°C**, the message changes to:
  ```json
  {
    "message": "Your refreshing iced coffee is ready",
    "prepared": "2025-03-15T12:00:00+0100"
  }
  ```

2. **Every 5th Call**

   - Returns: `503 Service Unavailable`
   - Body: _(empty)_
   - Reason: Simulates "out of coffee"

3. **April 1st (Fools’ Day)**
   - Returns: `418 I'm a teapot`
   - Body: _(empty)_
   - Reason: Coffee machine is joking

---

### Getting Started

#### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A valid API key for OpenWeatherMap (set in your configuration as `OpenWeatherApiKey`).

#### Build and Run

1. **Restore and Build:**

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

---
