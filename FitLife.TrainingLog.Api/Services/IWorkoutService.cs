using FitLife.TrainingLog.Api.DTOs;

namespace FitLife.TrainingLog.Api.Services;

public interface IWorkoutService
{
    Task<WorkoutProgramResponse> CreateProgramAsync(Guid memberId, CreateWorkoutProgramRequest request);
    Task<List<WorkoutProgramResponse>> GetProgramsAsync(Guid memberId);
    Task<WorkoutProgramResponse?> GetProgramByIdAsync(Guid memberId, Guid programId);
    Task<WorkoutProgramResponse> AddExerciseAsync(Guid memberId, Guid programId, AddExerciseRequest request);
    Task DeleteExerciseAsync(Guid memberId, Guid programId, Guid exerciseId);

    Task<WorkoutActivityResponse> LogActivityAsync(Guid memberId, CreateWorkoutActivityRequest request);
    Task<List<WorkoutActivityResponse>> GetActivitiesAsync(Guid memberId);
}