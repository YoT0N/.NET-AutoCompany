using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteStopController : ControllerBase
    {
        private readonly IRouteStopService _routeStopService;

        public RouteStopController(IRouteStopService routeStopService)
        {
            _routeStopService = routeStopService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteStopDto>>> GetAllStops()
        {
            var stops = await _routeStopService.GetAllStopsAsync();
            return Ok(stops);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteStopDto>> GetStopById(int id)
        {
            var stop = await _routeStopService.GetStopByIdAsync(id);
            if (stop == null)
                return NotFound($"RouteStop with ID {id} not found");

            return Ok(stop);
        }

        [HttpGet("route/{routeId}")]
        public async Task<ActionResult<IEnumerable<RouteStopInfoDto>>> GetStopsByRoute(int routeId)
        {
            var stops = await _routeStopService.GetStopsByRouteAsync(routeId);
            return Ok(stops);
        }

        [HttpPost]
        public async Task<ActionResult<RouteStopDto>> CreateStop([FromBody] CreateRouteStopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stop = await _routeStopService.CreateStopAsync(dto);
            return CreatedAtAction(nameof(GetStopById), new { id = stop.StopId }, stop);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RouteStopDto>> UpdateStop(int id, [FromBody] UpdateRouteStopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stop = await _routeStopService.UpdateStopAsync(id, dto);
            if (stop == null)
                return NotFound($"RouteStop with ID {id} not found");

            return Ok(stop);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStop(int id)
        {
            var result = await _routeStopService.DeleteStopAsync(id);
            if (!result)
                return NotFound($"RouteStop with ID {id} not found");

            return NoContent();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignStopToRoute([FromBody] AssignStopToRouteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _routeStopService.AssignStopToRouteAsync(dto);
            if (!result)
                return BadRequest("Failed to assign stop to route. Check if route and stop exist.");

            return Ok("Stop successfully assigned to route");
        }

        [HttpDelete("route/{routeId}/stop/{stopId}")]
        public async Task<IActionResult> RemoveStopFromRoute(int routeId, int stopId)
        {
            var result = await _routeStopService.RemoveStopFromRouteAsync(routeId, stopId);
            if (!result)
                return NotFound($"Assignment between Route {routeId} and Stop {stopId} not found");

            return NoContent();
        }
    }
}