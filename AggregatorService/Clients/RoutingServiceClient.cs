using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AggregatorService.Clients;

public class RoutingServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RoutingServiceClient> _logger;

    public RoutingServiceClient(HttpClient httpClient, ILogger<RoutingServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<RouteSheetDto>?> GetRouteSheetsByBusAsync(string countryNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling RoutingService for route sheets with bus {CountryNumber}", countryNumber);

            // Спочатку знайдемо BusId за CountryNumber
            var busResponse = await _httpClient.GetAsync($"/api/businfo/country-number/{countryNumber}", cancellationToken);

            if (!busResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("RoutingService: Bus {CountryNumber} not found", countryNumber);
                return new List<RouteSheetDto>();
            }

            var busContent = await busResponse.Content.ReadAsStringAsync(cancellationToken);
            var busInfo = JsonSerializer.Deserialize<BusInfoDto>(busContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (busInfo == null)
            {
                return new List<RouteSheetDto>();
            }

            // Тепер отримаємо RouteSheets за BusId
            var response = await _httpClient.GetAsync($"/api/routesheet/bus/{busInfo.BusId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("RoutingService returned {StatusCode} for bus {CountryNumber}",
                    response.StatusCode, countryNumber);
                return new List<RouteSheetDto>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var routeSheets = JsonSerializer.Deserialize<List<RouteSheetDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully retrieved {Count} route sheets for bus {CountryNumber}",
                routeSheets?.Count ?? 0, countryNumber);

            return routeSheets ?? new List<RouteSheetDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling RoutingService for bus {CountryNumber}", countryNumber);
            return new List<RouteSheetDto>();
        }
    }
}

public class BusInfoDto
{
    public int BusId { get; set; }
    public string CountryNumber { get; set; } = string.Empty;
}

public class RouteSheetDto
{
    public int SheetId { get; set; }
    public int RouteId { get; set; }
    public int BusId { get; set; }
    public DateTime SheetDate { get; set; }
    public string RouteNumber { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string BusCountryNumber { get; set; } = string.Empty;
}