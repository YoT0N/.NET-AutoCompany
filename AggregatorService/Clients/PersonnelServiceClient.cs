using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AggregatorService.Clients;

public class PersonnelServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PersonnelServiceClient> _logger;

    public PersonnelServiceClient(HttpClient httpClient, ILogger<PersonnelServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<WorkShiftDto>?> GetWorkShiftsByPersonnelAsync(int personnelId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling PersonnelService for work shifts of personnel {PersonnelId}", personnelId);

            var response = await _httpClient.GetAsync($"/api/workshifts/personnel/{personnelId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PersonnelService returned {StatusCode} for personnel {PersonnelId}",
                    response.StatusCode, personnelId);
                return new List<WorkShiftDto>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var workShifts = JsonSerializer.Deserialize<List<WorkShiftDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully retrieved {Count} work shifts for personnel {PersonnelId}",
                workShifts?.Count ?? 0, personnelId);

            return workShifts ?? new List<WorkShiftDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PersonnelService for personnel {PersonnelId}", personnelId);
            return new List<WorkShiftDto>();
        }
    }

    public async Task<PersonnelDto?> GetPersonnelByIdAsync(int personnelId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling PersonnelService for personnel {PersonnelId}", personnelId);

            // PersonnelService використовує MongoDB ObjectId як string, але також має PersonnelId як int
            // Спробуємо отримати через GetAll та відфільтрувати
            var response = await _httpClient.GetAsync("/api/personnel", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PersonnelService returned {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var allPersonnel = JsonSerializer.Deserialize<List<PersonnelDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var personnel = allPersonnel?.FirstOrDefault(p => p.PersonnelId == personnelId);

            if (personnel != null)
            {
                _logger.LogInformation("Successfully retrieved personnel {PersonnelId}", personnelId);
            }

            return personnel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PersonnelService for personnel {PersonnelId}", personnelId);
            return null;
        }
    }
}

public class WorkShiftDto
{
    public string Id { get; set; } = string.Empty;
    public int PersonnelId { get; set; }
    public DateTime ShiftDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public string RouteNumber { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PersonnelDto
{
    public string Id { get; set; } = string.Empty;
    public int PersonnelId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}