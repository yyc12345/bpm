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

        public List<Search_ExportItem> Search_Export;

        public void Search_Wrapper(bool omitVersion, string packageName) {
            if (!CheckStatus(BPMInstanceMethod.Search, BPMInstanceStatus.Ready)) return;
            if (!HaveDatabase(BPMInstanceMethod.Search)) return;
            CurrentStatus = BPMInstanceStatus.Working;
            Search_Core(omitVersion, packageName);
            CurrentStatus = BPMInstanceStatus.Ready;
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.Search);
        }

        private void Search_Core(bool omitVersion, string packageName) {

            //query
            var packageDbConn = new PackageDatabase();
            packageDbConn.Open();

            OnBPMInstanceEvent_Message(BPMInstanceMethod.Search, "Searching package table...");//todo: i18n
            //query package table first
            var packageTableReader = from item in packageDbConn.CoreDbContext.package
                                     where EF.Functions.Like(item.name, $"%{packageName}%") || EF.Functions.Like(item.desc, $"%{packageName}%") || EF.Functions.Like(item.aka, $"%{packageName}%")
                                     select item;

            //write into export value
            List<string> exportCache = new List<string>();
            foreach (var item in packageTableReader)
                exportCache.Add(item.name);

            OnBPMInstanceEvent_Message(BPMInstanceMethod.Search, "Searching version table...");//todo: i18n
            //query version table
            if (!omitVersion) {
                var versionTableReader = from item in packageDbConn.CoreDbContext.version
                                         where EF.Functions.Like(item.name, $"%{packageName}%") || EF.Functions.Like(item.additional_desc, $"%{packageName}%")
                                         group item by item.parent;

                foreach (var item in versionTableReader)
                    if (!exportCache.Contains(item.Key))
                        exportCache.Add(item.Key);
            }

            OnBPMInstanceEvent_Message(BPMInstanceMethod.Search, "Collecting data...");//todo: i18n
            //query info
            var infomationReader = from item in packageDbConn.CoreDbContext.package
                                   where exportCache.Contains(item.name)
                                   select item;


            OnBPMInstanceEvent_Message(BPMInstanceMethod.Search, "Querying install status...");//todo: i18n
            //query install package
            var packageDbConn2 = new InstalledDatabase();
            packageDbConn2.Open();
            var installedReader = (from item in packageDbConn2.CoreDbContext.installed
                                   where exportCache.Contains(item.name.Split('@')[0])
                                   select item.name.Split('@')[0]).ToList();


            OnBPMInstanceEvent_Message(BPMInstanceMethod.Search, "Building query result...");//todo: i18n
            //construct export data
            Search_Export = new List<Search_ExportItem>();
            foreach (var item in infomationReader) {
                Search_Export.Add(new Search_ExportItem() {
                    Name = item.name,
                    Description = item.desc,
                    Aka = item.aka,
                    IsInstalled = installedReader.Contains(item.name)
                });
            }

            packageDbConn.Close();
            packageDbConn2.Close();
        }
    }
}

namespace BallancePackageManager.ExportModule {

    public class Search_ExportItem {
        public string Name = "";
        public string Aka = "";
        public string Description = "";
        public bool IsInstalled = false;
    }

}
