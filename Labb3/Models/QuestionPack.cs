using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3.Models;

public enum Difficulty { Easy, Medium, Hard }

public class QuestionPack
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public QuestionPack()
    {
        Name = string.Empty;
        Difficulty = Difficulty.Medium;
        TimeLimitInSeconds = 30;
        Questions = new List<Question>();
    }

    public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
    {
        Name = name;
        Difficulty = difficulty;
        TimeLimitInSeconds = timeLimitInSeconds;
        Questions = new List<Question>();
    }

    public string Name { get; set; }

    public Difficulty Difficulty { get; set; }

    public int TimeLimitInSeconds { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? CategoryId { get; set; }

    [BsonIgnore]
    public string? CategoryName { get; set; }

    public List<Question> Questions { get; set; }
}
