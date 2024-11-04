using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.Api.Models;
using ToDoList.Api.Services;
using ToDoList.Core;
using ToDoList.Core.DueDateSettingStrategy;
using ToDoList.Infrastructure;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v2/[controller]")]
    [AllowAnonymous]
    public class ToDoItemsV2Controller : ControllerBase
    {
        private readonly INewTodoItemService _newTodoItemService;
        private readonly ILogger<ToDoItemsController> _logger;

        public ToDoItemsV2Controller(
            INewTodoItemService newTodoItemService,
            ILogger<ToDoItemsController> logger)
        {
            _newTodoItemService = newTodoItemService;
            _logger = logger;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ToDoItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Create New Item",
            Description = "Create a new to-do item"
            )]

        public async Task<ActionResult<ToDoItemDto>> PostAsync([FromBody] ToDoItemCreateRequest toDoItemCreateRequest)
        {
            CoreTodoItem todoItem = _newTodoItemService.CreateItem(
                toDoItemCreateRequest.Description,
                toDoItemCreateRequest.UserProvidedDueDate,
                toDoItemCreateRequest.DueDateSettingOption
                );
            var toDoItemDto = new ToDoItemDto
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
                CreatedTime = todoItem.CreatedTime,
                DueDate = todoItem.DueDate
            };
            return toDoItemDto;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(typeof(ToDoItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Upsert Item",
            Description = "Create or replace a to-do item by id"
            )]
        public async Task<ActionResult<ToDoItemDto>> PutAsync(string id, [FromBody] ToDoItemDto toDoItemDto)
        {
            CoreTodoItem coreTodoItem = await _newTodoItemService.ModifyDescription(id, toDoItemDto.Description);
            var newItemDto = new ToDoItemDto
            {
                Id = coreTodoItem.Id,
                Description = coreTodoItem.Description,
                CreatedTime = coreTodoItem.CreatedTime,
                DueDate = coreTodoItem.DueDate,
                Done = coreTodoItem.Done,
                Favorite = coreTodoItem.Favorite
            };
            return newItemDto;
        }
    }
}
