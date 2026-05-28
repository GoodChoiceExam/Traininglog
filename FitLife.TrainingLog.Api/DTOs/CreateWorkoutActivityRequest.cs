using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.DTOs;

// Request body til at logge en ny træningsaktivitet
public record CreateWorkoutActivityRequest(
    string ActivityName,
    ActivityType ActivityType,
    int DurationMinutes,
    DateTime PerformedAt);