using System.Collections.Generic;
using TodoApp.Dtos.Responses.Base;

namespace TodoApp.Extensions
{
    public static class ErrorExtension
    {
        public static void AddValidationError<T>(this BaseResponseDto<T> response, string field, string errorCode)
        {
            response.Errors.Add(new ErrorDto
            {
                IsValidationError = true,
                Field = field,
                Codes = new List<string> { errorCode }
            });
        }
    }
}