using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.DTOs;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RouteStopController : ControllerBase
    {
        private readonly IRouteStopService _routeStopService;
        private readonly ILogger<RouteStopController> _logger;

        public RouteStopController(
            IRouteStopService routeStopService,
            ILogger<RouteStopController> logger)
        {
            _routeStopService = routeStopService;
            _logger = logger;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RouteStopDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteStopDto>>> GetAllStops()
        {
            _logger.LogInformation("Getting all stops");
            var stops = await _routeStopService.GetAllStopsAsync();
            return Ok(stops);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<RouteStopDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDto<RouteStopDto>>> GetStops(
            [FromQuery] RouteStopFilterParameters parameters)
        {
            _logger.LogInformation(
                "Getting stops page {Page} with page size {PageSize}",
                parameters.Page,
                parameters.PageSize);

            var result = await _routeStopService.GetStopsPagedAsync(parameters);

            Response.Headers.Add("X-Pagination-Page", result.Page.ToString());
            Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
            Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
            Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RouteStopDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteStopDto>> GetStopById(int id)
        {
            _logger.LogInformation("Getting stop with ID {StopId}", id);
            var stop = await _routeStopService.GetStopByIdAsync(id);
            return Ok(stop);
        }

        [HttpGet("route/{routeId}")]
        [ProducesResponseType(typeof(IEnumerable<RouteStopInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RouteStopInfoDto>>> GetStopsByRoute(int routeId)
        {
            _logger.LogInformation("Getting stops for route {RouteId}", routeId);
            var stops = await _routeStopService.GetStopsByRouteAsync(routeId);
            return Ok(stops);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RouteStopDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RouteStopDto>> CreateStop([FromBody] CreateRouteStopDto dto)
        {
            _logger.LogInformation("Creating new stop: {StopName}", dto.StopName);

            var stop = await _routeStopService.CreateStopAsync(dto);

            _logger.LogInformation("Stop created successfully with ID {StopId}", stop.StopId);

            return CreatedAtAction(
                nameof(GetStopById),
                new { id = stop.StopId },
                stop);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RouteStopDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteStopDto>> UpdateStop(
            int id,
            [FromBody] UpdateRouteStopDto dto)
        {
            _logger.LogInformation("Updating stop with ID {StopId}", id);

            var stop = await _routeStopService.UpdateStopAsync(id, dto);

            _logger.LogInformation("Stop {StopId} updated successfully", id);

            return Ok(stop);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteStop(int id)
        {
            _logger.LogInformation("Deleting stop with ID {StopId}", id);

            await _routeStopService.DeleteStopAsync(id);

            _logger.LogInformation("Stop {StopId} deleted successfully", id);

            return NoContent();
        }

        [HttpPost("assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AssignStopToRoute([FromBody] AssignStopToRouteDto dto)
        {
            _logger.LogInformation(
                "Assigning stop {StopId} to route {RouteId} with order {Order}",
                dto.StopId,
                dto.RouteId,
                dto.StopOrder);

            await _routeStopService.AssignStopToRouteAsync(dto);

            _logger.LogInformation(
                "Stop {StopId} successfully assigned to route {RouteId}",
                dto.StopId,
                dto.RouteId);

            return Ok(new { message = "Stop successfully assigned to route" });
        }

        [HttpDelete("route/{routeId}/stop/{stopId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStopFromRoute(int routeId, int stopId)
        {
            _logger.LogInformation(
                "Removing stop {StopId} from route {RouteId}",
                stopId,
                routeId);

            await _routeStopService.RemoveStopFromRouteAsync(routeId, stopId);

            _logger.LogInformation(
                "Stop {StopId} successfully removed from route {RouteId}",
                stopId,
                routeId);

            return NoContent();
        }
    }
}