namespace FitLife.TrainingLog.Api.DTOs;

public record ExerciseResponse(
    Guid Id,
    string Name,
    int Sets,
    int Reps,
    decimal? WeightKg);

public record WorkoutProgramResponse(
    Guid Id,
    string Name,
    int ExerciseCount,
    List<ExerciseResponse> Exercises);