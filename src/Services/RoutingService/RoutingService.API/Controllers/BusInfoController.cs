using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusInfoController : ControllerBase
    {
        private readonly IBusInfoService _busInfoService;

        public BusInfoController(IBusInfoService busInfoService)
        {
            _busInfoService = busInfoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusInfoDto>>> GetAllBuses()
        {
            var buses = await _busInfoService.GetAllBusesAsync();
            return Ok(buses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusInfoDto>> GetBusById(int id)
        {
            var bus = await _busInfoService.GetBusByIdAsync(id);
            if (bus == null)
                return NotFound($"Bus with ID {id} not found");

            return Ok(bus);
        }

        [HttpGet("country-number/{countryNumber}")]
        public async Task<ActionResult<BusInfoDto>> GetBusByCountryNumber(string countryNumber)
        {
            var bus = await _busInfoService.GetBusByCountryNumberAsync(countryNumber);
            if (bus == null)
                return NotFound($"Bus with country number {countryNumber} not found");

            return Ok(bus);
        }

        [HttpPost]
        public async Task<ActionResult<BusInfoDto>> CreateBus([FromBody] CreateBusInfoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bus = await _busInfoService.CreateBusAsync(dto);
            return CreatedAtAction(nameof(GetBusById), new { id = bus.BusId }, bus);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BusInfoDto>> UpdateBus(int id, [FromBody] UpdateBusInfoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bus = await _busInfoService.UpdateBusAsync(id, dto);
            if (bus == null)
                return NotFound($"Bus with ID {id} not found");

            return Ok(bus);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var result = await _busInfoService.DeleteBusAsync(id);
            if (!result)
                return NotFound($"Bus with ID {id} not found");

            return NoContent();
        }
    }
}