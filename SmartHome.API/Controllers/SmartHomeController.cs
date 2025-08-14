using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Models;
using SmartHome.API.Services;

namespace SmartHome.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartHomeController : ControllerBase
{
    private readonly SmartHomeSimulationService _simulationService;

    public SmartHomeController(SmartHomeSimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    [HttpGet("state")]
    public ActionResult<SmartHomeState> GetState()
    {
        var state = _simulationService.GetCurrentState();
        return Ok(state);
    }
}
