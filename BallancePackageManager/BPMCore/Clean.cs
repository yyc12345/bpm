using ShareLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager.BPMCore {
    public static class Clean {

        public static void Core() {
            //remove all file

            ConsoleAssistance.WriteLine(I18N.Core("Clean_CleanAll"), ConsoleColor.Yellow);
            //confirm
            Console.WriteLine();
            ConsoleAssistance.Write(I18N.Core("General_Continue"), ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine(I18N.Core("General_CancelOperation"), ConsoleColor.Red);
                return;
            }

            var strDownload = Information.WorkPath.Enter("cache").Enter("download").Path;
            var strDependency = Information.WorkPath.Enter("cache").Enter("dependency").Path;

            Console.WriteLine(I18N.Core("Clean_Deleting", "download"));
            Directory.Delete(strDownload, true);
            Directory.CreateDirectory(strDownload);

            Console.WriteLine(I18N.Core("Clean_Deleting", "dependency"));
            Directory.Delete(strDependency, true);
            Directory.CreateDirectory(strDependency);

            ConsoleAssistance.WriteLine(I18N.Core("General_AllOperationDown"), ConsoleColor.Yellow);
        }

        public static void Core(string packageName) {

            //get info
            var downloadFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("download").Path);
            var dependencyFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("dependency").Path);

            var filelist = new List<string>();

            if (packageName.Contains("@")) {
                //special version
                filelist.AddRange(from item in downloadFolder.GetFiles($"{packageName}.zip") select item.FullName);
                filelist.AddRange(from item in dependencyFolder.GetFiles($"{packageName}.json") select item.FullName);
            } else {
                //all version
                filelist.AddRange(from item in downloadFolder.GetFiles($"{packageName}@*.zip") select item.FullName);
                filelist.AddRange(from item in dependencyFolder.GetFiles($"{packageName}@*.json") select item.FullName);
            }

            ConsoleAssistance.WriteLine(I18N.Core("Clean_FileList"), ConsoleColor.Yellow);
            foreach (var item in filelist) {
                Console.WriteLine($"{item}");
            }

            //confirm
            Console.WriteLine();
            ConsoleAssistance.Write(I18N.Core("General_Continue"), ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine(I18N.Core("General_CancelOperation"), ConsoleColor.Red);
                return;
            }

            foreach (var item in filelist) {
                Console.WriteLine(I18N.Core("Clean_Deleting", item));
                File.Delete(item);
            }

            ConsoleAssistance.WriteLine(I18N.Core("General_AllOperationDown"), ConsoleColor.Yellow);

        }


    }
}

