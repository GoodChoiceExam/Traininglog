using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.TrainingLog.Api.Models;

public class WorkoutActivity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MemberId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime PerformedAt { get; set; }
}