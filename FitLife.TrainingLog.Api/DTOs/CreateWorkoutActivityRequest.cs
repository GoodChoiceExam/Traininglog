using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.DTOs;

public record CreateWorkoutActivityRequest(
    string ActivityName,
    ActivityType ActivityType,
    int DurationMinutes,
    DateTime PerformedAt);