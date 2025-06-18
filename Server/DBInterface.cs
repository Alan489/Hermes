using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Hermes.Shared;
using Hermes.Server.Speaker;
using MySqlX.XDevAPI.Common;
using System.Collections;
using System.Reflection.PortableExecutable;
using System.Net.Http;
using System.Web;

namespace Hermes.Server
{
    public static class DBInterface
    {
        private static string connectionString;
        private static MySqlConnection connectiona;
        private static HttpClient _http;
        
        public static bool connect(string _connString, HttpClient http)
        {
            connectionString = _connString;
            _http = http;
            doConnection();
            return true;
        }

        private static bool doConnection()
        {
            try
            {
                connectiona = new MySqlConnection(connectionString);
                connectiona.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error | Failed to connect.");
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            return true;
        }

        public static void Close()
        {
            connectiona.Close();
            Environment.Exit(0);
        }

        private async static Task populateCoordinates()
        {
            string query = "SELECT incident_id, location " +
                "FROM incidents " +
                "WHERE lat IS NULL AND " +
                "disposition IS NULL";

            var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.IsDBNull(1))
                    continue;

                try
                {
                 List<Location>? locations =
                                await _http.GetFromJsonAsync<List<Location>?>
                                ("https://geocode.maps.co/search?q=" +
                                HttpUtility.UrlEncode(reader.GetString(1)) +
                                "&api_key=662c010d1e78e950019283hra89b83f");


                 if (locations == null || locations.Count == 0)
                  continue;

                 string query1 = "UPDATE incidents " +
                 $"SET lat = {locations[0].lat}, lon = {locations[0].lon} " +
                 $"WHERE incident_id IN ({reader.GetInt32(0)})";

                 var connection1 = new MySqlConnection(connectionString);
                 connection1.Open();
                 var cmd1 = new MySqlCommand(query1, connection1);
                 var reader1 = cmd1.ExecuteReader();
                 reader1.Close();
                 connection1.Close();

                 Thread.Sleep(1000); //Api is only 1/sec lol
                } catch (Exception ex) {
                }
                
            }

            reader.Close();
            connection.Close();
        }

        public static List<string[]> getCallLocations()
        {
            List<string[]> ret = new List<string[]>();
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            List<Assignment> result = new List<Assignment>();

            string query = "SELECT inc.lat, inc.lon " +
                "FROM incidents AS inc " +
                "WHERE inc.disposition IS NULL";

            var cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                    ret.Add(new string[]{reader.GetString(0), reader.GetString(1)});
            }
            reader.Close();
            connection.Close();
            return ret;
        }

        public async static Task<List<Assignment>> getCurrentAssignments()
        {
            await populateCoordinates();
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            List<Assignment> result = new List<Assignment>();

            string query = "SELECT ass.uid, ass.unit, ass.dispatch_time, inc.call_type, inc.call_details, inc.location, " +
                "lock_id, inc.incident_id, ass.arrival_time, ass.transport_time, ass.transportdone_time, ass.cleared_time, " +
                "inc.unit_number, inc.lat, inc.lon " +
                "FROM incident_units AS ass " +
                "INNER JOIN incidents AS inc ON ass.incident_id = inc.incident_id " +
                "INNER JOIN units ON ass.unit = units.unit " +
                "LEFT JOIN incident_locks ON incident_locks.incident_id = inc.incident_id " +
                "WHERE ass.cleared_time IS NULL AND units.type = 'Unit'";

            var cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Assignment ass = new Assignment();
                if (!reader.IsDBNull(0))
                    ass.uid     = reader.GetInt32(0);
                if (!reader.IsDBNull(1))
                    ass.unit    = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    ass.timeout = reader.GetDateTime(2);
                if (!reader.IsDBNull(3))
                    ass.type    = reader.GetString(3);
                if (!reader.IsDBNull(4))
                    ass.desc    = reader.GetString(4);
                if (!reader.IsDBNull(5))
                    ass.address = reader.GetString(5);
                if (!reader.IsDBNull(8))
                    ass.arrival_time = reader.GetDateTime(8);
                if (!reader.IsDBNull(9))
                    ass.transport_time = reader.GetDateTime(9);
                if (!reader.IsDBNull(10))
                    ass.transportdone_time = reader.GetDateTime(10);
                if (!reader.IsDBNull(11))
                    ass.cleared_time = reader.GetDateTime(11);
                if (!reader.IsDBNull(12))
                    ass.location_unit = reader.GetString(12);
                if (!reader.IsDBNull(13))
                    ass.lat = reader.GetString(13);
                if (!reader.IsDBNull(14))
                    ass.lon = reader.GetString(14);

                NewIncidentPage.addUnit(reader.GetInt32(7), ass.unit);
                NewIncidentPage.addDetails(reader.GetInt32(7), ass.desc);
                NewIncidentPage.addLocation(reader.GetInt32(7), ass.address);
                if (reader.IsDBNull(6))
                {
                    NewIncidentPage.setLock(reader.GetInt32(7), false);
                    ass.locked = false;
                }
                    
                else
                {
                    ass.locked = true;
                    NewIncidentPage.setLock(reader.GetInt32(7), true);
                }
                   

                result.Add(ass);
            }
            reader.Close();

            List<int> seen = new List<int>();
            List<Assignment> assignmentRemoval = new List<Assignment>();

            foreach (Assignment ass in result)
                if (seen.Contains(ass.uid))
                    assignmentRemoval.Add(ass);
                else
                    seen.Add(ass.uid);

            foreach (Assignment ass in assignmentRemoval)
                result.Remove(ass);

            query = "SELECT unit, personnel FROM units WHERE status != 'Out Of Service' AND type='Unit'";

            cmd = new MySqlCommand(query, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Assignment? ass = result.Find(a => a.unit == reader.GetString(0));
                if (ass != null)
                    ass.personnel = reader.GetString(1);
                else
                {
                    ass = new Assignment() {uid = -1, unit = reader.GetString(0), personnel = reader.GetString(1) };
                    result.Add(ass);
                }
                    
            }
            reader.Close();
            connection.Close();
            return result;
        }

        public static async Task updateUnit(string ass1, string col, string unit)
        {
            await populateCoordinates();
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = $"UPDATE incident_units SET {col} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE uid = {ass1}";

            var cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            { 

            }
            reader.Close();

            if (col != "cleared_time")
            {
                connection.Close();
                return;
            }
                

            query = $"UPDATE units SET status = 'In Service' WHERE unit = '{unit}'";

            cmd = new MySqlCommand(query, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {

            }
            reader.Close();
            connection.Close ();
        }

    }
}
