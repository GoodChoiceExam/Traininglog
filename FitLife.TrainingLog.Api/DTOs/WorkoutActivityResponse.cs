using FitLife.TrainingLog.Api.Models;

namespace FitLife.TrainingLog.Api.DTOs;

// Returneres til frontend efter en aktivitet er logget eller hentet
public record WorkoutActivityResponse(
    Guid Id,
    string ActivityName,
    ActivityType ActivityType,
    int DurationMinutes,
    DateTime PerformedAt);