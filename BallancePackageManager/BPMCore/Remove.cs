using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Remove {

        public static void Core(string packageName) {

            //get info
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            DirectoryInfo[] directoryList;
            if (packageName.Contains("@")) directoryList = installFolder.GetDirectories($"{packageName}");
            else directoryList = installFolder.GetDirectories($"{packageName}@*");

            if (directoryList.Count() == 0) {
                ConsoleAssistance.WriteLine("No matched installed package", ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine("There are the package which will be deleted: ", ConsoleColor.Yellow);
            foreach (var item in directoryList) {
                Console.WriteLine($"{item.Name}");
            }

            Console.WriteLine();
            ConsoleAssistance.Write("Are you sure that you want to remove all of them (Y/N): ", ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine("You cancle the operation.", ConsoleColor.Red);
                return;
            }

            //remove
            var res = RealRemove((from i in directoryList select i.Name).ToList());
            if (!res) ConsoleAssistance.WriteLine("Operation is aborted", ConsoleColor.Red);

            ConsoleAssistance.WriteLine("All packages have been removed", ConsoleColor.Yellow);
        }

        public static bool RealRemove(List<string> packageList) {
            foreach (var item in packageList) {
                var res = ScriptInvoker.Core(ConsoleAssistance.WorkPath + @"\cache\installed\" + item, ScriptInvoker.InvokeMethod.Remove, "");
                if (!res) {
                    ConsoleAssistance.WriteLine($"A error is throwed when removing {item}", ConsoleColor.Red);
                    return false;
                }
                Directory.Delete(ConsoleAssistance.WorkPath + @"\cache\installed\" + item, true);

                Console.WriteLine($"Remove {item} successfully");
            }

            return true;
        }

    }
}
