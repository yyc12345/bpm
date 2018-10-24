using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using ShareLib;

namespace BPMSMaintenance {

    public static class PackageManager {

        public static void AddPackage(SQLiteConnection packageDbConn, string name, string aka, string type, string version, string desc, string package_file_path, string dependency_file_path) {

            var cursor = new SQLiteCommand($"select * from package where name == \"{name}\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            if (reader.Read()) {
                ConsoleAssistance.WriteLine("Existed package. Please use addver to add package.", ConsoleColor.Red);
                return;
            }

            //set database
            cursor = new SQLiteCommand($"insert into package (name,aka,type,version,desc) values(\"{name}\",\"{aka}\",{type},\"{version}\",\"{desc}\")", packageDbConn);
            cursor.ExecuteNonQuery();

            //copy file
            File.Copy(package_file_path, ConsoleAssistance.WorkPath + $"package\\{name}@{version}.zip");
            File.Copy(dependency_file_path, ConsoleAssistance.WorkPath + $"dependency\\{name}@{version}.json");

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void AddVersion(SQLiteConnection packageDbConn, string name, string version, string package_file_path, string dependency_file_path) {

            var cursor = new SQLiteCommand($"select * from package where name == \"{name}\"", packageDbConn);    
            var reader = cursor.ExecuteReader();
            if (!reader.Read()) {
                ConsoleAssistance.WriteLine("Lost package. Please use addpkg to add package.", ConsoleColor.Red);
                return;
            }

            //set database
            var versionList = reader["version"].ToString().Split(',');
            if (versionList.Contains(version)) {
                ConsoleAssistance.WriteLine("Existed version.", ConsoleColor.Red);
                return;
            }

            var newData = String.Join(",", versionList) + $",{version}";

            cursor = new SQLiteCommand($"update package set version = \"{newData}\" where name == \"{name}\"", packageDbConn);
            cursor.ExecuteNonQuery();

            //copy file
            File.Copy(package_file_path, ConsoleAssistance.WorkPath + $"package\\{name}@{version}.zip");
            File.Copy(dependency_file_path, ConsoleAssistance.WorkPath + $"dependency\\{name}@{version}.json");

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void RemovePackage(SQLiteConnection packageDbConn, string name) {

            var cursor = new SQLiteCommand($"select * from package where name == \"{name}\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            if (!reader.Read()) {
                ConsoleAssistance.WriteLine("Lost package. Check out your package name.", ConsoleColor.Red);
                return;
            }

            //set database
            cursor = new SQLiteCommand($"delete from package where name == \"{name}\"", packageDbConn);
            cursor.ExecuteNonQuery();

            //del file
            var folder1 = new DirectoryInfo(ConsoleAssistance.WorkPath + @"package");
            foreach (var item in folder1.GetFiles($"{name}@*.zip")) {
                File.Delete(item.FullName);
            }
            folder1 = new DirectoryInfo(ConsoleAssistance.WorkPath + @"dependency");
            foreach (var item in folder1.GetFiles($"{name}@*.json")) {
                File.Delete(item.FullName);
            }

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void RemoveVersion(SQLiteConnection packageDbConn, string name, string version) {

            var cursor = new SQLiteCommand($"select * from package where name == \"{name}\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            if (!reader.Read()) {
                ConsoleAssistance.WriteLine("Lost package. Check out your package name.", ConsoleColor.Red);
                return;
            }

            //set database
            var versionList = new List<string>(reader["version"].ToString().Split(','));
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

            cursor = new SQLiteCommand($"update package set version = \"{newData}\" where name == \"{name}\"", packageDbConn);
            cursor.ExecuteNonQuery();

            //del file
            File.Delete(ConsoleAssistance.WorkPath + $"package\\{name}@{version}.zip");
            File.Delete(ConsoleAssistance.WorkPath + $"dependency\\{name}@{version}.json");

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

    }

}
