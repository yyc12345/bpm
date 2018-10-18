using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPMServer {
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
            param = CommandSplitter.SplitCommand(String.Join(" ", param));

            void runCode() {
                switch (head) {
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
                    case "addpkg":
                        if (param.Count != 6) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.AddPackage(param[0], param[1], param[2], param[3], param[4], param[5]);
                        break;
                    case "addver":
                        if (param.Count != 4) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.AddVersion(param[0], param[1], param[2], param[3]);
                        break;
                    case "delpkg":
                        if (param.Count != 2) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.RemovePackage(param[0], param[1]);
                        break;
                    case "delver":
                        if (param.Count != 2) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.RemoveVersion(param[0], param[1]);
                        break;
                    default:
                        ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                        break;
                }
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
            Console.WriteLine("  exit - exit server");
            Console.WriteLine("  config - edit server config");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }
}
