using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.Services;

// Kontrakt for forretningslogik around træningsprogrammer og aktivitetslogning
public interface IWorkoutService
{
    Task<WorkoutProgram> CreateProgramAsync(Guid memberId, CreateWorkoutProgramRequest request);
    Task<List<WorkoutProgram>> GetProgramsAsync(Guid memberId);
    Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId);
    Task<WorkoutProgram> AddExerciseAsync(Guid memberId, Guid programId, AddExerciseRequest request);
    Task DeleteExerciseAsync(Guid memberId, Guid programId, Guid exerciseId);
    Task<WorkoutProgram?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName);
    Task<WorkoutProgram?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg);
}