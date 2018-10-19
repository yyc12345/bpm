using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BallancePackageManager.BPMCore {
    public static class Show {
        public static void Core(string packageName) {

            if (!packageName.Contains("@")) {
                ConsoleAssistance.WriteLine("You should input package name with its version", ConsoleColor.Red);
                return;
            }

            var res = Download.DownloadPackageInfo(packageName);
            Console.WriteLine(Download.JudgeDownloadResult(res));
            if (res != Download.DownloadResult.OK && res != Download.DownloadResult.ExistedLocalFile) {
                ConsoleAssistance.WriteLine("Fail to read JSON file", ConsoleColor.Red);
            }

            var fs = new StreamReader(ConsoleAssistance.WorkPath + @"cache\dependency\" + packageName + ".json", Encoding.UTF8);
            var cache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
            fs.Close();
            fs.Dispose();

            ConsoleAssistance.WriteLine(packageName, ConsoleColor.Green);
            Console.WriteLine("");
            ConsoleAssistance.WriteLine("dependency: ", ConsoleColor.Yellow);
            if (cache.dependency.Count() == 0) ConsoleAssistance.WriteLine("None", ConsoleColor.Yellow);
            foreach (var item in cache.dependency) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");
            if (!cache.reverseConflict) ConsoleAssistance.WriteLine("conflict: ", ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine("only compatible with: ", ConsoleColor.Yellow);
            if (cache.conflict.Count() == 0) ConsoleAssistance.WriteLine("None", ConsoleColor.Yellow);
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
