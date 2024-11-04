using System.ComponentModel.DataAnnotations;
using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Api.Models
{
    public class ToDoItemCreateRequest
    {
        [Required]
        [StringLength(50)]
        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; }
        public bool Favorite { get; set; }
        public DateTime? UserProvidedDueDate { get; set; }
        public DueDateSettingOption DueDateSettingOption { get; set; }

    }
}
