using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Data.SQLite;

namespace BallancePackageManager.BPMCore {
    public static class List {
        public static void Core() {
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            var gamePath = Config.Read()["GamePath"];
            ScriptEngine pyEngine = Python.CreateEngine();

            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            int count = installFolder.GetFiles().Count();
            int brokenCount = 0;
            int upgradableCount = 0;

            foreach (var item in installFolder.GetFiles()) {
                var fileNameInfo = ConsoleAssistance.GetScriptInfo(item.Name);
                
                ConsoleAssistance.Write(fileNameInfo.packageName, ConsoleColor.Green);
                ConsoleAssistance.Write($" / {fileNameInfo.version}");

                //check broken
                dynamic dd = pyEngine.ExecuteFile(ConsoleAssistance.WorkPath + @"cache\installed\" + item.Name);
                int res = dd.check(gamePath);
                if (res == 0) {
                    ConsoleAssistance.Write(" [broken]", ConsoleColor.Red);
                    brokenCount++;
                }

                //check update                
                var cursor = new SQLiteCommand($"select * from package where name == \"{fileNameInfo.packageName}\"", packageDbConn);
                var reader = cursor.ExecuteReader();
                reader.Read();
                if( reader["version"].ToString().Split(',').Last() != fileNameInfo.version) {
                    ConsoleAssistance.Write(" [upgradable]", ConsoleColor.Yellow);
                    upgradableCount++;
                }

                Console.Write("\n");
            }

            packageDbConn.Close();

            Console.WriteLine("");

            if (installFolder.GetFiles().Count() == 0) ConsoleAssistance.WriteLine("No installed package", ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine($"Total {count} packages. {brokenCount} broken. {upgradableCount} upgradable.", ConsoleColor.Yellow);
        }
    }
}
