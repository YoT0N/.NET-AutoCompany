using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteSheetController : ControllerBase
    {
        private readonly IRouteSheetService _routeSheetService;

        public RouteSheetController(IRouteSheetService routeSheetService)
        {
            _routeSheetService = routeSheetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteSheetDto>>> GetAllRouteSheets()
        {
            var sheets = await _routeSheetService.GetAllRouteSheetsAsync();
            return Ok(sheets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteSheetDto>> GetRouteSheetById(int id)
        {
            var sheet = await _routeSheetService.GetRouteSheetByIdAsync(id);
            if (sheet == null)
                return NotFound($"RouteSheet with ID {id} not found");

            return Ok(sheet);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<RouteSheetDetailsDto>> GetRouteSheetDetails(int id)
        {
            var sheet = await _routeSheetService.GetRouteSheetDetailsAsync(id);
            if (sheet == null)
                return NotFound($"RouteSheet with ID {id} not found");

            return Ok(sheet);
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByDate(DateTime date)
        {
            var sheets = await _routeSheetService.GetRouteSheetsByDateAsync(date);
            return Ok(sheets);
        }

        [HttpGet("route/{routeId}")]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByRoute(int routeId)
        {
            var sheets = await _routeSheetService.GetRouteSheetsByRouteAsync(routeId);
            return Ok(sheets);
        }

        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<IEnumerable<RouteSheetDetailsDto>>> GetRouteSheetsByBus(int busId)
        {
            var sheets = await _routeSheetService.GetRouteSheetsByBusAsync(busId);
            return Ok(sheets);
        }

        [HttpPost]
        public async Task<ActionResult<RouteSheetDto>> CreateRouteSheet([FromBody] CreateRouteSheetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sheet = await _routeSheetService.CreateRouteSheetAsync(dto);
                return CreatedAtAction(nameof(GetRouteSheetById), new { id = sheet.SheetId }, sheet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RouteSheetDto>> UpdateRouteSheet(int id, [FromBody] UpdateRouteSheetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sheet = await _routeSheetService.UpdateRouteSheetAsync(id, dto);
                if (sheet == null)
                    return NotFound($"RouteSheet with ID {id} not found");

                return Ok(sheet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRouteSheet(int id)
        {
            var result = await _routeSheetService.DeleteRouteSheetAsync(id);
            if (!result)
                return NotFound($"RouteSheet with ID {id} not found");

            return NoContent();
        }
    }
}