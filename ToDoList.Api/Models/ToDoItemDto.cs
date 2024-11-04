using System.ComponentModel.DataAnnotations;
using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Api.Models
{
    public class ToDoItemDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [StringLength(50)]
        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; } = false;
        public bool Favorite { get; set; } = false;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
    }
}
