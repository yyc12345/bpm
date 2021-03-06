﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Zip;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Install {

        public static void Core(string packageName) {
            
            Console.WriteLine(I18N.Core("Install_CollectingPackageInfo"));

            //=================================================================pre-process
            //name is legal
            var packageDbConn = new Database();
            packageDbConn.Open();

            var reader = (from _i in packageDbConn.CoreDbContext.package
                          where _i.name == (packageName.Contains("@") ? packageName.Split('@', StringSplitOptions.None)[0] : packageName)
                          select _i).ToList();
            if (!reader.Any()) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NoMatchedPackage"), ConsoleColor.Red);
                return;
            }
            packageName = GetVersionNatrually(packageName, packageDbConn);

            //is installed ?
            var installFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);
            if (installFolder.GetDirectories($"{packageName}").Any()) {
                ConsoleAssistance.WriteLine(I18N.Core("Install_InstalledPackage"), ConsoleColor.Red);
                return;
            }

            //====================================================================get-info
            //get denpendency tree
            Console.WriteLine(I18N.Core("Install_BuildingDependencyTree"));
            var cache1 = GetPackageInfo(packageName, packageDbConn);
            if (!cache1.res) {
                ConsoleAssistance.WriteLine(I18N.Core("General_NetworkError"), ConsoleColor.Red);
                return;
            }

            //conflict detect
            Console.WriteLine(I18N.Core("Install_DetectConflict"));
            var cache2 = DetectConflict(cache1.topologyMap, packageDbConn);
            if (!cache2.status) {
                ConsoleAssistance.WriteLine(I18N.Core("Install_SelfConflict"), ConsoleColor.Red);
                return;
            }

            //sort dependency
            Console.WriteLine(I18N.Core("Install_SortingDependencyTree"));
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
                ConsoleAssistance.WriteLine(I18N.Core("Install_CloseLoop"), ConsoleColor.Red);
                return;
            }

            //remove installed package
            var realPackage = new List<string>(cache3.res);

            foreach (var item in installFolder.GetDirectories()) {
                if (realPackage.Contains(item.Name))
                    realPackage.Remove(item.Name);
            }

            packageDbConn.Close();

            //sort removed package
            var sort_recorder = new Record();
            sort_recorder.Init();
            var realRemovedPackage = sort_recorder.SortPackage(cache2.res);
            sort_recorder.Save();
            //=======================================================================output

            ConsoleAssistance.WriteLine(I18N.Core("Install_InstallList"), ConsoleColor.Yellow);
            foreach (var item in realPackage) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");

            ConsoleAssistance.WriteLine(I18N.Core("Install_RemoveList"), ConsoleColor.Yellow);
            if (realRemovedPackage.Count == 0) ConsoleAssistance.WriteLine(I18N.Core("General_None"), ConsoleColor.Yellow);
            foreach (var item in realRemovedPackage) {
                Console.WriteLine(item);
            }
            Console.WriteLine("");

            ConsoleAssistance.Write(I18N.Core("General_Continue"), ConsoleColor.Yellow);
            if (Console.ReadLine().ToUpper() != "Y") {
                ConsoleAssistance.WriteLine(I18N.Core("General_CancelOperation"), ConsoleColor.Red);
                return;
            }

            //============================================================================install
            //remove
            Console.WriteLine(I18N.Core("Install_RemovingSelectedPackage"));
            Remove.RealRemove(realRemovedPackage);

            //install
            Console.WriteLine(I18N.Core("Install_InstallingSelectedPackage"));
            //ready recorder
            var recorder = new Record();
            recorder.Init();

            var zipExtractor = new FastZip();

            foreach (var item in realPackage) {
                Console.WriteLine(I18N.Core("Install_InstallItem", item));
                //download
                var downloadRes = Download.DownloadPackage(item);
                Console.WriteLine(Download.JudgeDownloadResult(downloadRes));

                if (downloadRes != Download.DownloadResult.OK && downloadRes != Download.DownloadResult.ExistedLocalFile) {
                    ConsoleAssistance.WriteLine(I18N.Core("General_NetworkError"), ConsoleColor.Red);
                    return;
                }

                //remove decompress folder
                Directory.Delete(Information.WorkPath.Enter("cache").Enter("decompress").Path, true);
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("decompress").Path);

                //decompress
                Console.WriteLine(I18N.Core("Install_ExtractItem", item));

                zipExtractor.ExtractZip(Information.WorkPath.Enter("cache").Enter("download").Enter(item + ".zip").Path, Information.WorkPath.Enter("cache").Enter("decompress").Path, "");

                Console.WriteLine(I18N.Core("Install_RunScriptItem", item));
                var cacheRes = ScriptInvoker.Core(Information.WorkPath.Enter("cache").Enter("decompress").Path, ScriptInvoker.InvokeMethod.Install, "");
                if (!cacheRes.status) {
                    ConsoleAssistance.WriteLine(I18N.Core("General_ScriptError"), ConsoleColor.Red);
                    ConsoleAssistance.WriteLine(cacheRes.desc, ConsoleColor.Red);
                    //save installed package info
                    recorder.Save();
                    return;
                }

                //record first and copy folders
                Console.WriteLine(I18N.Core("Install_RecordItem", item));
                recorder.Add(item);
                PackageAssistance.DirectoryCopy(Information.WorkPath.Enter("cache").Enter("decompress").Path, Information.WorkPath.Enter("cache").Enter("installed").Enter(item).Path, true);

                Console.WriteLine(I18N.Core("Install_Success", item));

            }

            //close recorder
            recorder.Save();

            ConsoleAssistance.WriteLine(I18N.Core("General_AllOperationDown"), ConsoleColor.Yellow);

        }

        static string GetVersionNatrually(string packageName, Database sql) {
            if (packageName.Contains("@")) return packageName;
            var installFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);
            var cache = installFolder.GetDirectories($"{packageName}@*");
            if (!cache.Any()) return GetTopVersion(packageName, sql);
            else return cache[0].Name;

        }

        static string GetTopVersion(string packageNameWithoutVersion, Database sql) {
            if (packageNameWithoutVersion.Contains("@")) return packageNameWithoutVersion;
            var reader = (from _i in sql.CoreDbContext.package
                          where _i.name == packageNameWithoutVersion
                          select _i).ToList();
            return packageNameWithoutVersion + "@" + reader[0].version.Split(',').Last();
        }

        static List<string> GetAllVersion(string packageName, Database sql) {
            if (packageName.Contains("@")) return new List<string>() { packageName };
            var reader = (from _i in sql.CoreDbContext.package
                          where _i.name == packageName
                          select _i).ToList();
            var res = new List<string>();
            foreach (var item in reader[0].version.Split(',')) {
                res.Add($"{packageName}@{item}");
            }
            return res;
        }

        static (bool res, Dictionary<string, PackageJson> topologyMap) GetPackageInfo(string corePackage, Database sql) {

            Download.DownloadResult res;
            //corePackage = GetTopVersion(corePackage, sql);

            Dictionary<string, PackageJson> result = new Dictionary<string, PackageJson>();
            List<string> nowList = new List<string>() { corePackage };
            while (nowList.Count != 0) {
                var nextList = new List<string>();

                foreach (var item in nowList) {
                    //download
                    Console.WriteLine(I18N.Core("Install_DownloadInfo", item));
                    res = Download.DownloadPackageInfo(item);
                    Console.WriteLine(Download.JudgeDownloadResult(res));
                    if (res != Download.DownloadResult.OK && res != Download.DownloadResult.ExistedLocalFile)
                        return (false, null);

                    var fs = new StreamReader(Information.WorkPath.Enter("cache").Enter("dependency").Enter(item + ".json").Path, Encoding.UTF8);
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

            return (true, result);
        }

        static (bool status, List<string> res) DetectConflict(Dictionary<string, PackageJson> packageList, Database sql) {

            //detect themselves=================================================
            var self = packageList.Keys.ToList();
            var self_conflict = new HashSet<string>();
            var self_compatible = new HashSet<string>();
            foreach (var key in packageList.Keys) {
                var item = packageList[key];
                foreach (var item2 in item.conflict) {
                    foreach (var item3 in GetAllVersion(item2, sql)) {
                        if (item.reverseConflict) self_compatible.Add(item3);
                        else self_conflict.Add(item3);
                    }
                }

                //compatible with themselves
                if (item.reverseConflict) self_compatible.Add(key);
            }

            if (self.Intersect(self_conflict).Any()) return (false, null);
            if (self_compatible.Count != 0 && !self.All((a) => { return self_compatible.Contains(a); })) return (false, null);

            //detect installed package=========================================
            //get installed package list
            var installed = new List<string>();
            var installFolder = new DirectoryInfo(Information.WorkPath.Enter("cache").Enter("installed").Path);
            foreach (var item in installFolder.GetDirectories()) {
                installed.Add(item.Name);
            }

            //self --detect--> installed
            var res = (from item in installed
                       where self_conflict.Contains(item) || (self_compatible.Count != 0 && !self_compatible.Contains(item))
                       select item).ToList();

            //installed --detect--> self
            var realRes = new HashSet<string>(res);
            foreach (var item in installed) {
                PackageJson jsonCache;
                using (var fs = new StreamReader(Information.WorkPath.Enter("cache").Enter("dependency").Enter(item + ".json").Path, Encoding.UTF8)) {
                    jsonCache = JsonConvert.DeserializeObject<PackageJson>(fs.ReadToEnd());
                    fs.Close();
                }

                var realConflict = new List<string>();
                foreach (var outter in jsonCache.conflict) {
                    realConflict.AddRange(GetAllVersion(outter, sql));
                }
                if (jsonCache.reverseConflict) {
                    var cacheList = new List<string>(realConflict);
                    cacheList.Add(item);
                    if (!self.All((a) => { return cacheList.Contains(a); })) realRes.Add(item);
                } else {
                    if (self.Intersect(realConflict).Any()) realRes.Add(item);
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
