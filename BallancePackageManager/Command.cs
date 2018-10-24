using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallancePackageManager.BPMCore;
using System.Threading;
using ShareLib;

namespace BallancePackageManager {
    public static class Command {
        public static void CommandExecute(string[] command) {
            if (command.Length == 0) {
                ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                OutputHelp();
                return;
            }

            var param = new List<string>(command);
            var head = param[0];
            param.RemoveAt(0);

            void runCode() {
                switch (head) {
                    case "update":
                        Update.Core();
                        break;
                    case "search":
                        if (param.Count() != 0)
                            Search.Core(param[0]);
                        break;
                    case "install":
                        if (param.Count() != 0)
                            Install.Core(param[0]);
                        break;
                    case "list":
                        BallancePackageManager.BPMCore.List.Core();
                        break;
                    case "remove":
                        if (param.Count() != 0)
                            Remove.Core(param[0]);
                        break;
                    case "config":
                        switch (param.Count) {
                            case 0:
                                Config.Core();
                                break;
                            case 1:
                                Config.Core(param[0]);
                                break;
                            case 2:
                                Config.Core(param[0], param[1]);
                                break;
                            default:
                                ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                                break;
                        }
                        break;
                    case "show":
                        if (param.Count != 0)
                            Show.Core(param[0]);
                        break;
                    case "deploy":
                        if (param.Count != 2) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                        else Deploy.Core(param[0], param[1]);
                        break;
                    case "help":
                        OutputHelp();
                        break;
                    default:
                        ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                        OutputHelp();
                        break;
                }

#if DEBUG
                Console.ReadKey();
#endif
            }

            Thread td = new Thread(runCode);
            td.IsBackground = false;
            td.Start();

        }

        static void OutputHelp() {
            Console.WriteLine("Usage: bpm option [command]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("  list - list packages based on package names");
            Console.WriteLine("  search - search in package descriptions");
            Console.WriteLine("  show - show package details");
            Console.WriteLine("  install - install packages");
            Console.WriteLine("  remove - remove packages");
            //Console.WriteLine("  autoremove - Remove automatically all unused packages");
            Console.WriteLine("  update - update list of available packages");
            Console.WriteLine("  deploy - deploy package (especially for map and resources)");
            //Console.WriteLine("  full-upgrade - upgrade the system by removing/installing/upgrading packages");
            //Console.WriteLine("  restore - remove all installed package");
            Console.WriteLine("  config - edit the config");
            //Console.WriteLine("  exit - exit bpm");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }
}
