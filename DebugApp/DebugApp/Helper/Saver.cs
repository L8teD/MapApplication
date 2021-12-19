using CommonLib.Params;
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
using CsvHelper;

namespace DebugApp.Model
{
    class Saver
    {

        public static void WriteCSV<T>(List<T> data, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(data);
                    csvWriter.Flush();
                }
            }
        }
    }
}
