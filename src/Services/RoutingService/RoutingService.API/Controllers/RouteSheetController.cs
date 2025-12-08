using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.DTOs;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RouteSheetController : ControllerBase
    {
        private readonly IRouteSheetService _routeSheetService;
        private readonly ILogger<RouteSheetController> _logger;

        public RouteSheetController(
            IRouteSheetService routeSheetService,
            ILogger<RouteSheetController> logger)
        {
            _routeSheetService = routeSheetService;
            _logger = logger;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RouteSheetDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteSheetDto>>> GetAllRouteSheets()
        {
            _logger.LogInformation("Getting all route sheets");
            var sheets = await _routeSheetService.GetAllRouteSheetsAsync();
            return Ok(sheets);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<RouteSheetDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDto<RouteSheetDetailsDto>>> GetRouteSheets(
            [FromQuery] RouteSheetFilterParameters parameters)
        {
            _logger.LogInformation(
                "Getting route sheets page {Page} with page size {PageSize}",
                parameters.Page,
                parameters.PageSize);

            var result = await _routeSheetService.GetRouteSheetsPagedAsync(parameters);

            Response.Headers.Add("X-Pagination-Page", result.Page.ToString());
            Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
            Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
            Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RouteSheetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteSheetDto>> GetRouteSheetById(int id)
        {
            _logger.LogInformation("Getting route sheet with ID {SheetId}", id);
            var sheet = await _routeSheetService.GetRouteSheetByIdAsync(id);
            return Ok(sheet);
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(RouteSheetDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteSheetDetailsDto>> GetRouteSheetDetails(int id)
        {
            _logger.LogInformation("Getting route sheet details for ID {SheetId}", id);
            var sheet = await _routeSheetService.GetRouteSheetDetailsAsync(id);
            return Ok(sheet);
        }

        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(IEnumerable<RouteSheetDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByDate(
            DateTime date)
        {
            _logger.LogInformation("Getting route sheets for date {Date}", date.ToShortDateString());
            var sheets = await _routeSheetService.GetRouteSheetsByDateAsync(date);
            return Ok(sheets);
        }

        [HttpGet("route/{routeId}")]
        [ProducesResponseType(typeof(IEnumerable<RouteSheetDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByRoute(
            int routeId)
        {
            _logger.LogInformation("Getting route sheets for route {RouteId}", routeId);
            var sheets = await _routeSheetService.GetRouteSheetsByRouteAsync(routeId);
            return Ok(sheets);
        }

        [HttpGet("bus/{busId}")]
        [ProducesResponseType(typeof(IEnumerable<RouteSheetDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByBus(
            int busId)
        {
            _logger.LogInformation("Getting route sheets for bus {BusId}", busId);
            var sheets = await _routeSheetService.GetRouteSheetsByBusAsync(busId);
            return Ok(sheets);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RouteSheetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RouteSheetDto>> CreateRouteSheet(
            [FromBody] CreateRouteSheetDto dto)
        {
            _logger.LogInformation(
                "Creating new route sheet for route {RouteId}, bus {BusId}, date {Date}",
                dto.RouteId,
                dto.BusId,
                dto.SheetDate.ToShortDateString());

            var sheet = await _routeSheetService.CreateRouteSheetAsync(dto);

            _logger.LogInformation("Route sheet created successfully with ID {SheetId}", sheet.SheetId);

            return CreatedAtAction(
                nameof(GetRouteSheetById),
                new { id = sheet.SheetId },
                sheet);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RouteSheetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RouteSheetDto>> UpdateRouteSheet(
            int id,
            [FromBody] UpdateRouteSheetDto dto)
        {
            _logger.LogInformation("Updating route sheet with ID {SheetId}", id);

            var sheet = await _routeSheetService.UpdateRouteSheetAsync(id, dto);

            _logger.LogInformation("Route sheet {SheetId} updated successfully", id);

            return Ok(sheet);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRouteSheet(int id)
        {
            _logger.LogInformation("Deleting route sheet with ID {SheetId}", id);

            await _routeSheetService.DeleteRouteSheetAsync(id);

            _logger.LogInformation("Route sheet {SheetId} deleted successfully", id);

            return NoContent();
        }
    }
}