using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saulute.Common;
using Saulute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Controllers
{
    [ApiController]
    [Route("user/rooms")]
    public class UserRoomsController : Controller
    {
        private readonly AuthenticationContext _context;

        public IActionResult Index()
        {
            return View();
        }
        sealed class UserRoomData
        {
            public UserRoom UserRoom { get; set; }

            // Put anything here.
            public bool IsInside { get; set; }
            public double Distance { get; set; }
        }
        public UserRoomsController(AuthenticationContext context)
        {
            _context = context;
        }

        [HttpGet("owner/{userId}/live")]
        public async Task<IActionResult> GetLiveData(string userId)
        {
            var userRooms = await _context
               .UserRooms
               .Include(ur => ur.Owner)
               .Where(ur => ur.Owner.Id == userId)
               .ToListAsync();

            List<RSI> data = new List<RSI>();
            MySqlDb.GetDataFromSql(@"
                SELECT 
                    *
                FROM
                    projektas.sensordata s1
                WHERE
                    s1.timestamp
                    =
                    (
	                    SELECT
                            MAX(s2.timestamp)
                        FROM
                            projektas.sensordata s2
                        WHERE
                            s1.svyturioid=s2.svyturioid
                    )
                AND
                    pagalba=0
            ", (reader) =>
            {
                RSI value = new RSI();
                value.Rsi = int.Parse(reader.GetString("rssi"));
                value.IsRequested = reader.GetString("pagalba");
                value.Time = System.DateTime.Parse(reader.GetString("timestamp"));
                value.BeaconId = int.Parse(reader.GetString("svyturioid"));
                data.Add(value);
            });
            Random rnd = new Random();
            List<UserRoomData> userRoomData = new List<UserRoomData>();
            foreach(var userRoom in userRooms)
            {
                var rsiData = data.SingleOrDefault(rsi => rsi.BeaconId == userRoom.BeaconId);
                if (rsiData == null)
                {
                    continue;
                }
                double rssi = rsiData.Rsi;

                double measuredPower = 10;
                double N = 6;

                double distance = Math.Pow(10, (measuredPower - rssi) / (10 * N)) + rnd.Next(1, 10);

                // something, deep MATH
                userRoomData.Add(new UserRoomData
                {
                    UserRoom = userRoom,
                    Distance = Math.Round(distance, 1),
                });
            }
            if (userRoomData.Count > 0)
            {
                // REMOVE THIS AFTER DOING THE MATH
                var random_idx = rnd.Next(0, userRoomData.Count);
                userRoomData[random_idx].IsInside = true;
            }
            return Ok(userRoomData);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userRoom = _context.UserRooms.FindAsync(id);
            if (userRoom == null)
            {
                return NotFound();
            }

            _context.UserRooms.Remove(userRoom.Result);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("owner/{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var userRooms = await _context
                .UserRooms
                .Include(ur => ur.Owner)
                .Where(ur => ur.Owner.Id == userId)
                .ToListAsync();

            return Ok(userRooms);
        }

        [HttpPost("owner/{userId}")]
        public async Task<IActionResult> Create(string userId, [FromBody] UserRoom userRoom)
        {
            var user = _context.GetAllUsers().Single(u => u.Id == userId);

            userRoom.Owner = user;

            await _context.UserRooms.AddAsync(userRoom);
            await _context.SaveChangesAsync();
            return Ok(userRoom);
        }
    }
}
