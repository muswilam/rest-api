using System.Collections.Generic;
using TodoApp.ErrorCodes;

namespace TodoApp.Dtos.Responses.Base
{
    public class BaseResponseDto<T>
    {
        public bool Success { get; set; }

        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        public T Result { get; set; }
    }
}