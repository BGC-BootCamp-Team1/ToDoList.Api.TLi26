using ToDoList.Core.ApplicationExcepetions;

namespace ToDoList.Core.DueDateSettingStrategy
{
    public static class DueDateSetter
    {
        public static DateTime AutoSetDueDate(
            List<TodoItem> itemsDueInNextFiveDays,
            DueDateSettingOption dueDateSettingOption)
        {
            var strategy = SetStrategy(dueDateSettingOption);
            return strategy.GetDueDate(DateTime.Today, itemsDueInNextFiveDays);
        }

        public static DateTime ValidUserDueDate(DateTime userProvidedDueDate, long count)
        {
            if (userProvidedDueDate <= DateTime.Now.Date)
            {
                throw new TooEarlyDueDateException();
            }
            if (count >= Constants.MAX_ITEM_SAME_DUEDAY)
            {
                throw new ExceedMaxTodoItemsPerDueDateException();
            }
            return userProvidedDueDate;
        }


        private static IDueDateSettingStrategy SetStrategy(DueDateSettingOption dueDateSettingOption)
        {
            return dueDateSettingOption switch
            {
                DueDateSettingOption.SelectFirstAvailableDay => new FirstAvailableDayStrategy(),
                DueDateSettingOption.SelectFewestTodoItemsDay => new FewestTodoItemsDayStrategy(),
                _ => throw new InvalidDueDateSettingOptionException(),
            };
        }
    }
}
