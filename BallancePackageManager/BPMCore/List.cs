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
            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            int count = installFolder.GetDirectories().Count();
            int brokenCount = 0;
            int upgradableCount = 0;

            foreach (var item in installFolder.GetDirectories()) {
                Console.Write($"{item.Name}");

                //check update and output type             
                var cursor = new SQLiteCommand($"select * from package where name == \"{item.Name.Split('@')[0]}\"", packageDbConn);
                var reader = cursor.ExecuteReader();
                reader.Read();
                Console.Write(" [" + ((PackageType)int.Parse(reader["type"].ToString())).ToString() + "]");
                if (reader["version"].ToString().Split(',').Last() != item.Name.Split('@')[1]) {
                    ConsoleAssistance.Write($" [{I18N.Core("List_Upgradable")}]", ConsoleColor.Yellow);
                    upgradableCount++;
                }

                //check broken
                var res = ScriptInvoker.Core(item.FullName, ScriptInvoker.InvokeMethod.Check, "");
                if (!res) {
                    ConsoleAssistance.Write($" [{I18N.Core("List_Broken")}]", ConsoleColor.Red);
                    brokenCount++;
                }

                Console.Write("\n");
            }

            packageDbConn.Close();

            Console.WriteLine("");

            if (count == 0) ConsoleAssistance.WriteLine(I18N.Core("General_None"), ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine(I18N.Core("List_Total", count.ToString(), brokenCount.ToString(), upgradableCount.ToString()), ConsoleColor.Yellow);
        }
    }
}
