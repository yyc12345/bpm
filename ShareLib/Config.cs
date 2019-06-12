using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShareLib;

namespace ShareLib {
    public class Config {

        public Config(string fileName, Dictionary<string, string> defaultValue) {
            _fileName = fileName;
            _defaultValue = JsonConvert.SerializeObject(defaultValue);

            Configuration = Read();
        }

        string _fileName;
        string _defaultValue;
        public Dictionary<string, string> Configuration;

        Dictionary<string, string> Read() {
            if (!File.Exists(Information.WorkPath.Enter(_fileName).Path))
                Init();

            Dictionary<string, string> data;
            using (StreamReader fs = new StreamReader(Information.WorkPath.Enter(_fileName).Path, Encoding.UTF8)) {
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fs.ReadToEnd());
                fs.Close();
            }
            return data;
        }

        void Init() {
            using (StreamWriter fs = new StreamWriter(Information.WorkPath.Enter("config.cfg").Path, false, Encoding.UTF8)) {
                fs.Write(_defaultValue);
                fs.Close();
            }
        }

        public void Save() {
            using (StreamWriter fs = new StreamWriter(Information.WorkPath.Enter(_fileName).Path, false, Encoding.UTF8)) {
                fs.Write(JsonConvert.SerializeObject(this.Configuration));
                fs.Close();
            }
        }

    }

}
