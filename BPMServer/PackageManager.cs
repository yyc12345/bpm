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

namespace BPMServer {

    public static class PackageManager {

        public static void Ls(PackageDatabase packageDbConn, string limit) {
            if (limit == "") {
                foreach (var item in packageDbConn.CoreDbContext.package)
                    Console.WriteLine(item.name);
            } else {
                var reader = from it in packageDbConn.CoreDbContext.package
                             where it.name == limit
                             select it;
                if (!reader.Any()) {
                    ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                    return;
                }

                var item = reader.First();
                ConsoleAssistance.WriteLine(item.name, ConsoleColor.Yellow);
                Console.WriteLine($"aka: {item.aka}");
                Console.WriteLine($"type: {item.type} ({((PackageType)item.type).ToString()})");
                Console.WriteLine($"desc: {item.desc}");
                Console.WriteLine("all version:");

                var reader2 = from it2 in packageDbConn.CoreDbContext.version
                              where it2.parent == limit
                              select it2;
                foreach (var item2 in reader2)
                    Console.WriteLine($"\t{item2.name}");

            }
        }

        public static void Show(PackageDatabase packageDbConn, string name) {

            var reader = from it in packageDbConn.CoreDbContext.version
                         where it.name == name
                         select it;
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                return;
            }

            ConsoleAssistance.WriteLine(name, ConsoleColor.Yellow);
            var item = reader.First();

            Console.WriteLine($"parent: {item.parent}");
            Console.WriteLine($"additional desc: {item.additional_desc}");
            Console.WriteLine($"timestamp: {item.timestamp} ({item.timestamp.ToDateTime().ToString()})");
            Console.WriteLine($"suit os: Windows [{(OSType.Windows == ((OSType)item.suit_os & OSType.Windows) ? "X" : " ")}], UNIX [{(OSType.Unix == ((OSType)item.suit_os & OSType.Unix) ? "X" : " ")}], macOS [{(OSType.macOS == ((OSType)item.suit_os & OSType.macOS) ? "X" : " ")}]");
            Console.WriteLine($"dependency: {item.dependency}");
            Console.WriteLine($"reverse conflict: {item.reverse_conflict}");
            Console.WriteLine($"conflict: {item.conflict}");
            Console.WriteLine($"require decompress: {item.require_decompress}");
            Console.WriteLine($"internal script: {item.internal_script}");
            Console.WriteLine($"hash: {item.hash}");

        }

        public static void AddPackage(PackageDatabase packageDbConn, Command.AddpkgOption data) {

            var reader = from item in packageDbConn.CoreDbContext.package
                         where item.name == data.Name
                         select item;
            if (reader.Any()) {
                ConsoleAssistance.WriteLine("Existed package.", ConsoleColor.Red);
                return;
            }

            //set database
            var newObj = data.ToDatabaseFormat();
            if (newObj.status) {
                packageDbConn.CoreDbContext.package.Add(newObj.res);
                ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
            } else ConsoleAssistance.WriteLine("Illegal parameter", ConsoleColor.Red);

        }

        public static void AddVersion(PackageDatabase packageDbConn, Command.AddverOption data) {

            //check exist
            var reader = from item in packageDbConn.CoreDbContext.version
                         where item.name == data.Name
                         select item;
            if (reader.Any()) {
                ConsoleAssistance.WriteLine("Existed package.", ConsoleColor.Red);
                return;
            }

            //set database
            var newObj = data.ToDatabaseFormat();
            if (newObj.status) {
                //update database
                packageDbConn.CoreDbContext.version.Add(newObj.res);
                //copy package
                File.Copy(data.PackagePath, Information.WorkPath.Enter("package").Enter($"{data.Name}.zip").Path);

                ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
            } else ConsoleAssistance.WriteLine("Illegal parameter", ConsoleColor.Red);
        }

        public static void EditPackage(PackageDatabase packageDbConn, Command.EditpkgOption data) {
            var reader = from item in packageDbConn.CoreDbContext.package
                         where item.name == data.Name
                         select item;
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                return;
            }

