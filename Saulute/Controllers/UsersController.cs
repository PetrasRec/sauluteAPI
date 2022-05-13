using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addicted.Service;
using Microsoft.AspNetCore.Authorization;
using Addicted.Models;
using Addicted.Config;

namespace Addicted.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public ActionResult<dynamic> GetAllUsers()
        {
            var users = usersService.GetAllUsers().Select(async u =>
            {
                string roleName = await usersService.GetUserRoleId(u);
                var newU = new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    RoleId = roleName,
                };
                return newU;
            }).Select(u=>u.Result);
            return Ok(users);
        }

        [HttpGet("profile")]
        [Authorize()]
        public ActionResult<dynamic> GetUser()
        {
            var user = usersService.GetUserByEmail(User.Identity.Name);
            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize()]
        public ActionResult<dynamic> GetUserById(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var user = usersService.GetUserById(id);
            return Ok(user);
        }


        [HttpPut("{id}")]
        [Authorize()]
        public async Task<dynamic> UpdateUserByID(string ?id, [FromBody] UserModel newData)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var updatedUser = await usersService.UpdateUserByID(id, newData);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [HttpPost]
        public async Task<dynamic> AddNewUser([FromBody] UserModel user)
        {
            var addedUser = await usersService.AddNewUser(user);
            if (addedUser == null)
            {
                return BadRequest();
            }

            return Ok(addedUser);
        }

        [HttpPost("register")]
        public async Task<dynamic> RegisterNewUser([FromBody] UserModel user)
        {
            var addedUser = await usersService.RegisterNewUser(user);
            if (addedUser == null)
            {
                return BadRequest();
            }
            return Ok(addedUser);
        }
    }
}
