namespace ToDoList.Core.ApplicationExcepetions
{
    public class TooEarlyDueDateException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "The due date is too early. " +
            "Please try again.";

        public TooEarlyDueDateException() : base(DefaultMessage)
        {
        }
    }
}
