using System;
using System.Collections.Generic;
using APIVersioning.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIVersioning.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            // mimiking db operation.

            var users = new List<UserV2>
            {
                new UserV2 { Id = Guid.NewGuid(), Name = "Sully" },
                new UserV2 { Id = Guid.NewGuid(), Name = "Mario" },
                new UserV2 { Id = Guid.NewGuid(), Name = "Emi" }
            };

            return Ok(users);
        }
    }
}