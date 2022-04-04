using System.ComponentModel.DataAnnotations;

namespace TodoApp.Dtos.Requests.UserManagements
{
    public class UserRegistraionDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}