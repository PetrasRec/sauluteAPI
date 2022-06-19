﻿using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saulute.Common;
using Saulute.Models;
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


        private sealed class SupervisedUsersData
        {
            public List<SupervisedUser> SupervisedUsers { get; set; }
            public List<RSI> HelpStamps { get; set; }
        }

        public SupervisedUsersController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupervisedUsers(string id)
        {
            var supervisedUsers = await _context.SupervisedUsers
                .Include(supervised => supervised.Watcher)
                .Where(supervised => supervised.Watcher.Id == id)
                .ToListAsync();

            var userRooms = _context
                .UserRooms
                .Include(ur => ur.SupervisedUser)
                .ToList()
                .Where(ur => supervisedUsers.Find(u => u.Id == ur.SupervisedUser.Id) != null);

            var beaconIdx = userRooms.Select(ur => ur.BeaconId.ToString()).ToList();
            var joinedIds = string.Join(",", beaconIdx.ToArray());

            List<RSI> data = new List<RSI>();
            if (joinedIds.Length > 0)
            {
                MySqlDb.GetDataFromSql(
                    String.Format(
                        @"
                    SELECT
                        *
                    FROM 
                        sensordata
                    WHERE
                        svyturioid in ({0})
                    AND
                        DATE(timestamp) = DATE(NOW())
                    AND
                        pagalba=1
                    ",
                    joinedIds), (reader) =>
                {
                    RSI value = new RSI();
                    value.Rsi = int.Parse(reader.GetString("rssi"));
                    value.IsRequested = reader.GetString("pagalba");
                    value.Time = System.DateTime.Parse(reader.GetString("timestamp"));
                    value.BeaconId = int.Parse(reader.GetString("svyturioid"));
                    int ?seniorId = userRooms
                        ?.FirstOrDefault(ur => ur.BeaconId == value.BeaconId)
                        ?.SupervisedUser
                        ?.Id;

                    if (seniorId.HasValue)
                    {
                        value.SupervisedUserId = seniorId.Value;
                    }
                    data.Add(value);
                });
            }

            return Ok(new SupervisedUsersData
            {
                SupervisedUsers = supervisedUsers,
                HelpStamps = data,
            });
        }


        [HttpGet("{id}/info")]
        public ActionResult<dynamic> GetSupervisedUserById(int ?id)
        {
            var supervisedUser = _context.SupervisedUsers
                .Include(supervised => supervised.Watcher)
                .Where(supervised => supervised.Id == id).SingleOrDefault();

            return Ok(supervisedUser);
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> Create(string id, [FromBody] SupervisedUser supervisedUser)
        {
            supervisedUser.Watcher = _context.Users.Where(user => user.Id == id).SingleOrDefault();

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
            var supervisedUser = _context
                .SupervisedUsers
                .Single(u => u.Id == id);

            if (supervisedUser == null)
            {
                return NotFound();
            }

            var rooms = _context.UserRooms
                .Include(ur=>ur.SupervisedUser)
                .Where(ur => ur.SupervisedUser.Id == id);

            _context.UserRooms.RemoveRange(rooms);
            _context.SupervisedUsers.Remove(supervisedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
