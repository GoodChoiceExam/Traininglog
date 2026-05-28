using System.Security.Claims;
using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitLife.TrainingLog.Api.Controllers;

// Eksponerer endpoints til træningsprogrammer og aktivitetslogning via HTTP.
// MemberId hentes altid fra JWT-tokenet så et medlem kun kan se og redigere egne data.
[ApiController]
[Route("workout")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _service;
    private readonly ILogger<WorkoutsController> _logger;

    public WorkoutsController(IWorkoutService service, ILogger<WorkoutsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // Henter member-id fra JWT-tokenets NameIdentifier-claim og kaster exception hvis det mangler
    private Guid GetMemberId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

    [HttpPost("programs")]
    public async Task<IActionResult> CreateProgram(CreateWorkoutProgramRequest request)
    {
        var result = await _service.CreateProgramAsync(GetMemberId(), request);
        _logger.LogInformation("Created workout program {ProgramId}", result.Id);
        return CreatedAtAction(nameof(GetProgram), new { id = result.Id }, ToResponse(result));
    }

    [HttpGet("programs")]
    public async Task<IActionResult> GetPrograms()
    {
        _logger.LogInformation("Fetching workout programs for member");
        var result = await _service.GetProgramsAsync(GetMemberId());
        return Ok(result.Select(ToResponse).ToList());
    }

    [HttpGet("programs/{id:guid}")]
    public async Task<IActionResult> GetProgram(Guid id)
    {
        _logger.LogInformation("Fetching workout program {ProgramId}", id);
        var result = await _service.GetProgramByIdAsync(GetMemberId(), id);
        if (result is null)
        {
            _logger.LogWarning("Workout program {ProgramId} was not found", id);
            return NotFound();
        }

        return Ok(ToResponse(result));
    }

    [HttpPost("programs/{id:guid}/exercises")]
    public async Task<IActionResult> AddExercise(Guid id, AddExerciseRequest request)
    {
        var result = await _service.AddExerciseAsync(GetMemberId(), id, request);
        _logger.LogInformation("Added exercise to workout program {ProgramId}", id);
        return Ok(ToResponse(result));
    }

    [HttpDelete("programs/{programId:guid}/exercises/{exerciseId:guid}")]
    public async Task<IActionResult> DeleteExercise(Guid programId, Guid exerciseId)
    {
        await _service.DeleteExerciseAsync(GetMemberId(), programId, exerciseId);
        _logger.LogInformation("Deleted exercise {ExerciseId} from workout program {ProgramId}", exerciseId, programId);
        return NoContent();
    }

    [HttpPut("programs/{id:guid}")]
    public async Task<IActionResult> UpdateProgramName(Guid id, [FromBody] string newName)
    {
        var result = await _service.UpdateProgramNameAsync(GetMemberId(), id, newName);
        if (result is null)
        {
            _logger.LogWarning("Cannot update workout program {ProgramId}; it was not found", id);
            return NotFound();
        }

        _logger.LogInformation("Updated name of workout program {ProgramId}", id);
        return Ok(ToResponse(result));
    }

    [HttpPut("programs/{programId:guid}/exercises/{exerciseId:guid}")]
    public async Task<IActionResult> UpdateExercise(Guid programId, Guid exerciseId, AddExerciseRequest request)
    {
        var result = await _service.UpdateExerciseAsync(GetMemberId(), programId, exerciseId, request.Name, request.Sets, request.Reps, request.WeightKg);
        if (result is null)
        {
            _logger.LogWarning("Cannot update exercise {ExerciseId} in workout program {ProgramId}; not found", exerciseId, programId);
            return NotFound();
        }

        _logger.LogInformation("Updated exercise {ExerciseId} in workout program {ProgramId}", exerciseId, programId);
        return Ok(ToResponse(result));
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

    private static WorkoutProgramResponse ToResponse(WorkoutProgram p) => new(
        p.Id,
        p.Name,
        p.Exercises.Select(e => new ExerciseResponse(e.Id, e.Name, e.Sets, e.Reps, e.WeightKg)).ToList()
    );
}