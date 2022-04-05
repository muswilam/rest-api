using System.Collections.Generic;
using TodoApp.ErrorCodes;

namespace TodoApp.Dtos.Responses.Base
{
    public class BaseResponseDto<T>
    {
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        public T Result { get; set; }
    }
}