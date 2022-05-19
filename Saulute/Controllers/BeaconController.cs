using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saulute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Saulute.Controllers
{

    [ApiController]
    [Route("beacons")]
    public class BeaconController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public BeaconController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllBeacons()
        {
            var beacons = _context.UserBeacons.Include(supervised => supervised.User)
                .Where(supervised => supervised.User != null); ;
            return Ok(beacons);
        }

        [HttpGet("{userId}/users")]
        public async Task<IActionResult> GetUserBeacons(string userId)
        {
            var beacons = _context.UserBeacons
               .Include(supervised => supervised.User)
               .Where(supervised => supervised.User.Id == userId);

            return Ok(beacons);
        }


        [HttpGet("{identification}")]
        public async Task<IActionResult> GetBeaconById(string identification)
        {
            var beacons = await _context.Beacons
                .Include(b => b.Rooms)
                .SingleAsync(b => b.Identification == identification);
            return Ok(beacons);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] Beacon beacon)
        {
            await _context.Beacons.AddAsync(beacon);
            await _context.SaveChangesAsync();
            return Ok(beacon);
        }

        [HttpPost("{userId}/users")]
        public async Task<IActionResult> CreateUserBeacons(string userId, [FromBody] UserBeacon beacon)
        {
            beacon.User = _context.Users.Where(user => user.Id == userId).SingleOrDefault();

            await _context.UserBeacons.AddAsync(beacon);
            await _context.SaveChangesAsync();
            return Ok(beacon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromBody] UserBeacon beacon)
        {
            var dbBeacon = await _context.UserBeacons.FindAsync(beacon.BeaconId);
            dbBeacon.BeaconId = beacon.BeaconId;
            dbBeacon.User.UserName = beacon.User.UserName;

            await _context.SaveChangesAsync();
            return Ok(dbBeacon);
        }



        [HttpPost("{identification}/rooms")]
        public async Task<IActionResult> AddRoom(string identification, [FromBody] Room room)
        {
            var beacon = _context.Beacons
                .Include(b => b.Rooms)
                .Single(b => b.Identification == identification);

            beacon.Rooms.Add(room);

            await _context.SaveChangesAsync();
            

            return Ok(beacon);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var beacs = _context.Beacons.FindAsync(id);
            if (beacs == null)
            {
                return NotFound();
            }

            _context.Beacons.Remove(beacs.Result);
            await _context.SaveChangesAsync();
            return Ok();
        }


        //[HttpPost("/{identification}/live-data")]
        //public async Task<IActionResult> AddBeaconLiveData(string identification, [FromBody] RSI liveData,string help)
        //{
        //  //  var beacon = _context.Beacons.Where(b => b.Identification == identification);
        //  //  string indentification = beacon.Identification;
        //  //  liveData.Beacon = identification;
        //    liveData.IsRequested = help;
        //    var rsi = liveData;   
            
        //    return Ok(rsi);
        //}

        //public void Subscribe()
        //{
        //    client = new MqttClient("broker.hivemq.com");
        //    client.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
        //    client.Subscribe(new string[] { "Saulute/Message" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        //    client.Connect("Saulute");

        //}

        //private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        //{
        //    var message=Encoding.UTF8.GetString(e.Message);
        //    RSI rsi = new RSI();
        //    rsi.Rsi= int.Parse(message);
        //   // _context.Rsi.Add(rsi);
        //}


    }
}
