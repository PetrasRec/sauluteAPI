using Saulute.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Saulute.Common;

namespace Saulute.Controllers
{
    [ApiController]
    [Route("rssi")]
    public class RSSIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("beacons")]
        public List<int> GetBeaconIds()
        {
            List<int> data = new List<int>();
            MySqlDb.GetDataFromSql("select distinct svyturioid from kambariudata", (reader) =>
            {
                int value;
                value = int.Parse(reader.GetString("svyturioid"));
                data.Add(value);
            });
            return data;
        }

        [HttpGet("beacons/{id}/rooms")]
        public List<Room> GetBeaconRooms(int id)
        {
            List<Room> data = new List<Room>();

            string input = "select * from kambariudata where svyturioid=" + id.ToString();
            MySqlDb.GetDataFromSql(input, (reader) =>
            {
                int roomId = int.Parse(reader.GetString("id"));
                double edge1 = double.Parse(reader.GetString("kampas1"));
                double edge2 = double.Parse(reader.GetString("kampas2"));
                double edge3 = double.Parse(reader.GetString("kampas3"));
                double edge4 = double.Parse(reader.GetString("kampas4"));
                data.Add(new Room
                {
                    Id = roomId,
                    Corner1 = edge1,
                    Corner2 = edge2,
                    Corner3 = edge3,
                    Corner4 = edge4,
                });
            });
       
            return data;
        }
    }
}
