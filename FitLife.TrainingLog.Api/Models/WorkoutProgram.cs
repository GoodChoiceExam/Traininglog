using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.TrainingLog.Api.Models;

// Et træningsprogram tilhørende et enkelt medlem.
// Øvelser gemmes som en indlejret liste i dokumentet fremfor i en separat samling.
// ExerciseCount er en beregnet property der ikke gemmes i MongoDB.
public class WorkoutProgram
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MemberId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = [];

    public int ExerciseCount => Exercises.Count;
}