using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetRouteById(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound($"Route with ID {id} not found");

            return Ok(route);
        }

        [HttpGet("{id}/with-stops")]
        public async Task<ActionResult<RouteWithStopsDto>> GetRouteWithStops(int id)
        {
            var route = await _routeService.GetRouteWithStopsAsync(id);
            if (route == null)
                return NotFound($"Route with ID {id} not found");

            return Ok(route);
        }

        [HttpPost]
        public async Task<ActionResult<RouteDto>> CreateRoute([FromBody] CreateRouteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = await _routeService.CreateRouteAsync(dto);
            return CreatedAtAction(nameof(GetRouteById), new { id = route.RouteId }, route);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RouteDto>> UpdateRoute(int id, [FromBody] UpdateRouteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = await _routeService.UpdateRouteAsync(id, dto);
            if (route == null)
                return NotFound($"Route with ID {id} not found");

            return Ok(route);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var result = await _routeService.DeleteRouteAsync(id);
            if (!result)
                return NotFound($"Route with ID {id} not found");

            return NoContent();
        }
    }
}