using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.Api.Models;
using ToDoList.Api.Services;
using ToDoList.Core;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class ToDoItemsController : ControllerBase
    {

        private readonly IToDoItemService _toDoItemService;
        private readonly INewTodoItemService _newTodoItemService;
        private readonly ILogger<ToDoItemsController> _logger;


        public ToDoItemsController(
            IToDoItemService toDoItemService,
            INewTodoItemService newTodoItemService,
            ILogger<ToDoItemsController> logger)
        {
            _toDoItemService = toDoItemService;
            _newTodoItemService = newTodoItemService;
            _logger = logger;

        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ToDoItemDto>), 200)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Get All",
            Description ="Get all to-do items"
            )]
        public async Task<ActionResult<List<ToDoItemDto>>> GetAsync()
        {
            var result = await _toDoItemService.GetAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Get By Id",
            Description = "Get to-do item by id"
            )]
        public async Task<ActionResult<ToDoItemDto>> GetAsync(string id)
        {
            var result = await _toDoItemService.GetAsync(id);
            if (result == null)
            {
                return NotFound($"The item with id {id} does not exist.");
            }
            return Ok(result);
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
            return Created("", toDoItemDto);
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
            bool isCreate = false;
            var existingItem = await _toDoItemService.GetAsync(id);
            if (existingItem is null)
            {
                isCreate = true;
                await _toDoItemService.CreateAsync(toDoItemDto);
            }
            else
            {
                await _toDoItemService.ReplaceAsync(id, toDoItemDto);
            }

            return isCreate ? Created("", toDoItemDto) : Ok(toDoItemDto);
        }

        [HttpPut("{id}/description")]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(typeof(ToDoItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Upsert Item",
            Description = "Create or replace a to-do item by id"
            )]
        public async Task<ActionResult<ToDoItemDto>> PutDescriptionAsync(string id, [FromQuery] string? newDescription = null)
        {
            CoreTodoItem coreTodoItem = await _newTodoItemService.ModifyDescription(id, newDescription);
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

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Delete Item",
            Description = "Delete a to-do item by id"
            )]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var isSuccessful = await _toDoItemService.RemoveAsync(id);
            if (!isSuccessful)
            {
                return NotFound($"The item with id {id} does not exist.");
            }
            _logger.LogInformation($"To-do item {id} deleted.");
            return NoContent();
        }
    }
}
