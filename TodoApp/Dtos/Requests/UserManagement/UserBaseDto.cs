using System.ComponentModel.DataAnnotations;
using TodoApp.ErrorCodes;

namespace TodoApp.Dtos.Requests.UserManagements
{
    public class UserBaseDto
    {
        [Required(ErrorMessage = ValidationErrorCode.Required)]
        public string UserName { get; set; }

        [Required(ErrorMessage = ValidationErrorCode.Required)]
        [EmailAddress(ErrorMessage = ValidationErrorCode.InvalidFormat)]
        public string Email { get; set; }

        [Required(ErrorMessage = ValidationErrorCode.Required)]
        public string Password { get; set; }
    }
}