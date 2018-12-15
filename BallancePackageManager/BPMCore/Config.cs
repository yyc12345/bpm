using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    public static class Config {

        public static Dictionary<string, string> Read() {
            if (!File.Exists(Information.WorkPath.Enter("config.cfg").Path))
                Init();

            Dictionary<string, string> data;
            using (StreamReader fs = new StreamReader(Information.WorkPath.Enter("config.cfg").Path, Encoding.UTF8)) {
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fs.ReadToEnd());
                fs.Close();
            }
            return data;
        }

        static void Init() {
            using (StreamWriter fs = new StreamWriter(Information.WorkPath.Enter("config.cfg").Path, false, Encoding.UTF8)) {
                var cache = new Dictionary<string, string>() {
                    {"Sources" , "yyc.bkt.moe:3850" },
                    {"GamePath" , "" },
                    {"Language", "en-us" }
                };
                fs.Write(JsonConvert.SerializeObject(cache));
                fs.Close();
            }
        }

        public static void Core() {
            var cache = Read();
            OutputStruct(cache);
            return;
        }
        public static void Core(string itemName) {
            var cache = Read();
            if (cache.Keys.Contains(itemName)) Console.WriteLine(cache[itemName]);
            else ConsoleAssistance.WriteLine(I18N.Core("Config_InvalidConfig"), ConsoleColor.Red);
        }
        public static void Core(string itemName, string newValue) {
            var cache = Read();
            if (cache.Keys.Contains(itemName)) {
                cache[itemName] = newValue;
                Save(cache);
                Console.WriteLine(I18N.Core("Config_AppliedNewConfig"));
            } else ConsoleAssistance.WriteLine(I18N.Core("Config_InvalidConfig"), ConsoleColor.Red);
        }

        public static void Save(Dictionary<string, string> config) {
            using (StreamWriter fs = new StreamWriter(Information.WorkPath.Enter("config.cfg").Path, false, Encoding.UTF8)) {
                fs.Write(JsonConvert.SerializeObject(config));
                fs.Close();
            }
        }

        static void OutputStruct(Dictionary<string,string> config) {
            foreach (var item in config.Keys) {
                Console.Write($"{item}: ");
                Console.Write($"{config[item]}\n");
            }
        }

    }

}
