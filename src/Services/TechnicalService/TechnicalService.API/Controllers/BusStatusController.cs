using Microsoft.AspNetCore.Mvc;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusStatusController : ControllerBase
{
    [HttpGet]
    public ActionResult GetStatuses()
    {
        var statuses = new[]
        {
            new { StatusId = 1, StatusName = "Active", Description = "Bus is operational and in service" },
            new { StatusId = 2, StatusName = "Under Maintenance", Description = "Bus is currently being maintained" },
            new { StatusId = 3, StatusName = "Under Repair", Description = "Bus is being repaired" },
            new { StatusId = 4, StatusName = "Out of Service", Description = "Bus is not in service" },
            new { StatusId = 5, StatusName = "Written Off", Description = "Bus has been written off" }
        };

        return Ok(statuses);
    }
}