using System.Collections.Generic;
using APIVersioning.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIVersioning.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            // mimiking db operation.

            var users = new List<User>
            {
                new User { Id = 1, Name = "Sully" },
                new User { Id = 2, Name = "Mario" },
                new User { Id = 3, Name = "Emi" }
            };

            return Ok(users);
        }
    }
}