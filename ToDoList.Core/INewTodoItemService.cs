using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Core
{
    public interface INewTodoItemService
    {
        CoreTodoItem CreateItem(string description, DateTime? userProvidedDueDate, DueDateSettingOption dueDateSettingOption = DueDateSettingOption.SelectFirstAvailableDay);
        Task<CoreTodoItem> ModifyDescription(string id, string description);
    }
}