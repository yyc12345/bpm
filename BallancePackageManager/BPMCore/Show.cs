using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Show {
        public static void Core(string packageName) {

            if (!File.Exists(Information.WorkPath.Enter("package.db").Path)) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            if (!packageName.Contains("@")) {
                ConsoleAssistance.WriteLine(I18N.Core("General_SpecificVersion"), ConsoleColor.Red);
                return;
            }

            //read database
            var packageDbConn = new Database();
            packageDbConn.Open();

            var reader = (from item in packageDbConn.CoreDbContext.package
                          where item.name == packageName.Split("@", StringSplitOptions.None)[0]
                          select item).ToList();
            //detect existing
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoMatchedPackage"), ConsoleColor.Red);
                packageDbConn.Close();
                return;
            }
            //detect version
            if (!reader[0].version.Split('@').Contains(packageName.Split("@")[1])) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoMatchedPackage"), ConsoleColor.Red);
                packageDbConn.Close();
                return;
            }

            //read json file
            var res = Download.DownloadPackageInfo(packageName);
            Console.WriteLine(Download.JudgeDownloadResult(res));
            if (res != Download.DownloadResult.OK && res != Download.DownloadResult.ExistedLocalFile) {
                ConsoleAssistance.WriteLine(I18N.Core("Show_FailJson"), ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine(packageName, ConsoleColor.Green);
            Console.WriteLine("");

            //databse data
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Aka")}{reader[0].aka}", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Type")}{((PackageType)reader[0].type).ToString()}", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine($"{I18N.Core("Search&Show_Desc")}{reader[0].desc}", ConsoleColor.Yellow);

            packageDbConn.Close();

            var fs = new StreamReader(Information.WorkPath.Enter("cache").Enter("dependency").Enter(packageName + ".json").Path, Encoding.UTF8);
            var cache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
            fs.Close();
            fs.Dispose();

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
