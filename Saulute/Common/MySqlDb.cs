using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Common
{
    public static class MySqlDb
    {
        private static string connectionStr = "host=localhost;user=root;database=projektas;password=root;";

        public static void GetDataFromSql(string sqlQuery, Action<MySqlDataReader> fillData)
        {
            MySqlConnection conn = new MySqlConnection(connectionStr);
            string sql = sqlQuery;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                fillData(reader);
            }

            conn.Close();
        }
    }
}
