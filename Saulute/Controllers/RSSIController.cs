using Saulute.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Saulute.Controllers
{
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
    }
}
