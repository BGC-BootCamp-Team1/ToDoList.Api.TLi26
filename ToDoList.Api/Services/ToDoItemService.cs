using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToDoList.Api.Models;
using ToDoList.Infrastructure;

namespace ToDoList.Api.Services
{
    public class ToDoItemService : IToDoItemService
    {

        private readonly IMongoCollection<ToDoItem> _ToDoItemsCollection;

        private readonly ILogger<ToDoItemService> _Logger;

        public ToDoItemService(
            IOptions<TodoStoreDatabaseSettings> ToDoItemStoreDatabaseSettings, ILogger<ToDoItemService> logger)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            _ToDoItemsCollection = mongoDatabase.GetCollection<ToDoItem>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
            _Logger = logger;
        }

        public async Task CreateAsync(ToDoItemDto newToDoItem)
        {
            var item = new ToDoItem
            {
                Id = newToDoItem.Id,
                Description = newToDoItem.Description,
                Done = newToDoItem.Done,
                Favorite = newToDoItem.Favorite,
                CreatedTime = newToDoItem.CreatedTime,
            };
            _Logger.LogInformation($"Inserting new todo item to DB {newToDoItem.Id}");

            await _ToDoItemsCollection.InsertOneAsync(item);
        }

        
        public async Task<List<ToDoItemDto>> GetAsync()
        {
            var toDoItemDtos = new List<ToDoItemDto>();
            var toDoItems = await _ToDoItemsCollection.Find(_ => true).ToListAsync();
            if (toDoItems is null)
            {
                return toDoItemDtos;
            }
            for (var i = 0; i < toDoItems.Count; i++)
            {
                toDoItemDtos.Add(new ToDoItemDto
                {
                    Id = toDoItems[i].Id,
                    Description = toDoItems[i].Description,
                    Done = toDoItems[i].Done,
                    CreatedTime = toDoItems[i].CreatedTime,
                    Favorite = toDoItems[i].Favorite
                });
            }
            return toDoItemDtos;

        }

        public async Task<ToDoItemDto?> GetAsync(string id)
        {
            var toDoItem = await _ToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (toDoItem is null) { return null; }
            var toDoItemDto = new ToDoItemDto
            {
                Id = toDoItem.Id,
                Description = toDoItem.Description,
                Done = toDoItem.Done,
                Favorite = toDoItem.Favorite,
                CreatedTime = toDoItem.CreatedTime,
            };
            return toDoItemDto;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _ToDoItemsCollection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task ReplaceAsync(string id, ToDoItemDto updatedToDoItem)
        {
            var item = new ToDoItem { Id = id, Description = updatedToDoItem.Description, Done = updatedToDoItem.Done };
            await _ToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, item);
        }
    }
}
