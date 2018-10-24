using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Search {

        public static void Core(string packageName) {
            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                ConsoleAssistance.WriteLine("No package database. Please use *bpm update* to update your local package database.", ConsoleColor.Red);
                return;
            }

            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            var cursor = new SQLiteCommand($"select * from package where name like \"%{packageName}%\" or aka like \"%{packageName}%\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            var folder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"cache\installed");
            int count = 0;
            while (reader.Read()) {
                ConsoleAssistance.Write(reader["name"].ToString(), ConsoleColor.Green);
                Console.Write(" / " + reader["version"].ToString());

                var detectFiles = folder.GetDirectories($"{reader["name"].ToString()}@");
                ConsoleAssistance.Write(detectFiles.Count() == 0 ? "" : $" [{detectFiles.Count()} installed version]", ConsoleColor.Yellow);

                Console.Write($"\naka: {reader["aka"].ToString()}\ndescription: {reader["desc"].ToString()}\n\n");
                count++;
            }

            ConsoleAssistance.WriteLine($"Total {count} matched packages", ConsoleColor.Yellow);

            packageDbConn.Close();

        }

    }
}
