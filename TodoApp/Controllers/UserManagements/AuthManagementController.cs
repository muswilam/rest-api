using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Configuration;
using TodoApp.Dtos.Requests.UserManagement;
using TodoApp.Dtos.Responses.Base;
using TodoApp.Dtos.Responses.UserManagement;
using TodoApp.ErrorCodes;
using TodoApp.Extensions;

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
        public async Task<IActionResult> Register([FromBody] UserRegistraionRequestDto user)
        {
            var response = new BaseResponseDto<RegistrationResponseDto>();

            var emailExist = await _userManager.FindByEmailAsync(user.Email);
            var usernameExist = await _userManager.FindByNameAsync(user.UserName);

            // business validation.

            if (emailExist is not null)
            {
                response.AddValidationError(nameof(user.Email), ValidationErrorCode.AlreadyExist);
            }

            if (usernameExist is not null)
            {
                response.AddValidationError(nameof(user.UserName), ValidationErrorCode.AlreadyExist);
            }

            if (response.Errors.Any())
                return BadRequest(response);

            var newUser = new IdentityUser
            {
                Email = user.Email,
                UserName = user.UserName
            };

            var userCreatedResult = await _userManager.CreateAsync(newUser, user.Password);

            if (!userCreatedResult.Succeeded)
            {
                response.ValidateIdentity(userCreatedResult.Errors);

                return BadRequest(response);
            }

            response.Result = new RegistrationResponseDto
            {
                Token = GenerateJwtToken(newUser)
            };

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user)
        {
            var response = new BaseResponseDto<LoginResponseDto>();

            var userExist = await _userManager.FindByEmailAsync(user.Email);

            if (userExist is null)
            {
                response.AddBusinessError(BusinessErrorCode.InvalidLoginCredentials);
                return BadRequest(response);
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(userExist, user.Password);

            if (!isCorrectPassword)
            {
                response.AddBusinessError(BusinessErrorCode.InvalidLoginCredentials);
                return BadRequest(response);
            }

            response.Result = new LoginResponseDto
            {
                Token = GenerateJwtToken(userExist)
            };

            return Ok(response);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // encodes all the characters in the 'secert' into a sequence of bytes.
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // descriptor: is a class contains claims.
            // claims: are the information that we define within jwt 
            // to be able to read certain variables within it
            // like email, id, etc..
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // utilizing the claims.
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    
                    // Jti: is an id that gonna be used to utilize the refresh token functionality.
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(6),

                // SigningCredentials: defines what type of algorithm that gonna be used 
                // for encrypting this token.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // create a json web token. (jwt)
            // returns JwtSecurityToken
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            // serialize a JwtSecurityToken into a jwt in compact serialization format.
            // returns string. (JWE or JWS)
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}