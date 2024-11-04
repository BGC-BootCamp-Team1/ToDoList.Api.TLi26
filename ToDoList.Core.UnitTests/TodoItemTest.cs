using ToDoList.Core.ApplicationExcepetions;

namespace ToDoList.Core.Test
{
    public class TodoItemTest
    {
        [Fact]
        public void Modify_ShouldThrowException_WhenFourthModification()
        {
            DateTime today = DateTime.Today;
            List<Modification> threeTodayModifications =
            [
                new Modification(today.AddHours(9)),
            new Modification(today.AddHours(12)),
            new Modification(today.AddHours(14))
            ];
            TodoItem todoItem = new TodoItem("test", threeTodayModifications, today.AddDays(7));
            Assert.Throws<ExceedMaxModificationException>(() => todoItem.ModifyDescription("TEST"));
        }

        [Fact]
        public void Modify_ShouldModify_WhenThirdModification()
        {
            DateTime today = DateTime.Today;
            List<Modification> twoTodayModifications =
            [
                new Modification(today.AddDays(-1).AddHours(12)),
            new Modification(today.AddHours(12))
            ];
            TodoItem todoItem = new TodoItem("test", twoTodayModifications, today.AddDays(7));
            todoItem.ModifyDescription("TEST_MODIFY");
            Assert.Equal("TEST_MODIFY", todoItem.Description);
        }

        [Fact]
        public void Modify_ShouldModify_WhenFirstModification()
        {
            TodoItem todoItem = new TodoItem("test", DateTime.Today.AddDays(7));

            todoItem.ModifyDescription("TEST_MODIFY");
            Assert.Equal("TEST_MODIFY", todoItem.Description);
        }
    }
}