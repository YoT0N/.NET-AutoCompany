using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetTripById(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound($"Trip with ID {id} not found");

            return Ok(trip);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<TripDetailsDto>> GetTripDetails(int id)
        {
            var trip = await _tripService.GetTripDetailsAsync(id);
            if (trip == null)
                return NotFound($"Trip with ID {id} not found");

            return Ok(trip);
        }

        [HttpGet("route-sheet/{sheetId}")]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetTripsByRouteSheet(int sheetId)
        {
            var trips = await _tripService.GetTripsByRouteSheetAsync(sheetId);
            return Ok(trips);
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetCompletedTrips()
        {
            var trips = await _tripService.GetCompletedTripsAsync();
            return Ok(trips);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TripDetailsDto>>> GetPendingTrips()
        {
            var trips = await _tripService.GetPendingTripsAsync();
            return Ok(trips);
        }

        [HttpPost]
        public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var trip = await _tripService.CreateTripAsync(dto);
                return CreatedAtAction(nameof(GetTripById), new { id = trip.TripId }, trip);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TripDto>> UpdateTrip(int id, [FromBody] UpdateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripService.UpdateTripAsync(id, dto);
            if (trip == null)
                return NotFound($"Trip with ID {id} not found");

            return Ok(trip);
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> MarkTripAsCompleted(int id)
        {
            var result = await _tripService.MarkTripAsCompletedAsync(id);
            if (!result)
                return NotFound($"Trip with ID {id} not found");

            return Ok("Trip marked as completed");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var result = await _tripService.DeleteTripAsync(id);
            if (!result)
                return NotFound($"Trip with ID {id} not found");

            return NoContent();
        }
    }
}