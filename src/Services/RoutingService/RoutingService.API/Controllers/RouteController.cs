using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RouteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
        {
            _logger.LogInformation("Getting all routes");
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<RouteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDto<RouteDto>>> GetRoutes(
            [FromQuery] RouteFilterParameters parameters)
        {
            _logger.LogInformation(
                "Getting routes page {Page} with page size {PageSize}",
                parameters.Page,
                parameters.PageSize);

            var result = await _routeService.GetRoutesPagedAsync(parameters);

            // Add pagination metadata to response headers
            Response.Headers.Add("X-Pagination-Page", result.Page.ToString());
            Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
            Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
            Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteDto>> GetRouteById(int id)
        {
            _logger.LogInformation("Getting route with ID {RouteId}", id);
            var route = await _routeService.GetRouteByIdAsync(id);
            return Ok(route);
        }

        [HttpGet("{id}/with-stops")]
        [ProducesResponseType(typeof(RouteWithStopsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteWithStopsDto>> GetRouteWithStops(int id)
        {
            _logger.LogInformation("Getting route {RouteId} with stops", id);
            var route = await _routeService.GetRouteWithStopsAsync(id);
            return Ok(route);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RouteDto>> CreateRoute([FromBody] CreateRouteDto dto)
        {
            _logger.LogInformation("Creating new route: {RouteNumber}", dto.RouteNumber);

            var route = await _routeService.CreateRouteAsync(dto);

            _logger.LogInformation("Route created successfully with ID {RouteId}", route.RouteId);

            return CreatedAtAction(
                nameof(GetRouteById),
                new { id = route.RouteId },
                route);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RouteDto>> UpdateRoute(
            int id,
            [FromBody] UpdateRouteDto dto)
        {
            _logger.LogInformation("Updating route with ID {RouteId}", id);

            var route = await _routeService.UpdateRouteAsync(id, dto);

            _logger.LogInformation("Route {RouteId} updated successfully", id);

            return Ok(route);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            _logger.LogInformation("Deleting route with ID {RouteId}", id);

            await _routeService.DeleteRouteAsync(id);

            _logger.LogInformation("Route {RouteId} deleted successfully", id);

            return NoContent();
        }

        [HttpHead("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RouteExists(int id)
        {
            var exists = await _routeService.RouteExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}