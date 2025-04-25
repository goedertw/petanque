using Microsoft.AspNetCore.Mvc;

using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/seizoenen")]
public class SeizoensController : ControllerBase
{
    private readonly ISeizoensService _seizoensService;

    public SeizoensController(ISeizoensService seizoensService)
    {
        _seizoensService = seizoensService;
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var seizoenen = _seizoensService.GetAll();
            return Ok(seizoenen);
        }
        catch
        {
            return StatusCode(500, "Er ging iets mis");
        }
    }

}