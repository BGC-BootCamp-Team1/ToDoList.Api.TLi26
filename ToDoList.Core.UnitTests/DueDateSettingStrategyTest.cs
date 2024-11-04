using ToDoList.Core.ApplicationExcepetions;
using ToDoList.Core.DueDateSettingStrategy;

namespace ToDoList.Core.Test
{
    public class DueDateSettingStrategyTest
    {
        [Fact]
        public void ValidUserDueDate_ShouldThrowTooEarlyDueDateException_WhenDueDateIsInThePast()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.Date.AddDays(-1);

            // Act & Assert
            Assert.Throws<TooEarlyDueDateException>(() => DueDateSetter.ValidUserDueDate(pastDate, 0));
        }

        [Fact]
        public void ValidUserDueDate_ShouldThrowExceedMaxTodoItemsPerDueDateException_WhenCountExceedsMax()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.Date.AddDays(1);
            long countExceedsMax = Constants.MAX_ITEM_SAME_DUEDAY;

            // Act & Assert
            Assert.Throws<ExceedMaxTodoItemsPerDueDateException>(() => DueDateSetter.ValidUserDueDate(futureDate, countExceedsMax));
        }

        [Fact]
        public void ValidUserDueDate_ShouldReturnProvidedDueDate_WhenConditionsAreMet()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.Date.AddDays(1);
            long validCount = Constants.MAX_ITEM_SAME_DUEDAY - 1;

            // Act
            DateTime result = DueDateSetter.ValidUserDueDate(futureDate, validCount);

            // Assert
            Assert.Equal(futureDate, result);
        }

        [Fact]
        public void FirstAvailableDayStrategy_GetDueDate_ReturnsFirstAvailableDate()
        {
            // Arrange
            var dueDateSettingStrategy = new FirstAvailableDayStrategy();
            var startDate = new DateTime(2024, 11, 1);
            var todoItemsDueInNextFiveDays = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTime(2024, 11, 1) },
                new TodoItem { DueDate = new DateTime(2024, 11, 1) },
                new TodoItem { DueDate = new DateTime(2024, 11, 2) },
                new TodoItem { DueDate = new DateTime(2024, 11, 3) },
                new TodoItem { DueDate = new DateTime(2024, 11, 4) },
            };

            // Act
            var result = dueDateSettingStrategy.GetDueDate(startDate, todoItemsDueInNextFiveDays);

            // Assert
            Assert.Equal(new DateTime(2024, 11, 1), result);
        }

        [Fact]
        public void FirstAvailableDayStrategy_GetDueDate_ThrowsException_WhenNoAvailableDays()
        {
            // Arrange
            var dueDateSettingStrategy = new FirstAvailableDayStrategy();
            var startDate = new DateTime(2024, 11, 1);
            var todoItemsDueInNextFiveDays = new List<TodoItem>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < Constants.MAX_ITEM_SAME_DUEDAY; j++)
                {
                    todoItemsDueInNextFiveDays.Add(new TodoItem
                    {
                        DueDate = startDate.AddDays(i),
                    });
                }
            }

            // Act & Assert
            Assert.Throws<NoAvailableDaysException>(() => dueDateSettingStrategy.GetDueDate(startDate, todoItemsDueInNextFiveDays));
        }

        [Fact]
        public void FewestTodoItemsDayStrategy_GetDueDate_ReturnsDateWithLeastItems()
        {
            // Arrange
            var dueDateSettingStrategy = new FewestTodoItemsDayStrategy();
            var startDate = new DateTime(2024, 11, 1);
            var todoItemsDueInNextFiveDays = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTime(2024, 11, 1) },
                new TodoItem { DueDate = new DateTime(2024, 11, 1) },
                new TodoItem { DueDate = new DateTime(2024, 11, 2) },
                new TodoItem { DueDate = new DateTime(2024, 11, 3) },
                new TodoItem { DueDate = new DateTime(2024, 11, 3) },
                new TodoItem { DueDate = new DateTime(2024, 11, 4) },
                new TodoItem { DueDate = new DateTime(2024, 11, 4) },
                new TodoItem { DueDate = new DateTime(2024, 11, 5) },
                new TodoItem { DueDate = new DateTime(2024, 11, 5) },
            };

            // Act
            var result = dueDateSettingStrategy.GetDueDate(startDate, todoItemsDueInNextFiveDays);

            // Assert
            Assert.Equal(new DateTime(2024, 11, 2), result);
        }

        [Fact]
        public void FewestTodoItemsDayStrategy_GetDueDate_ThrowsException_WhenNoAvailableDays()
        {
            // Arrange
            var dueDateSettingStrategy = new FewestTodoItemsDayStrategy();
            var startDate = new DateTime(2024, 11, 1);
            var todoItemsDueInNextFiveDays = new List<TodoItem>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < Constants.MAX_ITEM_SAME_DUEDAY; j++)
                {
                    todoItemsDueInNextFiveDays.Add(new TodoItem
                    {
                        DueDate = startDate.AddDays(i),
                    });
                }
            }

            // Act & Assert
            Assert.Throws<NoAvailableDaysException>(() => dueDateSettingStrategy.GetDueDate(startDate, todoItemsDueInNextFiveDays));
        }
    }
}
