using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Api.Models;
using ToDoList.Api.Services;
using ToDoList.Infrastructure;

namespace ToDoList.Api.IntegrationTests
{
    public class ToDoItemsApiTest
    : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ToDoItemsApiTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(c =>
            {
                c.ConfigureTestServices(
                    x =>
                    {
                        x.PostConfigureAll<TodoStoreDatabaseSettings>(s => s.DatabaseName = "Test");
                        x.AddSingleton<IToDoItemService, InMemoryToDoItemService>();
                    });
            }
           );
        }

        [Fact]
        public async Task CRUD_APIs()
        {
            var token = TokenUtil.GetHSJwt();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = token;
            var id = Guid.NewGuid().ToString();
            var toDoItem = new ToDoItemDto
            {
                Id = id,
                Description = "Test",
                CreatedTime = DateTime.UtcNow,
                Done = false
            };
            var url = $"api/v1/todoitems";
            var createResponse = await client.PutAsJsonAsync(url + $"/{id}", toDoItem);

            createResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            var model = await createResponse.Content.ReadFromJsonAsync<ToDoItemDto>();
            Assert.NotNull(model);

            var getAllResponse = await client.GetAsync(url);
            getAllResponse.EnsureSuccessStatusCode();
            var toDoItems = await getAllResponse.Content.ReadFromJsonAsync<List<ToDoItemDto>>();
            Assert.NotNull(toDoItems);
            Assert.Single(toDoItems);


            var getResponse = await client.GetAsync(url + $"/{id}");
            getResponse.EnsureSuccessStatusCode();
            var toDoItemDto = await getResponse.Content.ReadFromJsonAsync<ToDoItemDto>();
            Assert.NotNull(toDoItemDto);

            toDoItem.Description = "Updated";

            var putResponse = await client.PutAsJsonAsync(url + $"/{id}", toDoItem);
            putResponse.EnsureSuccessStatusCode();
            var updatedItem = await putResponse.Content.ReadFromJsonAsync<ToDoItemDto>();
            Assert.NotNull(updatedItem);
            Assert.Equal("Updated", updatedItem.Description);

            getResponse = await client.GetAsync(url + $"/{id}");
            getResponse.EnsureSuccessStatusCode();
            toDoItemDto = await getResponse.Content.ReadFromJsonAsync<ToDoItemDto>();
            Assert.NotNull(toDoItemDto);

            Assert.Equal("Updated", toDoItem.Description);

            var deleteResponse = await client.DeleteAsync(url + $"/{id}");
            deleteResponse.EnsureSuccessStatusCode();

            getAllResponse = await client.GetAsync(url);
            getAllResponse.EnsureSuccessStatusCode();
            toDoItems = await getAllResponse.Content.ReadFromJsonAsync<List<ToDoItemDto>>();
            Assert.NotNull(toDoItems);
            Assert.Equal(0, toDoItems.Count);



        }
    }

}