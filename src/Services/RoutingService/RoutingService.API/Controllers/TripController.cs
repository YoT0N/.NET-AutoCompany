using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.DTOs;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ILogger<TripController> _logger;

        public TripController(ITripService tripService, ILogger<TripController> logger)
        {
            _tripService = tripService;
            _logger = logger;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips()
        {
            _logger.LogInformation("Getting all trips");
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<TripDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDto<TripDetailsDto>>> GetTrips(
            [FromQuery] TripFilterParameters parameters)
        {
            _logger.LogInformation(
                "Getting trips page {Page} with page size {PageSize}",
                parameters.Page,
                parameters.PageSize);

            var result = await _tripService.GetTripsPagedAsync(parameters);

            Response.Headers.Add("X-Pagination-Page", result.Page.ToString());
            Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
            Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
            Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TripDto>> GetTripById(int id)
        {
            _logger.LogInformation("Getting trip with ID {TripId}", id);
            var trip = await _tripService.GetTripByIdAsync(id);
            return Ok(trip);
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(TripDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TripDetailsDto>> GetTripDetails(int id)
        {
            _logger.LogInformation("Getting trip details for ID {TripId}", id);
            var trip = await _tripService.GetTripDetailsAsync(id);
            return Ok(trip);
        }

        [HttpGet("route-sheet/{sheetId}")]
        [ProducesResponseType(typeof(IEnumerable<TripDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetTripsByRouteSheet(
            int sheetId)
        {
            _logger.LogInformation("Getting trips for route sheet {SheetId}", sheetId);
            var trips = await _tripService.GetTripsByRouteSheetAsync(sheetId);
            return Ok(trips);
        }

        [HttpGet("completed")]
        [ProducesResponseType(typeof(IEnumerable<TripDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetCompletedTrips()
        {
            _logger.LogInformation("Getting completed trips");
            var trips = await _tripService.GetCompletedTripsAsync();
            return Ok(trips);
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<TripDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetPendingTrips()
        {
            _logger.LogInformation("Getting pending trips");
            var trips = await _tripService.GetPendingTripsAsync();
            return Ok(trips);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto dto)
        {
            _logger.LogInformation(
                "Creating new trip for route sheet {SheetId}",
                dto.SheetId);

            var trip = await _tripService.CreateTripAsync(dto);

            _logger.LogInformation("Trip created successfully with ID {TripId}", trip.TripId);

            return CreatedAtAction(
                nameof(GetTripById),
                new { id = trip.TripId },
                trip);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TripDto>> UpdateTrip(
            int id,
            [FromBody] UpdateTripDto dto)
        {
            _logger.LogInformation("Updating trip with ID {TripId}", id);

            var trip = await _tripService.UpdateTripAsync(id, dto);

            _logger.LogInformation("Trip {TripId} updated successfully", id);

            return Ok(trip);
        }

        [HttpPatch("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkTripAsCompleted(int id)
        {
            _logger.LogInformation("Marking trip {TripId} as completed", id);

            await _tripService.MarkTripAsCompletedAsync(id);

            _logger.LogInformation("Trip {TripId} marked as completed", id);

            return Ok(new { message = "Trip marked as completed successfully" });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            _logger.LogInformation("Deleting trip with ID {TripId}", id);

            await _tripService.DeleteTripAsync(id);

            _logger.LogInformation("Trip {TripId} deleted successfully", id);

            return NoContent();
        }
    }
}