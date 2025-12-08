using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator Mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IActionResult HandleResult<T>(T result)
        {
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}