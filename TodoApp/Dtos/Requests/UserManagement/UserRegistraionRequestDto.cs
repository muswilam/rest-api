using System.ComponentModel.DataAnnotations;
using TodoApp.ErrorCodes;

namespace TodoApp.Dtos.Requests.UserManagement
{
    public class UserRegistraionRequestDto : UserBaseDto
    {
        [Required(ErrorMessage = ValidationErrorCode.Required)]
        public string UserName { get; set; }
    }
}