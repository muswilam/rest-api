using System.Collections.Generic;

namespace TodoApp.Dtos.Responses.Base
{
    public class BaseResponseDto<T>
    {
        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public T Result { get; set; }
    }
}