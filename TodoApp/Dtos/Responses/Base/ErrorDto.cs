using System.Collections.Generic;

namespace TodoApp.Dtos.Responses.Base
{
    public class ErrorDto
    {
        public bool IsValidationError { get; set; }

        public string Field { get; set; }

        public List<string> Codes { get; set; } = new List<string>();
    }
}