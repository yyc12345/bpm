using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ShareLib;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Query;
//using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BPMSMaintenance {

    public static class PackageManager {

        public static void AddPackage(Database packageDbConn, string name, string aka, string type, string version, string desc, string package_file_path, string dependency_file_path) {

            var reader = packageDbConn.CoreDbContext.package
                .Where(x => x.name == name)
                .ToList();
            if (reader.Any()) {
                ConsoleAssistance.WriteLine("Existed package. Please use addver to add package.", ConsoleColor.Red);
                return;
            }

            //set database
            var newObj = new DatabaseItem();
            newObj.name = name;
            newObj.aka = aka;
            newObj.type = int.Parse(type);
            newObj.version = version;
            newObj.desc = desc;
            packageDbConn.CoreDbContext.package.Add(newObj);

            //copy file
            File.Copy(package_file_path, Information.WorkPath.Enter("package").Enter($"{name}@{version}.zip").Path);
            File.Copy(dependency_file_path, Information.WorkPath.Enter("dependency").Enter($"{name}@{version}.json").Path);

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void AddVersion(Database packageDbConn, string name, string version, string package_file_path, string dependency_file_path) {

            var reader = (from item in packageDbConn.CoreDbContext.package
                          where item.name == name
                          select item).ToList();
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package. Please use addpkg to add package.", ConsoleColor.Red);
                return;
            }

            //set database
            var versionList = reader[0].version.Split(',');
            if (versionList.Contains(version)) {
                ConsoleAssistance.WriteLine("Existed version.", ConsoleColor.Red);
                return;
            }

            var newData = String.Join(",", versionList) + $",{version}";

            reader[0].version = newData;

            //copy file
            File.Copy(package_file_path, Information.WorkPath.Enter("package").Enter($"{name}@{version}.zip").Path);
            File.Copy(dependency_file_path, Information.WorkPath.Enter("dependency").Enter($"{name}@{version}.json").Path);

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void RemovePackage(Database packageDbConn, string name) {

            var reader = (from item in packageDbConn.CoreDbContext.package
                          where item.name == name
                          select item).ToList();
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package. Check out your package name.", ConsoleColor.Red);
                return;
            }

            //set database
            packageDbConn.CoreDbContext.package.RemoveRange(reader);

            //del file
            var folder1 = new DirectoryInfo(Information.WorkPath.Enter("package").Path);
            foreach (var item in folder1.GetFiles($"{name}@*.zip")) {
                File.Delete(item.FullName);
            }
            folder1 = new DirectoryInfo(Information.WorkPath.Enter("dependency").Path);
            foreach (var item in folder1.GetFiles($"{name}@*.json")) {
                File.Delete(item.FullName);
            }

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void RemoveVersion(Database packageDbConn, string name, string version) {

            var reader = (from item in packageDbConn.CoreDbContext.package
                          where item.name == name
                          select item).ToList();
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package. Check out your package name.", ConsoleColor.Red);
                return;
            }

            //set database
            var versionList = new List<string>(reader[0].version.Split(','));
            if (!versionList.Contains(version)) {
                ConsoleAssistance.WriteLine("No matched version.", ConsoleColor.Red);
                return;
            }

            versionList.Remove(version);
            if (versionList.Count == 0) {
                ConsoleAssistance.WriteLine("This is this package's last version. Please use delpkg to remove it.", ConsoleColor.Red);
                return;
            }

            var newData = String.Join(",", versionList);

            reader[0].version = newData;

            //del file
            File.Delete(Information.WorkPath.Enter("package").Enter($"{name}@{version}.zip").Path);
            File.Delete(Information.WorkPath.Enter("dependency").Enter($"{name}@{version}.json").Path);

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

    }

}
