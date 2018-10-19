using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using SevenZip;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace BallancePackageManager.BPMCore {
    public static class Install {

        public static void Core(string packageName) {

            Console.WriteLine("Collecting pakcage infomation...");

            //=================================================================pre-process
            //name is legal
            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            var cursor = new SQLiteCommand($"select * from package where name == \"{(packageName.Contains("@") ? packageName.Split('@')[0] : packageName)}\"", packageDbConn);
            var reader = cursor.ExecuteReader();
            if (!reader.Read()) {
                ConsoleAssistance.WriteLine("Package is not existed", ConsoleColor.Red);
                return;
            }
            packageName = GetVersionNatrually(packageName, packageDbConn);

            //is installed ?
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");
            if (installFolder.GetFiles($"{packageName}.json").Any()) {
                ConsoleAssistance.WriteLine("Package is installed", ConsoleColor.Red);
                return;
            }

            //====================================================================get-info
            //get denpendency tree
            Console.WriteLine("Building dependency tree...");
            var cache1 = GetPackageInfo(packageName, packageDbConn);

            //conflict detect
            Console.WriteLine("Detecting package conflict...");
            var cache2 = DetectConflict(cache1.topologyMap, packageDbConn);
            if (!cache2.status) {
                ConsoleAssistance.WriteLine("The package, which you want to install, is self-conflict", ConsoleColor.Red);
                return;
            }

            //sort dependency
            Console.WriteLine("Sorting dependency tree...");
            var cache1_1 = new Dictionary<string, List<string>>();
            foreach (var item in cache1.topologyMap.Keys) {
                var cache = new List<string>();
                foreach (var depItem in cache1.topologyMap[item].dependency.ToList()) {
                    cache.Add(GetTopVersion(depItem, packageDbConn));
                }
                cache1_1.Add(item, cache);
            }
            var cache3 = KahnTopologySort(cache1_1);
            if (!cache3.status) {
                ConsoleAssistance.WriteLine("Closed-loop package dependency", ConsoleColor.Red);
                return;
            }

            //todo: remove installed package


            //remove installed package
            var realPackage = new List<string>(cache3.res);

            foreach (var item in installFolder.GetFiles($"*.json")) {
                var cacheSplit = ConsoleAssistance.GetScriptInfo(item.Name);
                realPackage.Remove($"{cacheSplit.packageName}@{cacheSplit.version}");
            }          

            packageDbConn.Close();
            //=======================================================================output

            ConsoleAssistance.WriteLine("There are the packages which will be installed: ", ConsoleColor.Yellow);
            foreach (var item in realPackage) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");

            ConsoleAssistance.WriteLine("There are the packages which will be removed due to the conflict: ", ConsoleColor.Yellow);
            foreach (var item in cache2.res) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");

            ConsoleAssistance.Write("Are you sure that you want to continue (Y/N): ", ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine("You cancle the operation.", ConsoleColor.Red);
                return;
            }

            //============================================================================install
            //remove
            Console.WriteLine("Removing selected packages...");
            Remove.RealRemove(cache2.res);

            //install
            Console.WriteLine("Installing selected packages...");
            var gamePath = Config.Read()["GamePath"];
            ScriptEngine pyEngine = Python.CreateEngine();

            foreach (var item in realPackage) {
                //download
                var downloadRes = Download.DownloadPackage(item);
                Console.WriteLine(Download.JudgeDownloadResult(downloadRes));

                if (downloadRes != Download.DownloadResult.OK && downloadRes != Download.DownloadResult.ExistedLocalFile) {
                    ConsoleAssistance.WriteLine("A error occured. Operation is cancled", ConsoleColor.Red);
                    return;
                }

                //remove decompress folder
                Directory.Delete(ConsoleAssistance.WorkPath + @"cache\decompress", true);
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"cache\decompress");

                //decompress
                Console.WriteLine($"Extracting {item}...");
                using (SevenZipExtractor tmp = new SevenZipExtractor(ConsoleAssistance.WorkPath + @"cache\download" + item + ".7z")) {
                    tmp.ExtractArchive(ConsoleAssistance.WorkPath + @"cache\decompress");
                }

                Console.WriteLine($"Running {item}'s setup.py...");
                dynamic dd = pyEngine.ExecuteFile(ConsoleAssistance.WorkPath + @"cache\decompress\setup.py");
                bool cacheRes = dd.install(gamePath, ConsoleAssistance.WorkPath + @"cache\decompress", "");
                if (!cacheRes) {
                    ConsoleAssistance.WriteLine("Installer report a error. Operation is cancled", ConsoleColor.Red);
                    return;
                }

                Console.WriteLine($"Recording {item}'s info...");
                File.Copy(ConsoleAssistance.WorkPath + @"cache\decompress\setup.py", ConsoleAssistance.WorkPath + @"cache\installed\" + item + ".py");

                Console.WriteLine($"{item} is installed successfully");

            }

            ConsoleAssistance.WriteLine("All operation have done.", ConsoleColor.Yellow);

        }

        static string GetVersionNatrually(string packageName, SQLiteConnection sql) {
            if (packageName.Contains("@")) return packageName;
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");
            var cache = installFolder.GetFiles($"{packageName}@*.json");
            if (!cache.Any()) return GetTopVersion(packageName, sql);
            else {
                var cache2 = ConsoleAssistance.GetScriptInfo(cache[0].Name);
                return $"{cache2.packageName}@{cache2.version}";
            }
        }

        static string GetTopVersion(string packageNameWithoutVersion, SQLiteConnection sql) {
            if (packageNameWithoutVersion.Contains("@")) return packageNameWithoutVersion;
            var cursor = new SQLiteCommand($"select * from package where name == \"{packageNameWithoutVersion}\"", sql);
            var reader = cursor.ExecuteReader();
            reader.Read();
            return packageNameWithoutVersion + "@" + reader["version"].ToString().Split(',').Last();
        }

        static List<string> GetAllVersion(string packageName, SQLiteConnection sql) {
            if (packageName.Contains("@")) return new List<string>() { packageName };
            var cursor = new SQLiteCommand($"select * from package where name == \"{packageName}\"", sql);
            var reader = cursor.ExecuteReader();
            reader.Read();
            var res = new List<string>();
            foreach (var item in reader["version"].ToString().Split(',')) {
                res.Add($"{packageName}@{item}");
            }
            return res;
        }

        static (Download.DownloadResult res, Dictionary<string, PackageJson> topologyMap) GetPackageInfo(string corePackage, SQLiteConnection sql) {

            Download.DownloadResult res;
            //corePackage = GetTopVersion(corePackage, sql);

            Dictionary<string, PackageJson> result = new Dictionary<string, PackageJson>();
            List<string> nowList = new List<string>() { corePackage };
            while (nowList.Count != 0) {
                var nextList = new List<string>();

                foreach (var item in nowList) {
                    //download
                    Console.WriteLine($"Downloading {corePackage}'s infomation...");
                    res = Download.DownloadPackageInfo(corePackage);
                    Console.WriteLine(Download.JudgeDownloadResult(res));
                    if (res != Download.DownloadResult.OK && res != Download.DownloadResult.ExistedLocalFile)
                        return (res, null);

                    var fs = new StreamReader(ConsoleAssistance.WorkPath + @"cache\dependency\" + item + ".json", Encoding.UTF8);
                    var cache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
                    fs.Close();
                    fs.Dispose();

                    //add item
                    result.Add(item, cache);

                    foreach (var dependency in cache.dependency) {
                        var dependencyPackage = GetVersionNatrually(dependency, sql);
                        if (!result.ContainsKey(dependencyPackage)) nextList.Add(dependencyPackage);
                    }
                }

                nowList = new List<string>(nextList);
            }

            return (Download.DownloadResult.OK, result);
        }

        static (bool status, List<string> res) DetectConflict(Dictionary<string, PackageJson> packageList, SQLiteConnection sql) {

            //detect themselves=================================================
            var self = packageList.Keys.ToList();
            var self_conflict = new HashSet<string>();
            var self_compatible = new HashSet<string>();
            foreach (var item in packageList.Values) {
                foreach (var item2 in item.conflict) {
                    foreach (var item3 in GetAllVersion(item2, sql)) {
                        if (item.reverseConflict) self_compatible.Add(item3);
                        else self_conflict.Add(item3);
                    }
                }
            }

            if (self.Intersect(self_conflict).Any()) return (false, null);
            if (!self.All((a) => { return self_compatible.Contains(a); })) return (false, null);

            //detect installed package=========================================
            //get installed package list
            var installed = new List<string>();
            var installFolder = new DirectoryInfo(ConsoleAssistance.WorkPath + @"\cache\installed");
            foreach (var item in installFolder.GetFiles("*.json")) {
                var cache = ConsoleAssistance.GetScriptInfo(item.Name);
                installed.Add($"{cache.packageName}@{cache.version}");
            }

            //self --detect--> installed
            var res = (from item in installed
                       where self_conflict.Contains(item) || !self_compatible.Contains(item)
                       select item).ToList();

            //installed --detect--> self
            var realRes = new HashSet<string>(res);
            foreach (var item in installed) {
                PackageJson jsonCache;
                using (var fs = new StreamReader(ConsoleAssistance.WorkPath + @"\cache\installed\" + item + ".json", Encoding.UTF8)) {
                    jsonCache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
                    fs.Close();
                }

                if (jsonCache.reverseConflict) {
                    if (!self.All((a) => { return jsonCache.conflict.Contains(a); })) realRes.Add(item);
                } else {
                    if (self.Intersect(jsonCache.conflict).Any()) realRes.Add(item);
                }
            }

            return (true, realRes.ToList());

        }

        static (bool status, List<string> res) KahnTopologySort(Dictionary<string, List<string>> topologyMap) {
            var result = new List<string>();

            while (topologyMap.Any()) {
                var selectedItem = "";
                foreach (var item in topologyMap.Keys) {
                    //get 0 depth item
                    if (!topologyMap[item].Any()) {
                        result.Add(item);
                        selectedItem = item;
                        goto success;
                    }
                }

                //no matched item. closed-loop dependency
                return (false, null);

                success:
                //remove relative branch
                topologyMap.Remove(selectedItem);
                foreach (var item in topologyMap.Values) {
                    item.Remove(selectedItem);
                }
            }

            return (true, result);
        }


    }
}
