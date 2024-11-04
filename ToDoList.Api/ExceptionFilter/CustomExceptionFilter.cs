using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using ToDoList.Core.ApplicationExcepetions;

namespace ToDoList.Api.ExceptionFilter
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ExceedMaxModificationException ||
                context.Exception is ExceedMaxTodoItemsPerDueDateException ||
                context.Exception is InvalidDueDateSettingOptionException ||
                context.Exception is NoAvailableDaysException ||
                context.Exception is TooEarlyDueDateException
                )
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else 
            {
                context.Result = new ObjectResult("Unknow Error")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            context.ExceptionHandled = true;
        }
    }
}
