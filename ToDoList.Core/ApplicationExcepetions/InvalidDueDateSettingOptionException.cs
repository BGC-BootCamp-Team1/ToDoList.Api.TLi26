namespace ToDoList.Core.ApplicationExcepetions
{
    public class InvalidDueDateSettingOptionException : Exception
    {
        private static readonly string DefaultMessage = "Invalid due date setting option";

        public InvalidDueDateSettingOptionException() : base(DefaultMessage)
        {
        }
    }
}
