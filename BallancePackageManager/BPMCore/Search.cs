using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Search {

        public static void Core(string packageName) {
            if (!File.Exists(Information.WorkPath.Enter("package.db").Path)) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                return;
            }

            var packageDbConn = new Database();
            packageDbConn.Open();

            var rgx = new Regex($@"(\w*|\W*){packageName}(\w*|\W*)");
            var reader = (from item in packageDbConn.CoreDbContext.package
                          where rgx.IsMatch(item.name) || rgx.IsMatch(item.aka)
                          select item).ToList();
            var folder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);

            foreach (var i in reader) {
                ConsoleAssistance.Write(i.name, ConsoleColor.Green);
                Console.Write(" @ " + i.version);

                var detectFiles = folder.GetDirectories($"{i.name}@");
                ConsoleAssistance.Write(detectFiles.Count() == 0 ? "" : $" [{I18N.Core("Search_InstalledVersion", detectFiles.Count().ToString())}]", ConsoleColor.Yellow);

                Console.Write($"\n{I18N.Core("Search&Show_Aka")}{i.aka}\n{I18N.Core("Search&Show_Type")}{((PackageType)i.type).ToString()}\n{I18N.Core("Search&Show_Desc")}{i.desc}\n\n");
            }

            ConsoleAssistance.WriteLine(I18N.Core("Search_Count", reader.Count.ToString()), ConsoleColor.Yellow);

            packageDbConn.Close();

        }

    }
}
