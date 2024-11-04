using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Api.Models
{
    [BsonIgnoreExtraElements]
    public class ToDoItem
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; }
        public bool Favorite { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTime CreatedTime { get; set; }
        [BsonRepresentation(BsonType.String)]
        public DateTime DueDate { get; set; }
    }
}
