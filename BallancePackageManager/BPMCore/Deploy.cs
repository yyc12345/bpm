using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Deploy {

        public static void Core(string packageName, string parameter) {
            //get info
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            DirectoryInfo[] directoryList;
            if (packageName.Contains("@")) directoryList = installFolder.GetDirectories($"{packageName}");
            else {
                ConsoleAssistance.WriteLine("You should specific a version of your package", ConsoleColor.Red);
                return;
            }

            if (directoryList.Count() == 0) {
                ConsoleAssistance.WriteLine("No matched installed package", ConsoleColor.Red);
                return;
            }

            var finalFolder = directoryList[0];
            var res = ScriptInvoker.Core(finalFolder.FullName, ScriptInvoker.InvokeMethod.Deploy, parameter);
            if (!res) {
                ConsoleAssistance.WriteLine("A error is occured when deploying package.", ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine("Delopy successfully!", ConsoleColor.Yellow);
        }

    }
}
