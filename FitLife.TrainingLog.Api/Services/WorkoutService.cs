using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Repositories;

namespace FitLife.TrainingLog.Api.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkoutProgramResponse> CreateProgramAsync(Guid memberId, CreateWorkoutProgramRequest request)
    {
        var program = new WorkoutProgram
        {
            MemberId = memberId,
            Name = request.Name
        };

        var created = await _repository.CreateProgramAsync(program);
        return ToResponse(created);
    }

    public async Task<List<WorkoutProgramResponse>> GetProgramsAsync(Guid memberId)
    {
        var programs = await _repository.GetProgramsByMemberAsync(memberId);
        return programs.Select(ToResponse).ToList();
    }

    public async Task<WorkoutProgramResponse?> GetProgramByIdAsync(Guid memberId, Guid programId)
    {
        var program = await _repository.GetProgramByIdAsync(memberId, programId);
        return program is null ? null : ToResponse(program);
    }

    public async Task<WorkoutProgramResponse> AddExerciseAsync(Guid memberId, Guid programId, AddExerciseRequest request)
    {
        var program = await _repository.GetProgramByIdAsync(memberId, programId)
            ?? throw new KeyNotFoundException("Program ikke fundet");

        var exercise = new Exercise
        {
            WorkoutProgramId = programId,
            Name = request.Name,
            Sets = request.Sets,
            Reps = request.Reps,
            WeightKg = request.WeightKg
        };

        program.Exercises.Add(exercise);
        var updated = await _repository.UpdateProgramAsync(program);
        return ToResponse(updated);
    }

    public async Task DeleteExerciseAsync(Guid memberId, Guid programId, Guid exerciseId)
    {
        var program = await _repository.GetProgramByIdAsync(memberId, programId)
            ?? throw new KeyNotFoundException("Program ikke fundet");

        program.Exercises.RemoveAll(e => e.Id == exerciseId);
        await _repository.UpdateProgramAsync(program);
    }

    public async Task<WorkoutActivityResponse> LogActivityAsync(Guid memberId, CreateWorkoutActivityRequest request)
    {
        var activity = new WorkoutActivity
        {
            MemberId = memberId,
            ActivityName = request.ActivityName,
            ActivityType = request.ActivityType,
            DurationMinutes = request.DurationMinutes,
            PerformedAt = request.PerformedAt
        };

        var logged = await _repository.LogActivityAsync(activity);
        return ToActivityResponse(logged);
    }

    public async Task<List<WorkoutActivityResponse>> GetActivitiesAsync(Guid memberId)
    {
        var activities = await _repository.GetActivitiesByMemberAsync(memberId);
        return activities.Select(ToActivityResponse).ToList();
    }

    private static WorkoutProgramResponse ToResponse(WorkoutProgram p) => new(
        p.Id,
        p.Name,
        p.ExerciseCount,
        p.Exercises.Select(e => new ExerciseResponse(e.Id, e.Name, e.Sets, e.Reps, e.WeightKg)).ToList()
    );

    private static WorkoutActivityResponse ToActivityResponse(WorkoutActivity a) => new(
        a.Id,
        a.ActivityName,
        a.ActivityType,
        a.DurationMinutes,
        a.PerformedAt
    );
}