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

        [HttpGet("owner/{userId}/live/help")]
        public async Task<IActionResult> GetLiveHelpData(int userId)
        {
            var userRooms = await _context
              .UserRooms
              .Include(ur => ur.SupervisedUser)
              .Where(ur => ur.SupervisedUser.Id == userId)
              .ToListAsync();
            var beaconIdx = userRooms.Select(ur => ur.BeaconId.ToString()).ToList();
            var joinedIds = string.Join(",", beaconIdx.ToArray());

            List<RSI> data = new List<RSI>();

            List<HelpData> helpData = new List<HelpData>();
            if (joinedIds.Length > 0)
            {
                MySqlDb.GetDataFromSql(String.Format(
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
                    data.Add(value);
                });
                double closestDist = 100000000;
                int best_idx = -1;
                int index = -1;
                foreach (var rsiData in data)
                {
                    index++;

                    var userRoom = userRooms.FirstOrDefault(userRoom => userRoom.BeaconId == rsiData.BeaconId);
                    if (userRoom == null)
                    {
                        continue;
                    }
                    double measuredPower = -65;
                    double N = 2;

                    double absRssi = Math.Abs(rsiData.Rsi);
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
                    if (diff >= 0)
                    {
                        helpData.Add(new HelpData
                        {
                            UserRoom = userRoom,
                            CallTime = rsiData.Time,
                        });
                    }
                    else
                    {
                        helpData.Add(new HelpData
                        {
                            UserRoom = null,
                            CallTime = rsiData.Time,
                        });
                    }

                }
            }
            return Ok(helpData);
        }

        [HttpGet("owner/{userId}/live")]
        public async Task<IActionResult> GetLiveData(int userId)
        {
            var userRooms = await _context
               .UserRooms
               .Include(ur => ur.SupervisedUser)
               .Where(ur => ur.SupervisedUser.Id == userId)
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
                    rssi!=0
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
                var rsiData = data.FirstOrDefault(rsi => rsi.BeaconId == userRoom.BeaconId);
                if(rsiData == null || rsiData.Time < DateTime.Now.AddSeconds(-10))
                {
                    userRoomData.Add(new UserRoomData
                    {
                        UserRoom = userRoom,
                        Distance = -1,
                    });
                    continue;
                }
                double measuredPower = -65;
                double N = 2;

                double absRssi = Math.Abs(rsiData.Rsi);
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
                if (diff >= 0)
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
        public async Task<IActionResult> Get(int userId)
        {
            var userRooms = await _context
                .UserRooms
                .Include(ur => ur.SupervisedUser)
                .Include(ur => ur.Owner)
                .Where(ur => ur.SupervisedUser.Id == userId)
                .ToListAsync();

            return Ok(userRooms);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UserRoom newRoom)
        {
            var userRoom = await _context
                .UserRooms
                .FindAsync(id);

            userRoom.Name = newRoom.Name;
            await _context.SaveChangesAsync();
            return Ok(userRoom);
        }

        [HttpPost("owner/{userId}/supervisedUser/{supervisedId}")]
        public async Task<IActionResult> Create(string userId, int supervisedId, [FromBody] UserRoom userRoom)
        {
            var user = _context.GetAllUsers().Single(u => u.Id == userId);
            var supervisedUser = _context.SupervisedUsers.Single(u => u.Id == supervisedId);

            userRoom.Owner = user;
            userRoom.SupervisedUser = supervisedUser;

            await _context.UserRooms.AddAsync(userRoom);
            await _context.SaveChangesAsync();
            return Ok(userRoom);
        }
    }
}
