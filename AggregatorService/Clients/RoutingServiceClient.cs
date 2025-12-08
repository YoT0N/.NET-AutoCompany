using Grpc.Core;
using RoutingService.Grpc;

namespace AggregatorService.Clients;

public class RoutingGrpcClient
{
    private readonly RoutingGrpcService.RoutingGrpcServiceClient _client;
    private readonly ILogger<RoutingGrpcClient> _logger;

    public RoutingGrpcClient(
        RoutingGrpcService.RoutingGrpcServiceClient client,
        ILogger<RoutingGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<RouteResponse?> GetRouteByIdAsync(int routeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling gRPC RoutingService for route {RouteId}", routeId);

            var request = new GetRouteByIdRequest { RouteId = routeId };
            var response = await _client.GetRouteByIdAsync(request, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully retrieved route {RouteId} via gRPC", routeId);
            return response;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("Route {RouteId} not found", routeId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling gRPC RoutingService for route {RouteId}", routeId);
            return null;
        }
    }

    public async Task<List<RouteResponse>> GetAllRoutesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling gRPC RoutingService for all routes");

            var request = new GetAllRoutesRequest();
            var response = await _client.GetAllRoutesAsync(request, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} routes via gRPC", response.Routes.Count);
            return response.Routes.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling gRPC RoutingService for all routes");
            return new List<RouteResponse>();
        }
    }

    public async Task<List<RouteSheetResponse>> GetRouteSheetsByBusAsync(int busId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling gRPC RoutingService for route sheets of bus {BusId}", busId);

            var request = new GetRouteSheetsByBusRequest { BusId = busId };
            var response = await _client.GetRouteSheetsByBusAsync(request, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} route sheets for bus {BusId} via gRPC",
                response.RouteSheets.Count, busId);

            return response.RouteSheets.ToList();
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("No route sheets found for bus {BusId}", busId);
            return new List<RouteSheetResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling gRPC RoutingService for bus {BusId}", busId);
            return new List<RouteSheetResponse>();
        }
    }
}