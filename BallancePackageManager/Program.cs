using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;


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
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"cache\download"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"cache\download");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"cache\installed"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"cache\installed");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"cache\decompress"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"cache\decompress");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"cache\dependency"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"cache\dependency");

            var config = BPMCore.Config.Read();
            while (config["GamePath"] == "") {
                Console.WriteLine("Type a proper path to place Ballance and its package pls. :)");
                config["GamePath"] = Console.ReadLine();
            }
            BPMCore.Config.Save(config);
            
            //run
            Command.CommandExecute(args);

        }
    }
}
