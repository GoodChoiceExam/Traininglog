using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.TrainingLog.Api.Models;

public class Exercise
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WorkoutProgramId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public decimal? WeightKg { get; set; }
}