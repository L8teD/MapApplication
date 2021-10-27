using CommonLib.Params;
using CsvHelper;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF.Helper
{
    class Saver
    {
        private static SQLiteConnection conn;
        private static SQLiteConnection MakeConn(string fileName)
        {
            return new SQLiteConnection("Data Source=" + fileName + ";Version=3;");
        }
        public static void WriteDB(OutputData outputData, string fileName)
        {
            WriteDB(outputData.FullDisplayedData.DisplayedDatasIdeal, fileName, "ideal");
            WriteDB(outputData.FullDisplayedData.DisplayedDatasError, fileName, "Error");
        }
        private static void WriteDB(List<DisplayedData> displayedDatas, string fileName, string tableName)
        {
            conn = MakeConn(fileName);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            //OPEN DB
            command.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + " (Latitude REAL, Longitude REAL, Velocity REAL, VelocityNorth REAL, VelocityEast REAL, Heading REAL, Roll REAL, Pitch REAL)";
            command.ExecuteNonQuery();
            //WRITE DB

            if (conn.State != ConnectionState.Open)
            {
                //Console.WriteLine("Open connection with database");
            }
            foreach (DisplayedData displayedData in displayedDatas)
            {
                command.CommandText = "INSERT INTO " +tableName +" ('Latitude', 'Longitude', 'Velocity', 'VelocityNorth', 'VelocityEast', 'Heading', 'Roll', 'Pitch') " +
                    "values ('" + displayedData.Latitude + "','" + displayedData.Longitude + "','" + displayedData.Velocity + "','" + displayedData.VelocityNorth +
                    "','" + displayedData.VelocityEast + "','" + displayedData.Heading + "','" + displayedData.Roll + "','" + displayedData.Pitch + "')";
                command.ExecuteNonQuery();
            }
            command.Dispose();
            conn.Close();
        }
        public static void WriteCSV(List<DisplayedData> displayedDatas, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(displayedDatas);
                    csvWriter.Flush();
                }
            }

        }
    }
}
