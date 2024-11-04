using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ToDoList.Core;

namespace ToDoList.Infrastructure;

public class TodoItemPo
{
    [BsonId]
    public string? Id { get; set; }
    public string Description { get; set; }
    public bool Done { get; set; } = false;
    public bool Favorite { get; set; } = false;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public IList<Modification> Modifications { get; set; } = [];
    public DateTime DueDate { get; set; }
}