using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Controllers
{
    [ApiController]
    [Route("supervised/users")]
    public class SupervisedUsersController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public SupervisedUsersController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{user_id}")]
        public async Task<IActionResult> Index(string userId)
        {
            var supervisedUsers = _context.SupervisedUsers
                .Include(supervised => supervised.Watcher)
                .Where(supervised => supervised.Watcher.Id == userId);

            return Ok(supervisedUsers);
        }


        [HttpGet("{id}/info")]
        public ActionResult<dynamic> GetSupervisedUserById(int ?id)
        {
            var supervisedUser = _context.SupervisedUsers
                .Include(supervised => supervised.Watcher)
                .Where(supervised => supervised.Id == id).SingleOrDefault();

            return Ok(supervisedUser);
        }


        [HttpPost("{user_id}")]
        public async Task<IActionResult> Create(string userId, [FromBody] SupervisedUser supervisedUser)
        {
            supervisedUser.Watcher = _context.Users.Where(user => user.Id == userId).SingleOrDefault();

            await _context.SupervisedUsers.AddAsync(supervisedUser); 
            await _context.SaveChangesAsync();
            return Ok(supervisedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] SupervisedUser supervisedUser)
        {
            var dbSupervisedUser = await _context.SupervisedUsers.FindAsync(id);
            dbSupervisedUser.Name = supervisedUser.Name;
            dbSupervisedUser.Surname = supervisedUser.Surname;

            await _context.SaveChangesAsync();
            return Ok(dbSupervisedUser);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var supervisedUser = _context.SupervisedUsers.FindAsync(id);
            if (supervisedUser == null)
            {
                return NotFound();
            }

            _context.SupervisedUsers.Remove(supervisedUser.Result);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
