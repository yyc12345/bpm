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
            if (!File.Exists(Information.WorkPath.Enter("package.db").Path)) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            //get info
            var installFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);

            DirectoryInfo[] directoryList;
            if (packageName.Contains("@")) directoryList = installFolder.GetDirectories($"{packageName}");
            else directoryList = installFolder.GetDirectories($"{packageName}@*");

            if (directoryList.Count() == 0) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoMatchedPackage"), ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine(I18N.Core("Remove_RemoveList"), ConsoleColor.Yellow);
            foreach (var item in directoryList) {
                Console.WriteLine($"{item.Name}");
            }

            Console.WriteLine();
            ConsoleAssistance.Write(I18N.Core("General_Continue"), ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine(I18N.Core("General_CancelOperation"), ConsoleColor.Red);
                return;
            }

            //remove
            var res = RealRemove((from i in directoryList select i.Name).ToList());
            if (!res) {
                ConsoleAssistance.WriteLine(I18N.Core("General_OperationAborted"), ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine(I18N.Core("General_AllOperationDown"), ConsoleColor.Yellow);
        }

        public static bool RealRemove(List<string> packageList) {
            foreach (var item in packageList) {
                Console.WriteLine(I18N.Core("Remove_Removing", item));
                var res = ScriptInvoker.Core(Information.WorkPath.Enter("cache").Enter("installed").Enter(item).Path, ScriptInvoker.InvokeMethod.Remove, "");
                if (!res.status) {
                    ConsoleAssistance.WriteLine(I18N.Core("General_ScriptError"), ConsoleColor.Red);
                    ConsoleAssistance.WriteLine(res.desc, ConsoleColor.Red);
                    return false;
                }
                Directory.Delete(Information.WorkPath.Enter("cache").Enter("installed").Enter(item).Path, true);

                Console.WriteLine(I18N.Core("Remove_Success", item));
            }

            return true;
        }

    }
}
