namespace ToDoList.Core.ApplicationExcepetions
{
    public class NoAvailableDaysException : Exception
    {
        private static readonly string DefaultMessage = "No available days in the next 5 days.";

        public NoAvailableDaysException() : base(DefaultMessage)
        {
        }
    }
}
