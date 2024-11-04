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

        public CoreTodoItem CreateItem(string description, DateTime? userProvidedDueDate, DueDateSettingOption dueDateSettingOption = 0)
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
            CoreTodoItem todoItem = new CoreTodoItem(description, dueDate);
            _todosRepository.Save(todoItem);
            return todoItem;
        }

        public async Task<CoreTodoItem> ModifyDescription(string id, string description)
        {
            CoreTodoItem todoItem = await _todosRepository.FindById(id);
            todoItem.ModifyDescription(description);
            await _todosRepository.Save(todoItem);
            return todoItem;
        }
    }
}
