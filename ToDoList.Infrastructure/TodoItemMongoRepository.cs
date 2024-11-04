using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToDoList.Core;

namespace ToDoList.Infrastructure;

public class TodoItemMongoRepository : ITodoItemsRepository
{
    private readonly IMongoCollection<TodoItemPo?> _todosCollection;

    public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
        _todosCollection = mongoDatabase.GetCollection<TodoItemPo>(todoStoreDatabaseSettings.Value.CollectionName);
    }

    public async Task<TodoItem> FindById(string? id)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, id);
        TodoItemPo? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemPo 转换为 TodoItem
        TodoItem todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    private TodoItem ConvertToTodoItem(TodoItemPo? todoItemPo)
    {
        if (todoItemPo == null) return null;

        return new ToDoList.Core.TodoItem
        {
            Id = todoItemPo.Id,
            Description = todoItemPo.Description,
            IsComplete = todoItemPo.IsComplete,
            DueDate = todoItemPo.DueDate,
            CreatedTime = todoItemPo.CreatedTime,
            Modifications = [.. todoItemPo.Modifications]
        };
    }

    private TodoItemPo ConvertToTodoItemPo(TodoItem? todoItem)
    {
        if (todoItem == null) return null;
        return new TodoItemPo
        {
            Id = todoItem.Id,
            Description = todoItem.Description,
            IsComplete = todoItem.IsComplete,
            CreatedTime = todoItem.CreatedTime,
            Modifications = [.. todoItem.Modifications],
            DueDate = todoItem.DueDate
        };
    }

    public async void Save(TodoItem? todoItem)
    {
        ArgumentNullException.ThrowIfNull(todoItem);
        TodoItemPo todoItemPo = ConvertToTodoItemPo(todoItem);
        if (FindById(todoItemPo.Id) == null)
        {
            // Insert new item
            await _todosCollection.InsertOneAsync(todoItemPo);
        }
        else
        {
            // Update existing item
            var filter = Builders<TodoItemPo>.Filter.Eq(t => t.Id, todoItemPo.Id);
            await _todosCollection.ReplaceOneAsync(filter, todoItemPo, new ReplaceOptions { IsUpsert = true });
        }
    }

    public async Task<long> CountTodoItemsOnTheSameDueDate(DateTime dueDate)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.DueDate, dueDate);
        var count = await _todosCollection.CountDocumentsAsync(filter);

        return count;
    }

    public async Task<List<TodoItem>> GetTodoItemsDueInNextFiveDays()
    {
        var today = DateTime.Today.Date.ToUniversalTime();

        var filter = Builders<TodoItemPo>.Filter.And(
            Builders<TodoItemPo>.Filter.Gte(item => item.DueDate, today),
            Builders<TodoItemPo>.Filter.Lt(item => item.DueDate, today.AddDays(5)));

        var todoItemPos = await _todosCollection.Find(filter).ToListAsync();
        var todoItems = todoItemPos.Select(ConvertToTodoItem).ToList();
        return todoItems;
    }
}
