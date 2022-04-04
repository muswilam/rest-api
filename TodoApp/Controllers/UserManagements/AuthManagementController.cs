using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TodoApp.Configuration;
using TodoApp.Dtos.Requests.UserManagements;
using TodoApp.Dtos.Responses.Base;
using TodoApp.Dtos.Responses.UserManagement;

namespace TodoApp.Controllers.UserManagements
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagementController(IOptions<JwtConfig> jwtOptions,
                                        UserManager<IdentityUser> userManager)
        {
            _jwtConfig = jwtOptions.Value;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistraionDto user)
        {
            var response = new BaseResponseDto<RegistrationResponseDto>();

            // ToDo: implement registraion.

            var registrationResponse = new RegistrationResponseDto
            {
                Token = "dsdsdssdsdd"
            };

            response.Success = true;
            response.Result = registrationResponse;

            return Ok(response);
        }
    }
}