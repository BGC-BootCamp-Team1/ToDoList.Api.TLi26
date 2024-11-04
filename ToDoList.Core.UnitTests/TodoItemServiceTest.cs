using ToDoList.Core;
using Moq;
using ToDoList.Core.ApplicationExcepetions;
using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Core.Test
{
    public class TodoItemServiceTest
    {
        [Fact]
        public void CreateItem_ShouldThrowException_WhenCreateNinthItem()
        {
            DateTime dueDate = DateTime.Now.AddDays(7);

            var mockRepository = new Mock<ITodoItemsRepository>();

            mockRepository.Setup(repo => repo.CountTodoItemsOnTheSameDueDate(It.IsAny<DateTime>()))
                .ReturnsAsync(8);

            var service = new NewTodoItemService(mockRepository.Object);

            Assert.Throws<ExceedMaxTodoItemsPerDueDateException>(() => service.CreateItem("test", dueDate));
        }

        [Fact]
        public void CreateItem_ShouldCreate_WhenCreateSecondItem()
        {
            DateTime dueDate = DateTime.Now.AddDays(7);

            var mockRepository = new Mock<ITodoItemsRepository>();
            mockRepository.Setup(repo => repo.CountTodoItemsOnTheSameDueDate(It.IsAny<DateTime>()))
                .ReturnsAsync(1);

            var service = new NewTodoItemService(mockRepository.Object);
            var actualTodoItem = service.CreateItem("test", dueDate);

            Assert.Equal("test", actualTodoItem.Description);
            Assert.Equal(dueDate, actualTodoItem.DueDate);
        }

        [Fact]
        public void CreateItem_ShouldThrowEexceptionWhenEarlierDueDate()
        {
            DateTime dueDate = DateTime.Now.AddDays(-10);

            var mockRepository = new Mock<ITodoItemsRepository>();
            mockRepository.Setup(repo => repo.CountTodoItemsOnTheSameDueDate(It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            var service = new NewTodoItemService(mockRepository.Object);

            Assert.Throws<TooEarlyDueDateException>(() => service.CreateItem("test", dueDate));
        }

        [Fact]
        public async Task CreateItem_ShouldSetFirstAvailableDay_WhenUserProvidedDueDateIsNull()
        {
            // Arrange
            var description = "Test Todo Item";
            DateTime? userProvidedDueDate = null;
            DueDateSettingOption dueDateSettingOption = DueDateSettingOption.SelectFirstAvailableDay;
            var todoItemsDueInNextFiveDays = new List<TodoItem>
            {
                new TodoItem("Existing Item 1", DateTime.Now.AddDays(1)),
                new TodoItem("Existing Item 2", DateTime.Now.AddDays(3))
            };
            var mockRepository = new Mock<ITodoItemsRepository>();
            mockRepository
                .Setup(repo => repo.GetTodoItemsDueInNextFiveDays())
                .ReturnsAsync(todoItemsDueInNextFiveDays);

            DateTime expectedDueDate = DateTime.Now.Date;

            var service = new NewTodoItemService(mockRepository.Object);

            // Act
            var result = service.CreateItem(description, userProvidedDueDate, dueDateSettingOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(description, result.Description);
            Assert.Equal(expectedDueDate, result.DueDate);
        }

        [Fact]
        public async Task CreateItem_ShouldSetFewestTodoItemsDay_WhenUserProvidedDueDateIsNull()
        {
            // Arrange
            var description = "Test Todo Item";
            DateTime? userProvidedDueDate = null;
            DueDateSettingOption dueDateSettingOption = DueDateSettingOption.SelectFewestTodoItemsDay;
            var todoItemsDueInNextFiveDays = new List<TodoItem>
            {
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(0) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(0) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(1) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(2) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(2) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(3) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(3) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(4) },
                new TodoItem { DueDate = DateTime.Today.Date.AddDays(4) }
            };
            var mockRepository = new Mock<ITodoItemsRepository>();
            mockRepository
                .Setup(repo => repo.GetTodoItemsDueInNextFiveDays())
                .ReturnsAsync(todoItemsDueInNextFiveDays);

            DateTime expectedDueDate = DateTime.Today.Date.AddDays(1);

            var service = new NewTodoItemService(mockRepository.Object);

            // Act
            var result = service.CreateItem(description, userProvidedDueDate, dueDateSettingOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(description, result.Description);
            Assert.Equal(expectedDueDate, result.DueDate);
        }

        [Fact]
        public async Task ModifyDescription_ShouldModifyDescription()
        {
            // Arrange
            TodoItem todoItem = new TodoItem("test", DateTime.Today.AddDays(7));

            var mockRepository = new Mock<ITodoItemsRepository>();
            var service = new NewTodoItemService(mockRepository.Object);

            // Act & Assert
            Assert.Null(Record.Exception(() => service.ModifyDescription("test modify", todoItem)));
            mockRepository.Verify(r => r.Save(It.Is<TodoItem>(t => t == todoItem)), Times.Once);
        }
    }
}
