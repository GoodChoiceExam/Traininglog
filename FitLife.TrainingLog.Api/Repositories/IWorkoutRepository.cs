using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.Repositories;

public interface IWorkoutRepository
{
    Task<WorkoutProgram> CreateProgramAsync(WorkoutProgram program);
    Task<List<WorkoutProgram>> GetProgramsByMemberAsync(Guid memberId);
    Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId);
    Task<WorkoutProgram> UpdateProgramAsync(WorkoutProgram program);

    Task<WorkoutActivity> LogActivityAsync(WorkoutActivity activity);
    Task<List<WorkoutActivity>> GetActivitiesByMemberAsync(Guid memberId);
    Task<WorkoutProgram?> UpdateProgramNameAsync(Guid memberId, Guid programId, string newName);
    Task<WorkoutProgram?> UpdateExerciseAsync(Guid memberId, Guid programId, Guid exerciseId, string name, int sets, int reps, decimal? weightKg);
}