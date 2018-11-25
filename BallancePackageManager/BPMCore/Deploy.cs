﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Deploy {

        public static void Core(string packageName, string parameter) {
            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            //get info
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");

            DirectoryInfo[] directoryList;
            if (packageName.Contains("@")) directoryList = installFolder.GetDirectories($"{packageName}");
            else {
                ConsoleAssistance.WriteLine(I18N.Core("General_SpecificVersion"), ConsoleColor.Red);
                return;
            }

            if (directoryList.Count() == 0) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoMatchedPackage"), ConsoleColor.Red);
                return;
            }

            var finalFolder = directoryList[0];
            var res = ScriptInvoker.Core(finalFolder.FullName, ScriptInvoker.InvokeMethod.Deploy, parameter);
            if (res.status) {
                ConsoleAssistance.WriteLine(I18N.Core("General_ScriptError"), ConsoleColor.Red);
                ConsoleAssistance.WriteLine(res.desc, ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine(I18N.Core("General_AllOperationDown"), ConsoleColor.Yellow);
        }

    }
}
