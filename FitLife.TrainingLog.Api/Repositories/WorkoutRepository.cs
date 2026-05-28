using FitLife.TrainingLog.Api.Models;
using MongoDB.Driver;

namespace FitLife.TrainingLog.Api.Repositories;

// MongoDB-implementering af IWorkoutRepository.
// Programmer og aktiviteter gemmes i to separate samlinger.
public class WorkoutRepository : IWorkoutRepository
{
    private readonly IMongoCollection<WorkoutProgram> _programs;

    public WorkoutRepository(IMongoDatabase database)
    {
        _programs = database.GetCollection<WorkoutProgram>("workoutprograms");
    }

    public async Task<WorkoutProgram> CreateProgramAsync(WorkoutProgram program)
    {
        await _programs.InsertOneAsync(program);
        return program;
    }

    public async Task<List<WorkoutProgram>> GetProgramsByMemberAsync(Guid memberId)
    {
        return await _programs.Find(p => p.MemberId == memberId).ToListAsync();
    }

    // Filtrerer på både programId og memberId så et program ikke kan hentes af en anden bruger
    public async Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId)
    {
        return await _programs
            .Find(p => p.Id == programId && p.MemberId == memberId)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutProgram> UpdateProgramAsync(WorkoutProgram program)
    {
        await _programs.ReplaceOneAsync(p => p.Id == program.Id, program);
        return program;
    }

    public async Task<WorkoutProgram?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName)
    {
        var program = await GetProgramByIdAsync(memberId, programId);
        if (program is null) return null;

        program.Name = newName;
        await _programs.ReplaceOneAsync(p => p.Id == programId, program);
        return program;
    }
    
    // Henter programmet, opdaterer øvelsen i hukommelsen og gemmer hele dokumentet igen
    public async Task<WorkoutProgram?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg)
    {
        var program = await GetProgramByIdAsync(memberId, programId);
        if (program is null) return null;

        var exercise = program.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise is null) return null;

        exercise.Name = name;
        exercise.Sets = sets;
        exercise.Reps = reps;
        exercise.WeightKg = weightKg;

        await _programs.ReplaceOneAsync(p => p.Id == programId, program);
        return program;
    }
}