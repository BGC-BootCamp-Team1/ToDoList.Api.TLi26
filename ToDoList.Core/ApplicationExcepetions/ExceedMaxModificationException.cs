namespace ToDoList.Core.ApplicationExcepetions
{
    public class ExceedMaxModificationException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "You have reached the maximum number of modifications for today. " +
            "Please try again tomorrow.";

        public ExceedMaxModificationException() : base(DefaultMessage)
        {
        }
    }
}
