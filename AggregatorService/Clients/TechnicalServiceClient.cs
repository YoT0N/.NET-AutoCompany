using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AggregatorService.Clients;

public class TechnicalServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TechnicalServiceClient> _logger;

    public TechnicalServiceClient(HttpClient httpClient, ILogger<TechnicalServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BusDto?> GetBusAsync(string countryNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling TechnicalService for bus {CountryNumber}", countryNumber);

            var response = await _httpClient.GetAsync($"/api/bus/{countryNumber}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("TechnicalService returned {StatusCode} for bus {CountryNumber}",
                    response.StatusCode, countryNumber);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var bus = JsonSerializer.Deserialize<BusDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully retrieved bus {CountryNumber} from TechnicalService", countryNumber);

            return bus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TechnicalService for bus {CountryNumber}", countryNumber);
            return null;
        }
    }
}

public class BusDto
{
    public string CountryNumber { get; set; } = string.Empty;
    public string BoardingNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int PassengerCapacity { get; set; }
    public int YearOfManufacture { get; set; }
    public decimal Mileage { get; set; }
    public DateTime DateOfReceipt { get; set; }
    public DateTime? WriteoffDate { get; set; }
    public int CurrentStatusId { get; set; }
    public string? CurrentStatusName { get; set; }
}