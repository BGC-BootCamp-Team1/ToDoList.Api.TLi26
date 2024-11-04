using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Core
{
    public class NewTodoItemService : INewTodoItemService
    {
        private readonly ITodoItemsRepository _todosRepository;

        public NewTodoItemService(ITodoItemsRepository repository)
        {
            _todosRepository = repository;
        }

        public TodoItem CreateItem(string description, DateTime? userProvidedDueDate, DueDateSettingOption dueDateSettingOption = 0)
        {
            DateTime dueDate;
            if (userProvidedDueDate.HasValue)
            {
                var count = _todosRepository.CountTodoItemsOnTheSameDueDate(userProvidedDueDate.Value).Result;
                dueDate = DueDateSetter.ValidUserDueDate(userProvidedDueDate.Value, count);
            }
            else
            {
                var todoItemsDueInNextFiveDays = _todosRepository.GetTodoItemsDueInNextFiveDays().Result;
                dueDate = DueDateSetter.AutoSetDueDate(todoItemsDueInNextFiveDays, dueDateSettingOption);
            }
            TodoItem item = new TodoItem(description, dueDate);
            return item;
        }

        public void ModifyDescription(string description, TodoItem todoItem)
        {
            todoItem.ModifyDescription(description);
            _todosRepository.Save(todoItem);
        }
    }
}
