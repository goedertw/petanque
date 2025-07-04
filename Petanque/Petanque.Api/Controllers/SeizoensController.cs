using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
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
    
    [HttpPost]
    public IActionResult Create([FromBody] SeizoenRequestContract request)
    {
        try
        {
            var createdSeizoen = _seizoensService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = createdSeizoen.SeizoensId }, createdSeizoen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
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