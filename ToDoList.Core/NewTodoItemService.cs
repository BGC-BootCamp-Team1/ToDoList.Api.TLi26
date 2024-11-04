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
            TodoItem todoItem = new TodoItem(description, dueDate);
            _todosRepository.Save(todoItem);
            return todoItem;
        }

        public async Task ModifyDescription(string id, string description)
        {
            TodoItem todoItem = await _todosRepository.FindById(id);
            todoItem.ModifyDescription(description);
            _todosRepository.Save(todoItem);
        }
    }
}
