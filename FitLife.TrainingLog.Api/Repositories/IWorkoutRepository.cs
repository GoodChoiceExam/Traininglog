using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.Repositories;

// Kontrakt for dataadgang til træningsprogrammer og aktiviteter.
// MemberId sendes med til opslag så et medlem aldrig kan tilgå andres programmer.
public interface IWorkoutRepository
{
    Task<WorkoutProgram> CreateProgramAsync(WorkoutProgram program);
    Task<List<WorkoutProgram>> GetProgramsByMemberAsync(Guid memberId);
    Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId);
    Task<WorkoutProgram> UpdateProgramAsync(WorkoutProgram program);
    Task<WorkoutProgram?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName);
    Task<WorkoutProgram?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg);
}