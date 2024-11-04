namespace ToDoList.Core.ApplicationExcepetions
{
    public class ExceedMaxTodoItemsPerDueDateException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "You have reached the maximum number of todo items for one due date. " +
            "Please try again.";

        public ExceedMaxTodoItemsPerDueDateException() : base(DefaultMessage)
        {
        }
    }
}
