using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.DTOs;

public record WorkoutActivityResponse(
    Guid Id,
    string ActivityName,
    ActivityType ActivityType,
    int DurationMinutes,
    DateTime PerformedAt);