using System.Security.Claims;
using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitLife.TrainingLog.Api.Controllers;

[ApiController]
[Route("workout")]
[Authorize]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutService _service;
    private readonly ILogger<WorkoutController> _logger;

    public WorkoutController(IWorkoutService service, ILogger<WorkoutController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private Guid GetMemberId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

    [HttpPost("programs")]
    public async Task<IActionResult> CreateProgram(CreateWorkoutProgramRequest request)
    {
        var result = await _service.CreateProgramAsync(GetMemberId(), request);
        return CreatedAtAction(nameof(GetProgram), new { id = result.Id }, result);
    }

    [HttpGet("programs")]
    public async Task<IActionResult> GetPrograms()
    {
        var result = await _service.GetProgramsAsync(GetMemberId());
        return Ok(result);
    }

    [HttpGet("programs/{id:guid}")]
    public async Task<IActionResult> GetProgram(Guid id)
    {
        var result = await _service.GetProgramByIdAsync(GetMemberId(), id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("programs/{id:guid}/exercises")]
    public async Task<IActionResult> AddExercise(Guid id, AddExerciseRequest request)
    {
        var result = await _service.AddExerciseAsync(GetMemberId(), id, request);
        return Ok(result);
    }

    [HttpDelete("programs/{programId:guid}/exercises/{exerciseId:guid}")]
    public async Task<IActionResult> DeleteExercise(Guid programId, Guid exerciseId)
    {
        await _service.DeleteExerciseAsync(GetMemberId(), programId, exerciseId);
        return NoContent();
    }
    
    [HttpPut("programs/{id:guid}")]
    public async Task<IActionResult> UpdateProgramName(Guid id, [FromBody] string newName)
    {
        var result = await _service.UpdateProgramNameAsync(GetMemberId(), id, newName);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpPut("programs/{programId:guid}/exercises/{exerciseId:guid}")]
    public async Task<IActionResult> UpdateExercise(Guid programId, Guid exerciseId, AddExerciseRequest request)
    {
        var result = await _service.UpdateExerciseAsync(GetMemberId(), programId, exerciseId, request.Name, request.Sets, request.Reps, request.WeightKg);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("activities")]
    public async Task<IActionResult> LogActivity(CreateWorkoutActivityRequest request)
    {
        var result = await _service.LogActivityAsync(GetMemberId(), request);
        return Ok(result);
    }

    [HttpGet("activities")]
    public async Task<IActionResult> GetActivities()
    {
        var result = await _service.GetActivitiesAsync(GetMemberId());
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("service", "FitLife TrainingLog API");
        var ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion;
        properties.Add("version", ver!);
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString();
            properties.Add("hosted-at-address", ipa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            properties.Add("hosted-at-address", "Could not resolve IP-address");
        }
        return properties;
    }
}