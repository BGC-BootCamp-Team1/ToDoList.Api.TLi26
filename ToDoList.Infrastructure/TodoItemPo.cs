using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ToDoList.Core;

namespace ToDoList.Infrastructure;

public class TodoItemPo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public IList<Modification> Modifications { get; set; } = [];
    public DateTime DueDate { get; set; }
}