using Saulute.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

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

        public List<RSI> GetLiveData()
        {
            List<RSI> data = new List<RSI>();

            MySqlConnection conn=new MySqlConnection("host=localhost;user=root;database=projektas;");
            string sql = "select * from sensordata";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                RSI value=new RSI();
                value.Rsi = int.Parse(reader.GetString("rssi"));
                value.IsRequested = reader.GetString("pagalba");
                value.Time= System.DateTime.Parse(reader.GetString("timestamp"));
                data.Add(value);
            }
            return data;
        }


        [HttpGet("beacons")]
        public List<int> GetBeaconIds()
        {
            List<int> data = new List<int>();
            MySqlConnection conn = new MySqlConnection("host=localhost;user=root;database=projektas;");
            string sql = "select distinct svyturioid from kambariudata";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int value;
                value = int.Parse(reader.GetString("svyturioid"));
                data.Add(value);
            }
            return data;
        }

        [HttpGet("beacons/{id}/rooms")]
        public List<Room> GetBeaconRooms(int id)
        {
            List<Room> data = new List<Room>();
            MySqlConnection conn = new MySqlConnection("host=localhost;user=root;database=projektas;");
            string sql = "select * from kambariudata where svyturioid=" + id.ToString();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int roomId = int.Parse(reader.GetString("id"));
                double edge1 = double.Parse(reader.GetString("kampas1"));
                double edge2 = double.Parse(reader.GetString("kampas2"));
                double edge3 = double.Parse(reader.GetString("kampas3"));
                double edge4 = double.Parse(reader.GetString("kampas4"));
                data.Add(new Room {
                    Id = roomId,
                    Corner1 = edge1,
                    Corner2 = edge2,
                    Corner3 = edge3,
                    Corner4 = edge4,
                });
            }
            return data;
        }
    }
}
