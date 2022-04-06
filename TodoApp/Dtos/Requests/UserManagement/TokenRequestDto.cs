using System.ComponentModel.DataAnnotations;
using TodoApp.ErrorCodes;

namespace TodoApp.Dtos.Requests.UserManagement
{
    public class TokenRequestDto
    {
        [Required(ErrorMessage = ValidationErrorCode.Required)]
        public string Token { get; set; }
        
        [Required(ErrorMessage = ValidationErrorCode.Required)]
        public string RefreshToken { get; set; }
    }
}