            var got = reader.First();
            var newObj = data.ToDatabaseFormat(got);
            if (newObj.status) {
                packageDbConn.CoreDbContext.package.Remove(got);
                packageDbConn.CoreDbContext.package.Add(newObj.res);
                ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
            } else ConsoleAssistance.WriteLine("Illegal parameter", ConsoleColor.Red);
        }

        public static void EditVersion(PackageDatabase packageDbConn, Command.EditverOption data) {
            var reader = from item in packageDbConn.CoreDbContext.version
                         where item.name == data.Name
                         select item;
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                return;
            }

            var got = reader.First();
            var newObj = data.ToDatabaseFormat(got);
            if (newObj.status) {
                packageDbConn.CoreDbContext.version.Remove(got);
                packageDbConn.CoreDbContext.version.Add(newObj.res);

                //copy file
                if (data.PackagePath != "~") {
                    File.Delete(Information.WorkPath.Enter("package").Enter($"{data.Name}.zip").Path);
                    File.Copy(data.PackagePath, Information.WorkPath.Enter("package").Enter($"{data.Name}.zip").Path);
                }
                ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
            } else ConsoleAssistance.WriteLine("Illegal parameter", ConsoleColor.Red);
        }

        public static void RemovePackage(PackageDatabase packageDbConn, string name) {

            //remove from package table
            var reader = from item in packageDbConn.CoreDbContext.package
                         where item.name == name
                         select item;
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                return;
            }

            //set database
            packageDbConn.CoreDbContext.package.RemoveRange(reader);

            //remove all version
            var reader2 = from item in packageDbConn.CoreDbContext.version
                          where item.parent == name
                          select item;
            foreach (var item in reader2) {
                //remove file and database
                File.Delete(Information.WorkPath.Enter("package").Enter($"{item.name}.zip").Path);
                packageDbConn.CoreDbContext.version.Remove(item);
            }

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

        public static void RemoveVersion(PackageDatabase packageDbConn, string name) {

            var reader = from item in packageDbConn.CoreDbContext.version
                         where item.name == name
                         select item;
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine("Lost package.", ConsoleColor.Red);
                return;
            }

            //set database
            packageDbConn.CoreDbContext.version.RemoveRange(reader);

            //del file
            File.Delete(Information.WorkPath.Enter("package").Enter($"{name}.zip").Path);

            ConsoleAssistance.WriteLine("Operation done.", ConsoleColor.Yellow);
        }

    }

    /// <summary>
    /// the helper for convert command to database structure
    /// </summary>
    public static class CommandOptionHelper {

        public static (PackageDatabaseTablePackageItem res, bool status) ToDatabaseFormat(this Command.AddpkgOption ori) {
            var obj = new PackageDatabaseTablePackageItem() {
                name = ori.Name,
                aka = ori.Aka,
                desc = ori.Desc
            };

            try {
                obj.type = (PackageType)(int.Parse(ori.Type));
            } catch {
                return (obj, false);
            }

            return (obj, true);
        }

        public static (PackageDatabaseTablePackageItem res, bool status) ToDatabaseFormat(this Command.EditpkgOption ori, PackageDatabaseTablePackageItem item) {
            var obj = new PackageDatabaseTablePackageItem() {
                name = ori.Name == "~" ? item.name : ori.Name,
                aka = ori.Aka == "~" ? item.aka : ori.Aka,
                desc = ori.Desc == "~" ? item.desc : ori.Desc
            };

            try {
                obj.type = ori.Type == "~" ? item.type : (PackageType)(int.Parse(ori.Type));
            } catch {
                return (obj, false);
            }

            return (obj, true);
        }

        public static (PackageDatabaseTableVersionItem res, bool status) ToDatabaseFormat(this Command.AddverOption ori) {
            var obj = new PackageDatabaseTableVersionItem() {
                name = ori.Name,
                parent = ori.Parent,
                additional_desc = ori.AdditionalDesc,
                dependency = ori.Dependency,
                conflict = ori.Conflict,
                require_decompress = ori.RequireDecompress,
                hash = SignVerifyHelper.GetFileHash(ori.PackagePath)
            };

            try {
                if (ori.Timestamp == "+") obj.timestamp = DateTime.Now.ToUNIXTimestamp();
                else obj.timestamp = long.Parse(ori.Timestamp);

                obj.suit_os = (OSType)(int.Parse(ori.SuitOS));
                obj.reverse_conflict = bool.Parse(ori.ReverseConflict);
                obj.internal_script = bool.Parse(ori.ReverseConflict);
            } catch {
                return (obj, false);
            }

            return (obj, true);
        }

        public static (PackageDatabaseTableVersionItem res, bool status) ToDatabaseFormat(this Command.EditverOption ori, PackageDatabaseTableVersionItem item) {
            var obj = new PackageDatabaseTableVersionItem() {
                name = ori.Name == "~" ? item.name : ori.Name,
                parent = ori.Parent == "~" ? item.parent : ori.Parent,
                additional_desc = ori.AdditionalDesc == "~" ? item.additional_desc : ori.AdditionalDesc,
                dependency = ori.Dependency == "~" ? item.dependency : ori.Dependency,
                conflict = ori.Conflict == "~" ? item.conflict : ori.Conflict,
                require_decompress = ori.RequireDecompress == "~" ? item.require_decompress : ori.RequireDecompress,
                hash = ori.PackagePath == "~" ? item.hash : SignVerifyHelper.GetFileHash(ori.PackagePath)
            };

            try {
                if (ori.Timestamp == "+") obj.timestamp = DateTime.Now.ToUNIXTimestamp();
                else if (ori.Timestamp == "~") obj.timestamp = item.timestamp;
                else obj.timestamp = long.Parse(ori.Timestamp);

                obj.suit_os = ori.SuitOS == "~" ? item.suit_os : (OSType)(int.Parse(ori.SuitOS));
                obj.reverse_conflict = ori.ReverseConflict == "~" ? item.reverse_conflict : bool.Parse(ori.ReverseConflict);
                obj.internal_script = ori.InternalScript == "~" ? item.internal_script : bool.Parse(ori.ReverseConflict);
            } catch {
                return (obj, false);
            }

            return (obj, true);
        }

    }


}
