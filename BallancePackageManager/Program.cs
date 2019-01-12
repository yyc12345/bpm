using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using ShareLib;

namespace BallancePackageManager {
    class Program {
        static void Main(string[] args) {

            /*
            var gamePath = "";

            var config = BPMCore.Config.Read();
            if (config["GamePath"] != "") {
                gamePath = config["GamePath"];
                goto certificate;
            }

            //detect game folder
            
            try {
                var key = Registry.LocalMachine;
                RegistryKey coreKey;
                if (Environment.Is64BitOperatingSystem) {
                    coreKey = key.OpenSubKey(@"SOFTWARE\Wow6432Node\ballance\Settings");
                } else {
                    coreKey = key.OpenSubKey(@"SOFTWARE\ballance\Settings");
                }

                gamePath = coreKey.GetValue("TargetDir").ToString();
                if (gamePath == "System.String[]") {
                    var realData = (string[])coreKey.GetValue("TargetDir");
                    gamePath = realData[0];
                }

            } catch (Exception) {
                //error
                gamePath = "";
            }

            certificate:
            

            if (gamePath != "")
                gamePath = gamePath[gamePath.Length - 1] == '\\' ? gamePath : gamePath + "\\";

            while (gamePath == "" || !File.Exists(gamePath + "Startup.exe")) {
                Console.WriteLine("Fail to detect Ballance install folder. Pls type your Ballance install path.");
                gamePath = Console.ReadLine();
            }

            if (gamePath != config["GamePath"]) {
                config["GamePath"] = gamePath;
                BPMCore.Config.Save(config);
            }
            */

            //detect local cache folder
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("download").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("download").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("installed").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("installed").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("decompress").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("decompress").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("dependency").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("dependency").Path);

            //init i18n
            I18N.Init(BPMCore.Config.Read()["Language"]);

            /*
            var config = BPMCore.Config.Read();
            while (config["GamePath"] == "") {
                Console.WriteLine(I18N.Core("Init_TypePath"));
                var cache = new FilePathBuilder(Console.ReadLine(), Information.OS);
                config["GamePath"] = cache.Path;
            }
            BPMCore.Config.Save(config);
            */
            
            //run
            Command.CommandExecute(args);

        }
    }
}
