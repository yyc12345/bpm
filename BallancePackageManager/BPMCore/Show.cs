using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Show {
        public static void Core(string packageName) {

            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            if (!packageName.Contains("@")) {
                ConsoleAssistance.WriteLine(I18N.Core("General_SpecificVersion"), ConsoleColor.Red);
                return;
            }

            //read json file
            var res = Download.DownloadPackageInfo(packageName);
            Console.WriteLine(Download.JudgeDownloadResult(res));
            if (res != Download.DownloadResult.OK && res != Download.DownloadResult.ExistedLocalFile) {
                ConsoleAssistance.WriteLine(I18N.Core("Show_FailJson"), ConsoleColor.Red);
                return;
            }

            var fs = new StreamReader(ConsoleAssistance.WorkPath + @"cache\dependency\" + packageName + ".json", Encoding.UTF8);
            var cache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
            fs.Close();
            fs.Dispose();

            ConsoleAssistance.WriteLine(packageName, ConsoleColor.Green);
            Console.WriteLine("");

            //read database
            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            var cursor = new SQLiteCommand($"select * from package where name == \"{packageName.Split('@')[0]}\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            reader.Read();
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Aka")}{reader["aka"].ToString()}", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Type")}{((PackageType)int.Parse(reader["type"].ToString())).ToString()}", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Desc")}{reader["desc"].ToString()}", ConsoleColor.Yellow);

            packageDbConn.Close();

            Console.WriteLine("");
            //output json
            ConsoleAssistance.WriteLine(I18N.Core("Show_Dependency"), ConsoleColor.Yellow);
            if (cache.dependency.Count() == 0) ConsoleAssistance.WriteLine("None", ConsoleColor.Yellow);
            foreach (var item in cache.dependency) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");
            if (!cache.reverseConflict) ConsoleAssistance.WriteLine(I18N.Core("Show_Conflict"), ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine(I18N.Core("Show_Compatible"), ConsoleColor.Yellow);
            if (cache.conflict.Count() == 0) ConsoleAssistance.WriteLine(I18N.Core("General_None"), ConsoleColor.Yellow);
            foreach (var item in cache.conflict) {
                Console.WriteLine(item);
            }

        }
    }


    public class PackageJson {
        public string[] dependency { get; set; }
        public bool reverseConflict { get; set; }
        public string[] conflict { get; set; }
    }

}
