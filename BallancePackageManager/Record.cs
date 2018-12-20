using System;
using System.Collections.Generic;
using System.Text;
using ShareLib;
using System.IO;
using Newtonsoft.Json;

namespace BallancePackageManager {
    public class Record {

        List<string> sortedInstalledPackage = new List<string>();

        public void Init() {
            var file = Information.WorkPath.Enter("InstallRecord.db").Path;
            if (File.Exists(file)) {
                var fs = new StreamReader(file, Encoding.UTF8);
                sortedInstalledPackage = JsonConvert.DeserializeObject<List<string>>(fs.ReadToEnd());
                fs.Close();
                fs.Dispose();
            } else this.Save();
        }

        public void Save() {
            var file = Information.WorkPath.Enter("InstallRecord.db").Path;
            var fs = new StreamWriter(file, false, Encoding.UTF8);
            fs.Write(JsonConvert.SerializeObject(sortedInstalledPackage));
            fs.Close();
            fs.Dispose();
        }

        public void Add(string item) {
            sortedInstalledPackage.Add(item);
        }

        public void Remove(string item) {
            sortedInstalledPackage.Remove(item);
        }

        public List<string> SortPackage(List<string> arr) {
            arr.Sort((x, y) => sortedInstalledPackage.IndexOf(x) - sortedInstalledPackage.IndexOf(y));
            return arr;
        }

    }
}
