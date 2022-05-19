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

        sealed class HelpData
        {
            public DateTime CallTime { get; set; }
            public UserRoom UserRoom { get; set; }
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
            List<UserRoomData> userRoomData = new List<UserRoomData>();

            double closestDist = 100000000;
            int best_idx = -1;
            int index = -1;
            foreach(var userRoom in userRooms)
            {
                index++;
                var rsiData = data.SingleOrDefault(rsi => rsi.BeaconId == userRoom.BeaconId);
                if (rsiData == null)
                {
                    continue;
                }
                double measuredPower = 10;
                double N = 6;

                Func<double, double> getDist = (rssi) => Math.Pow(10, (measuredPower - rssi) / (10 * N));

                // distances  from the corners to the beacon
                double corner1Dist = getDist(userRoom.Corner1);
                double corner2Dist = getDist(userRoom.Corner2);
                double corner3Dist = getDist(userRoom.Corner3);
                double corner4Dist = getDist(userRoom.Corner4);

                double longestDist = new double[] { corner1Dist, corner2Dist, corner3Dist, corner4Dist }
                                        .ToList()
                                        .Max();

                // longestDist is the radius of the circle
                // The distance from the user to the beacon
                double userDist = getDist(rsiData.Rsi);

                double diff = longestDist - userDist;
                if (diff > 0)
                {
                    if (closestDist > userDist)
                    {
                        closestDist = userDist;
                        best_idx = index;
                    }
                }
                userRoomData.Add(new UserRoomData
                {
                    UserRoom = userRoom,
                    Distance = Math.Round(userDist, 1),
                });
            }


            if (best_idx != -1)
            {
                userRoomData[best_idx].IsInside = true;
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
