namespace ToDoList.Core
{
    public interface ITodoItemsRepository
    {
        public Task<long> CountTodoItemsOnTheSameDueDate(DateTime dueDate);
        public Task<List<CoreTodoItem>> GetTodoItemsDueInNextFiveDays();
        public Task<CoreTodoItem> FindById(string id);
        public Task Save(CoreTodoItem todoItem);
    }
}
