using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class List {
        public static void Core() {
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            int count = installFolder.GetDirectories().Count();
            int brokenCount = 0;
            int upgradableCount = 0;

            foreach (var item in installFolder.GetDirectories()) {
                Console.Write($"{item.Name}");

                //check broken
                var res = ScriptInvoker.Core(item.FullName, ScriptInvoker.InvokeMethod.Check, "");
                if (!res) {
                    ConsoleAssistance.Write(" [broken]", ConsoleColor.Red);
                    brokenCount++;
                }

                //check update                
                var cursor = new SQLiteCommand($"select * from package where name == \"{item.Name.Split('@')[0]}\"", packageDbConn);
                var reader = cursor.ExecuteReader();
                reader.Read();
                if( reader["version"].ToString().Split(',').Last() != item.Name.Split('@')[1]) {
                    ConsoleAssistance.Write(" [upgradable]", ConsoleColor.Yellow);
                    upgradableCount++;
                }

                Console.Write("\n");
            }

            packageDbConn.Close();

            Console.WriteLine("");

            if (count == 0) ConsoleAssistance.WriteLine("No installed package", ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine($"Total {count} packages. {brokenCount} broken. {upgradableCount} upgradable.", ConsoleColor.Yellow);
        }
    }
}
