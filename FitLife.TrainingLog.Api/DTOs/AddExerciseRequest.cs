namespace FitLife.TrainingLog.Api.DTOs;

// Request body til at tilføje eller opdatere en øvelse i et træningsprogram
public record AddExerciseRequest(
    string Name,
    int Sets,
    int Reps,
    decimal? WeightKg);