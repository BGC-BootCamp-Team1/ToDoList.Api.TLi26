﻿namespace ToDoList.Core.DueDateSettingStrategy
{
    public interface IDueDateSettingStrategy
    {
        public DateTime GetDueDate(DateTime startDate, List<CoreTodoItem> itemsDueInNextFiveDays);
    }
}
