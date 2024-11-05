using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System.Net;
using System.Text;
using System.Text.Json;
using ToDoList.Api.Models;
using ToDoList.Core;
using ToDoList.Infrastructure;

namespace ToDoList.Api.ApiTests
{
    public class PutOneTodoItemTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IMongoCollection<ToDoItem> _mongoCollection;

        public PutOneTodoItemTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("TodoItem");
            _mongoCollection = mongoDatabase.GetCollection<ToDoItem>("todos");
        }

        public async Task InitializeAsync()
        {
            await _mongoCollection.DeleteManyAsync(FilterDefinition<ToDoItem>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async void Should_PutAsync_CreateTodoItem()
        {
            var id = Guid.NewGuid().ToString();
            var toDoItemDto = new ToDoItemDto
            {
                Id = id,
                Description = "Test",
                CreatedTime = DateTime.UtcNow,
                Done = false
            };

            var json = JsonSerializer.Serialize(toDoItemDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/v1/todoitems/{id}",content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();

            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodos);
            Assert.Equal("Test", returnedTodos.Description);
            Assert.False(returnedTodos.Favorite);
            Assert.False(returnedTodos.Done);
        }

        [Fact]
        public async void Should_PutAsync_ModifyTodoItem()
        {
            var id = Guid.NewGuid().ToString();
            
            // Arrange
            var todoItem = new ToDoItem
            {
                Id = id,
                Description = "Buy groceries",
                Done = false,
                Favorite = true,
                CreatedTime = DateTime.UtcNow
            };

            var toDoItemDto = new ToDoItemDto
            {
                Id = id,
                Description = "test description",
                CreatedTime = todoItem.CreatedTime,
                Done = todoItem.Done,
                Favorite = todoItem.Favorite,
            };

            await _mongoCollection.InsertOneAsync(todoItem);

            var json = JsonSerializer.Serialize(toDoItemDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/v1/todoitems/{id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();

            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodos);
            Assert.Equal("test description", returnedTodos.Description);
            Assert.True(returnedTodos.Favorite);
            Assert.False(returnedTodos.Done);
        }

        [Fact]
        public async Task Should_PutAsync_ModifyTodoItem_v2()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            

            var toDoItem = new ToDoItemDto
            {
                Id = id,
                Description = "Initial Description"
            };
            string newDescription = "NewDescription";
            var requestUri = $"/api/v2/todoitemsV2/{id}?newDescription={newDescription}";
            var content = new StringContent(newDescription, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync(requestUri, content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.Equal(id, returnedTodos.Id);
            Assert.Equal(newDescription, returnedTodos.Description);
        }
        
    }
}
