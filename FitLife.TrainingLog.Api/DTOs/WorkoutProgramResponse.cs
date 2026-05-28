namespace FitLife.TrainingLog.Api.DTOs;

// Response-DTO for en enkelt øvelse
public record ExerciseResponse(
    Guid Id,
    string Name,
    int Sets,
    int Reps,
    decimal? WeightKg);

// Response-DTO for et træningsprogram med alle øvelser indlejret
public record WorkoutProgramResponse(
    Guid Id,
    string Name,
    List<ExerciseResponse> Exercises);