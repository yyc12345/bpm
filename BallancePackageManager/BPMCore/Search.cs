using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ShareLib;
using BallancePackageManager.ExportModule;
using Microsoft.EntityFrameworkCore;

namespace BallancePackageManager {

    public partial class BPMInstance {

        public List<Search_PackageItem> Search_Export;

        public void Search_CoreWrapper(bool omitVersion, string packageName) {
            if (!CheckStatus(BPMInstanceMethod.Search, BPMInstanceStatus.Ready)) return;
            if (!HaveDatabase(BPMInstanceMethod.Search)) return;
            CurrentStatus = BPMInstanceStatus.Working;
            Search_Core(omitVersion, packageName);
            CurrentStatus = BPMInstanceStatus.Ready;
            BPMInstanceEvent_MethodDone?.Invoke(BPMInstanceMethod.Search);
        }

        private void Search_Core(bool omitVersion, string packageName) {

            //query
            var packageDbConn = new PackageDatabase();
            packageDbConn.Open();

            //query package table first
            var packageTableReader = from item in packageDbConn.CoreDbContext.package
                                     where EF.Functions.Like(item.name, $"%{packageName}%") || EF.Functions.Like(item.desc, $"%{packageName}%") || EF.Functions.Like(item.aka, $"%{packageName}%")
                                     select item;

            //write into export value
            List<string> exportCache = new List<string>();
            foreach (var item in packageTableReader)
                exportCache.Add(item.name);

            //query version table
            if (!omitVersion) {
                var versionTableReader = from item in packageDbConn.CoreDbContext.version
                                         where EF.Functions.Like(item.name, $"%{packageName}%") || EF.Functions.Like(item.additional_desc, $"%{packageName}%")
                                         group item by item.parent;

                foreach (var item in versionTableReader)
                    if (!exportCache.Contains(item.Key))
                        exportCache.Add(item.Key);
            }

            //query info
            var infomationReader = from item in packageDbConn.CoreDbContext.package
                                   where exportCache.Contains(item.name)
                                   select item;
            
            packageDbConn.Close();

            //query install package
            var packageDbConn2 = new InstalledDatabase();
            packageDbConn2.Open();
            var installedReader = (from item in packageDbConn2.CoreDbContext.installed
                                  where exportCache.Contains(item.name.Split('@')[0])
                                  select item.name.Split('@')[0]).ToList();

            packageDbConn2.Close();

            //construct export data
            Search_Export = new List<Search_PackageItem>();
            foreach(var item in infomationReader) {
                Search_Export.Add(new Search_PackageItem() {
                    Name = item.name,
                    Description = item.desc,
                    Aka=item.aka,
                    IsInstalled = installedReader.Contains(item.name)
                });
            }
            
        }
    }
}

namespace BallancePackageManager.ExportModule {

    public class Search_PackageItem {
        public string Name = "";
        public string Aka = "";
        public string Description = "";
        public bool IsInstalled = false;
    }

}
