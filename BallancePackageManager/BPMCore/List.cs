using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class List {
        public static void Core() {
            
            var installFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);

            var packageDbConn = new Database();
            packageDbConn.Open();

            int count = installFolder.GetDirectories().Count();
            //int brokenCount = 0;
            //int upgradableCount = 0;

            foreach (var item in installFolder.GetDirectories()) {
                Console.Write($"{item.Name}");

                //check update and output type
                var reader = (from item2 in packageDbConn.CoreDbContext.package
                              where item2.name == item.Name.Split('@', StringSplitOptions.None)[0]
                              select item2).ToList();
                Console.Write(" [" + I18N.Core($"PackageType_{((PackageType)reader[0].type).ToString()}") + "]");
                //if (reader["version"].ToString().Split(',').Last() != item.Name.Split('@')[1]) {
                //    ConsoleAssistance.Write($" [{I18N.Core("List_Upgradable")}]", ConsoleColor.Yellow);
                //    upgradableCount++;
                //}

                ////check broken
                //var res = ScriptInvoker.Core(item.FullName, ScriptInvoker.InvokeMethod.Check, "");
                //if (!res) {
                //    ConsoleAssistance.Write($" [{I18N.Core("List_Broken")}]", ConsoleColor.Red);
                //    brokenCount++;
                //}

                Console.Write("\n");
            }

            packageDbConn.Close();

            Console.WriteLine("");

            if (count == 0) ConsoleAssistance.WriteLine(I18N.Core("General_None"), ConsoleColor.Yellow);
            else ConsoleAssistance.WriteLine(I18N.Core("List_Total", count.ToString()), ConsoleColor.Yellow);
        }
    }
}
