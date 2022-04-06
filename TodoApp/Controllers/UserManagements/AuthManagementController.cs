using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Configuration;
using TodoApp.Data;
using TodoApp.Dtos.Requests.UserManagement;
using TodoApp.Dtos.Responses.Base;
using TodoApp.Dtos.Responses.UserManagement;
using TodoApp.ErrorCodes;
using TodoApp.Extensions;
using TodoApp.Models;

namespace TodoApp.Controllers.UserManagements
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApiDbContext _context;

        public AuthManagementController(IOptions<JwtConfig> jwtOptions,
                                        UserManager<IdentityUser> userManager,
                                        TokenValidationParameters tokenValidationParameters,
                                        ApiDbContext context)
        {
            _jwtConfig = jwtOptions.Value;
            _userManager = userManager;
            this._tokenValidationParameters = tokenValidationParameters;
            _context = context;
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
                TokenInfo = await GenerateJwtTokenAsync(newUser)
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
                TokenInfo = await GenerateJwtTokenAsync(userExist)
            };

            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto token)
        {
            var response = new BaseResponseDto<TokenResponseDto>();

            var verifyTokenResult = await VerifyToken(token);

            if (verifyTokenResult is null)
            {
                response.AddBusinessError(BusinessErrorCode.InvalidToken);
                return BadRequest(response);
            }

            if (verifyTokenResult.Errors.Any())
            {
                // ToDo: add extension method (addError).
                verifyTokenResult.Errors.ForEach(e => response.Errors.Add(e));
                return BadRequest(response);
            }

            response.Result = verifyTokenResult.Result;
            return Ok(response);
        }

        #region Helpers.
        private async Task<TokenResponseDto> GenerateJwtTokenAsync(IdentityUser user)
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
                Expires = DateTime.UtcNow.AddSeconds(30),

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

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                IsUsed = false,
                IsRevoked = false,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRandomString(35) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string GenerateRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        private async Task<BaseResponseDto<TokenResponseDto>> VerifyToken(TokenRequestDto token)
        {
            var response = new BaseResponseDto<TokenResponseDto>();

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // 01-validate token is a propper jwt token formatting.
                var claimsPrincipal = jwtTokenHandler.ValidateToken(token.Token, _tokenValidationParameters, out var validatedToken);

                // 02-validate token has been encrypted using the encryption that we have specified.
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var isSameEncryption = jwtSecurityToken.Header
                                                           .Alg
                                                           .Equals(SecurityAlgorithms.HmacSha256,
                                                                   StringComparison.InvariantCultureIgnoreCase);

                    if (!isSameEncryption)
                        return null;
                }

                // 03-validate token expiry date.
                var utcLongExpiryDate = long.Parse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcLongExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    response.AddBusinessError(BusinessErrorCode.TokenNotExpiredYet);
                    return response;
                }

                //04-validate actual token exists in database.
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token.Token);

                if (storedToken is null)
                {
                    response.AddBusinessError(BusinessErrorCode.TokenNotExist);
                    return response;
                }

                //05-validate token is used before.
                if (storedToken.IsUsed)
                {
                    response.AddBusinessError(BusinessErrorCode.TokenHasBeenUsed);
                    return response;
                }

                //06-validate token is revoked.
                if (storedToken.IsRevoked)
                {
                    response.AddBusinessError(BusinessErrorCode.TokenHasBeenRevoked);
                    return response;
                }

                //07-validate Jti mathes jwtId of the refresh token in database.
                var jti = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    response.AddBusinessError(BusinessErrorCode.TokenDoesNotMatch);
                    return response;
                }

                // update current token.
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // generate new token.
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                response.Result = await GenerateJwtTokenAsync(user);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // utc time is an integer (long) number of seconds from the 1970/1/1 till now. 
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeVal.AddSeconds(unixTimeStamp).ToLocalTime();
        }
        #endregion
    }
}