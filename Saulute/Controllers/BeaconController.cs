using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saulute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Controllers
{

    [ApiController]
    [Route("supervised/users")]
    public class BeaconController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public BeaconController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("/")]
        public async Task<IActionResult> GetAllBeacons()
        {
            var beacons = await _context.Beacons
                .Include(b => b.Rooms)
                .ToListAsync();

            return Ok(beacons);
        }

        [HttpGet("/{identification}")]
        public async Task<IActionResult> GetBeaconById(string identification)
        {
            var beacons = await _context.Beacons
                .Include(b => b.Rooms)
                .SingleAsync(b => b.Identification == identification);
            return Ok(beacons);
        }

        [HttpPost("/")]
        public async Task<IActionResult> Create([FromBody] Beacon beacon)
        {
            await _context.Beacons.AddAsync(beacon);
            await _context.SaveChangesAsync();
            return Ok(beacon);
        }

        [HttpPost("/{identification}/rooms")]
        public async Task<IActionResult> AddRoom(string identification, [FromBody] Room room)
        {
            var beacon = _context.Beacons
                .Include(b => b.Rooms)
                .Single(b => b.Identification == identification);

            beacon.Rooms.Add(room);

            await _context.SaveChangesAsync();

            return Ok(beacon);
        }
    }
}
