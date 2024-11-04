using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Core
{
    public interface INewTodoItemService
    {
        TodoItem CreateItem(string description, DateTime? userProvidedDueDate, DueDateSettingOption dueDateSettingOption = DueDateSettingOption.SelectFirstAvailableDay);
        void ModifyDescription(string description, TodoItem todoItem);
    }
}