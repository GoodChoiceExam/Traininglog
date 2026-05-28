using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Repositories;


namespace FitLife.TrainingLog.Api.Services;

// Delegerer dataoperationer til repository og returnerer domænemodeller
public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkoutProgram> CreateProgramAsync(Guid memberId, CreateWorkoutProgramRequest request)
    {
        var program = new WorkoutProgram
        {
            MemberId = memberId,
            Name = request.Name
        };

        return await _repository.CreateProgramAsync(program);
    }

    public async Task<List<WorkoutProgram>> GetProgramsAsync(Guid memberId)
    {
        return await _repository.GetProgramsByMemberAsync(memberId);
    }

    public async Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId)
    {
        return await _repository.GetProgramByIdAsync(memberId, programId);
    }

    // Kaster KeyNotFoundException hvis programmet ikke findes, som controlleren fanger og returnerer 404 på
    public async Task<WorkoutProgram> AddExerciseAsync(Guid memberId, Guid programId, AddExerciseRequest request)
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
        return await _repository.UpdateProgramAsync(program);
    }

    public async Task DeleteExerciseAsync(Guid memberId, Guid programId, Guid exerciseId)
    {
        var program = await _repository.GetProgramByIdAsync(memberId, programId)
            ?? throw new KeyNotFoundException("Program ikke fundet");

        program.Exercises.RemoveAll(e => e.Id == exerciseId);
        await _repository.UpdateProgramAsync(program);
    }
    
    public async Task<WorkoutProgram?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName)
    {
        return await _repository.UpdateProgramNameAsync(memberId, programId, newName);
    }

    public async Task<WorkoutProgram?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg)
    {
        return await _repository.UpdateExerciseAsync(memberId, programId, exerciseId, name, sets, reps, weightKg);
    }
}