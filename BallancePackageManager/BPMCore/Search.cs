﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ShareLib;

namespace BallancePackageManager {

    public partial class BPMInstance {

        public void Search_CoreWrapper(List<string> packageName) {
            if (!CheckStatus(BPMInstanceMethod.Search, BPMInstanceStatus.Ready)) return;
            if (!HaveDatabase(BPMInstanceMethod.Search)) return;
            CurrentStatus = BPMInstanceStatus.Working;
            Search_Core(packageName);
            CurrentStatus = BPMInstanceStatus.Ready;
            BPMInstanceEvent_MethodDone?.Invoke(BPMInstanceMethod.Search);
        }

        private void Search_Core(List<string> packageName) {

            packageName = (from item in packageName
                           where item != ""
                           select item).ToList();

            //convert package name to regex words
            var fullMatchedRgxList = new List<string>();
            foreach (var item in packageName) {
                fullMatchedRgxList.Add($@"((\w|\W)*{item}(\w|\W)*)");
            }

            //query
            var packageDbConn = new PackageDatabase();
            packageDbConn.Open();

            string full_matched = String.Join("|", fullMatchedRgxList.ToArray());
            var rgx = new Regex($@"({full_matched})");
            var singleMatchedReader = (from item in packageDbConn.CoreDbContext.package
                                       where rgx.IsMatch(item.name) || ((item.aka == null || item.aka == "") ? false : rgx.IsMatch(item.aka))
                                       select item).ToList();

            var fullMatchedReader = (from item in packageDbConn.CoreDbContext.package
                                     where Search_ContainAll(item.name, packageName) || ((item.aka == null || item.aka == "") ? false : Search_ContainAll(item.aka, packageName))
                                     select item).ToList();

            //output
            var folder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);

            //full matched
            ConsoleAssistance.WriteLine(I18N.Core("Search_FullMatched"), ConsoleColor.Yellow);
            Console.WriteLine();
            foreach (var i in fullMatchedReader) {
                ConsoleAssistance.Write(i.name, ConsoleColor.Green);
                Console.Write(" @ " + i.version);

                var detectFiles = folder.GetDirectories($"{i.name}@");
                ConsoleAssistance.Write(detectFiles.Count() == 0 ? "" : $" [{I18N.Core("Search_InstalledVersion", detectFiles.Count().ToString())}]", ConsoleColor.Yellow);

                Console.Write($"\n{I18N.Core("Search&Show_Aka")}{i.aka}\n{I18N.Core("Search&Show_Type")}{I18N.Core($"PackageType_{((PackageType)i.type).ToString()}")}\n{I18N.Core("Search&Show_Desc")}{i.desc}\n\n");
            }
            ConsoleAssistance.WriteLine(I18N.Core("Search_Count", fullMatchedReader.Count.ToString()), ConsoleColor.Yellow);
            Console.WriteLine();

            //single matched
            ConsoleAssistance.WriteLine(I18N.Core("Search_SingleMatched"), ConsoleColor.Yellow);
            Console.WriteLine();
            foreach (var i in singleMatchedReader) {
                ConsoleAssistance.Write(i.name, ConsoleColor.Green);
                Console.Write(" @ " + i.version);

                var detectFiles = folder.GetDirectories($"{i.name}@");
                ConsoleAssistance.Write(detectFiles.Count() == 0 ? "" : $" [{I18N.Core("Search_InstalledVersion", detectFiles.Count().ToString())}]", ConsoleColor.Yellow);

                Console.Write($"\n{I18N.Core("Search&Show_Aka")}{i.aka}\n{I18N.Core("Search&Show_Type")}{I18N.Core($"PackageType_{((PackageType)i.type).ToString()}")}\n{I18N.Core("Search&Show_Desc")}{i.desc}\n\n");
            }
            ConsoleAssistance.WriteLine(I18N.Core("Search_Count", singleMatchedReader.Count.ToString()), ConsoleColor.Yellow);

            packageDbConn.Close();

        }

        private bool Search_ContainAll(string str, List<string> matchList) {
            foreach (var item in matchList) {
                if (!str.Contains(item)) return false;
            }
            return true;
        }
    }
}
