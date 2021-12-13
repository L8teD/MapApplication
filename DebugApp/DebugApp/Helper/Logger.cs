using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static DebugApp.Model.Types;
using System.Windows;
using static CommonLib.Types;
using System.Text.Json;
using System.Configuration;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using DebugApp.ViewModel;

namespace DebugApp.Model
{
    public class Logger
    {
        private static string tableName = "Logger";
        private static SQLiteConnection conn;
        private static SQLiteConnection MakeConn(string fileName)
        {
            return new SQLiteConnection("Data Source=" + fileName + ";Version=3;");
        }
        public static void PrintErrorInfo(string message, InitData initData)
        {
            DebugInfo info = new DebugInfo();
            info.Date = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
            info.Message = message;
            info.Fixed = 0;
            info.CountOfPoints = initData.rtpList.Count();
            info.input = new DebugInput();
            info.input.latitude = "";
            info.input.longitude = "";
            info.input.altitude = "";
            info.input.velocity = "";
            foreach (RouteTurningPoint RTP in initData.rtpList)
            {
                info.input.latitude += RTP.Latitude.ToString() + " ";
                info.input.longitude += RTP.Longitude.ToString() + " ";
                info.input.altitude += RTP.Altitude.ToString() + " ";
                info.input.velocity += RTP.Velocity.ToString()  + " ";
            }
            PrintInfoTXT(info);
            PrintInfoDB(info);
        }
        private static async void PrintInfoTXT(DebugInfo info)
        {
            string text = info.Date + "\n" + info.Message + "\n";
            
            text += "Input:" + "\nLat = " + info.input.latitude + "\nLon = " + info.input.longitude + "\nAlt = " + info.input.longitude + "\nVel = " + info.input.velocity;
            text += "\n----------------------------------------------------------------------------";
            try
            {
                using (StreamWriter sw = new StreamWriter("../../Logger.txt", true))
                {

                    await sw.WriteLineAsync(text);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private static void PrintInfoDB(DebugInfo info)
        {
            conn = MakeConn(ConfigurationManager.AppSettings["DataBaseFilePath"]);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            //OPEN DB
            command.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName +
                " (id INTEGER PRIMARY KEY, Date REAL, Message TEXT, Count INTEGER, Latitude TEXT, Longitude TEXT, Altitude TEXT, Velocity TEXT, Fixed INTEGER)";
            command.ExecuteNonQuery();
            //WRITE DB
            command.CommandText = "INSERT INTO " + tableName + " ('Date', 'Message', 'Count', 'Latitude', 'Longitude', 'Altitude', 'Velocity', 'Fixed') " +
                "values ('" + Converter.DateTimeToUnix(DateTime.Now) + "','" + info.Message + "','" + info.CountOfPoints + "','" + info.input.latitude + "','" 
                + info.input.longitude + "','" + info.input.altitude + "','" + info.input.velocity + "','"+ info.Fixed + "')";
            command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
        }
        public static List<DebugInfo> ReadInfoFromDB()
        {
            List<DebugInfo> infoList = new List<DebugInfo>();


            string cs = @"URI=file:" + ConfigurationManager.AppSettings["DataBaseFilePath"];
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    //    using (var cmd = new SQLiteCommand(con))
                    //    {
                    //        cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY,
                    //name TEXT, price INT)";
                    //        cmd.ExecuteNonQuery();
                    //        cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Audi',52642)";
                    //        cmd.ExecuteNonQuery();

                    //        cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Mercedes',57127)";
                    //        cmd.ExecuteNonQuery();
                    //    }
                    string stm = "SELECT * FROM Logger";

                    using (var cmd = new SQLiteCommand(stm, con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                DebugInfo info = new DebugInfo();
                                info.id = rdr.GetInt32(0);
                                DateTime date = Converter.UnixToDateTime(rdr.GetDouble(1));
                                info.Date = date.ToLongTimeString() + " " + date.ToLongDateString();
                                info.Message = rdr.GetString(2);
                                info.CountOfPoints = rdr.GetInt32(3);
                                info.input.latitude = rdr.GetString(4);
                                info.input.longitude = rdr.GetString(5);
                                info.input.altitude = rdr.GetString(6);
                                info.input.velocity = rdr.GetString(7);
                                info.Fixed = rdr.GetInt32(8);
                                infoList.Add(info);
                            }
                        }

                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show(sqlEx.Message);
            }
            
            return infoList;
            
        }
        public static void RemoveDataFromDB(int id)
        {
            string cs = @"URI=file:" + ConfigurationManager.AppSettings["DataBaseFilePath"];

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                string stm = $"DELETE FROM Logger WHERE id ={id}";

                using (var cmd = new SQLiteCommand(con))
                { 
                    cmd.CommandText = $"DELETE FROM Logger WHERE id ={id}";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        public class DebugInfo
        {
            public int id { get; set; }
            public string Date { get; set; }
            public string Message { get; set; }
            public int CountOfPoints { get; set; }
            public int Fixed { get; set; }
            public DebugInput input;
        }
        public struct DebugInput
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string altitude { get; set; }
            public string velocity { get; set; }
        }
    }
}
