using System.Collections.Generic;
using APIVersioning.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIVersioning.Controllers.V1
{
    [ApiController]
    [Route("api/users")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            // mimiking db operation.

            var users = new List<UserV1>
            {
                new UserV1 { Id = 1, Name = "Sully" },
                new UserV1 { Id = 2, Name = "Mario" },
                new UserV1 { Id = 3, Name = "Emi" }
            };

            return Ok(users);
        }
    }
}