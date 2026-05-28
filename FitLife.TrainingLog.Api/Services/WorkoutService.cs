using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Repositories;


namespace FitLife.TrainingLog.Api.Services;

// Mapper mellem DTOs og domænemodeller og delegerer dataoperationer til repository
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

    // Kaster KeyNotFoundException hvis programmet ikke findes, som controlleren fanger og returnerer 404 på
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
    
    public async Task<WorkoutProgramResponse?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName)
    {
        var program = await _repository.UpdateProgramNameAsync(memberId, programId, newName);
        return program is null ? null : ToResponse(program);
    }

    public async Task<WorkoutProgramResponse?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg)
    {
        var program = await _repository.UpdateExerciseAsync(memberId, programId, exerciseId, name, sets, reps, weightKg);
        return program is null ? null : ToResponse(program);
    }
    
    // Mapper domænemodel til response-DTO inklusive alle indlejrede øvelser
    private static WorkoutProgramResponse ToResponse(WorkoutProgram p) => new(
        p.Id,
        p.Name,
        p.Exercises.Select(e => new ExerciseResponse(e.Id, e.Name, e.Sets, e.Reps, e.WeightKg)).ToList()
    );
}