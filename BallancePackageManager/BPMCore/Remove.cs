using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager.BPMCore {
    public static class Remove {

        public static void Core(string packageName) {

            //get info
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            FileInfo[] fileList;
            if (packageName.Contains("@")) fileList = installFolder.GetFiles(packageName + ".py");
            else fileList = installFolder.GetFiles($"{packageName}@*.py");

            if (fileList.Count() == 0) {
                ConsoleAssistance.WriteLine("No matched installed package", ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.Write("There are the package which will be deleted: ", ConsoleColor.Yellow);
            foreach (var item in fileList) {
                var cache = ConsoleAssistance.GetScriptInfo(item.Name);
                Console.WriteLine($"{cache.packageName} / {cache.version}");
            }

            ConsoleAssistance.Write("Are you sure that you want to remove all of them (Y/N): ", ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine("You cancle the operation.", ConsoleColor.Red);
                return;
            }

            //remove
            RealRemove((from i in fileList select i.Name).ToList());

            ConsoleAssistance.WriteLine("All packages have been removed", ConsoleColor.Yellow);
        }

        public static void RealRemove(List<string> fileList) {
            var gamePath = Config.Read()["GamePath"];
            ScriptEngine pyEngine = Python.CreateEngine();

            foreach (var item in fileList) {
                dynamic dd = pyEngine.ExecuteFile(ConsoleAssistance.WorkPath + @"cache\installed\" + item);
                dd.check(gamePath);
                File.Delete(ConsoleAssistance.WorkPath + @"cache\installed\" + item);

                var cache = ConsoleAssistance.GetScriptInfo(item);
                Console.WriteLine($"Remove {cache.packageName} / {cache.version} successfully");
            }

            
        }

    }
}
