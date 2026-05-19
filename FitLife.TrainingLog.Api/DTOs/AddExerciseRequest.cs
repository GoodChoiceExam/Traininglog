namespace FitLife.TrainingLog.Api.DTOs;

public record AddExerciseRequest(
    string Name,
    int Sets,
    int Reps,
    decimal? WeightKg);