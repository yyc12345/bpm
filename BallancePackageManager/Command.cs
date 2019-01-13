using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallancePackageManager.BPMCore;
using System.Threading;
using ShareLib;
using System.IO;

namespace BallancePackageManager {
    public static class Command {
        public static void CommandExecute(string[] command) {
            if (command.Length == 0) {
                ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidCommand"), ConsoleColor.Red);
                OutputHelp();
                return;
            }

            //read status
            var _config = Config.Read();
            bool HaveGamePath = _config["GamePath"] != "";
            bool HaveDatabase = File.Exists(Information.WorkPath.Enter("package.db").Path);

            var param = new List<string>(command);
            var head = param[0];
            param.RemoveAt(0);

            void runCode() {
                switch (head) {
                    case "update":
                        if (param.Count() != 0) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Update.Core();
                        break;
                    case "search":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (param.Count() == 0) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Search.Core(param);
                        break;
                    case "install":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (!HaveGamePath) ConsoleAssistance.WriteLine(I18N.Core("General_NoGamePath"), ConsoleColor.Red);
                        else if (param.Count() != 1) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Install.Core(param[0]);
                        break;
                    case "list":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (param.Count() != 0) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else List.Core();
                        break;
                    case "remove":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (!HaveGamePath) ConsoleAssistance.WriteLine(I18N.Core("General_NoGamePath"), ConsoleColor.Red);
                        else if (param.Count() != 1) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Remove.Core(param[0]);
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
                                ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                                break;
                        }
                        break;
                    case "show":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (param.Count != 1) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Show.Core(param[0]);
                        break;
                    case "deploy":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (!HaveGamePath) ConsoleAssistance.WriteLine(I18N.Core("General_NoGamePath"), ConsoleColor.Red);
                        else if (param.Count != 2) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Deploy.Core(param[0], param[1]);
                        break;
                    case "guide":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (!HaveGamePath) ConsoleAssistance.WriteLine(I18N.Core("General_NoGamePath"), ConsoleColor.Red);
                        else if (param.Count != 1) ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        else Guide.Core(param[0]);
                        break;
                    case "clean":
                        if (!HaveDatabase) ConsoleAssistance.WriteLine(I18N.Core("General_NoDatabase"), ConsoleColor.Red);
                        else if (param.Count == 1) Clean.Core(param[0]);
                        else if (param.Count == 0) Clean.Core();
                        else ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidParameterCount"), ConsoleColor.Red);
                        break;
                    case "help":
                        OutputHelp();
                        break;
                    default:
                        ConsoleAssistance.WriteLine(I18N.Core("Command_InvalidCommand"), ConsoleColor.Red);
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
            Console.WriteLine(I18N.Core("Help_1"));
            Console.WriteLine("");
            Console.WriteLine(I18N.Core("Help_2"));
            Console.WriteLine("");
            Console.WriteLine(I18N.Core("Help_3"));
            Console.WriteLine(I18N.Core("Help_4"));
            Console.WriteLine(I18N.Core("Help_5"));
            Console.WriteLine(I18N.Core("Help_6"));
            Console.WriteLine(I18N.Core("Help_7"));
            Console.WriteLine(I18N.Core("Help_8"));
            //Console.WriteLine("  autoremove - Remove automatically all unused packages");
            Console.WriteLine(I18N.Core("Help_9"));
            Console.WriteLine(I18N.Core("Help_10"));
            //Console.WriteLine("  full-upgrade - upgrade the system by removing/installing/upgrading packages");
            //Console.WriteLine("  restore - remove all installed package");
            Console.WriteLine(I18N.Core("Help_11"));
            Console.WriteLine(I18N.Core("Help_13"));
            Console.WriteLine(I18N.Core("Help_15"));
            Console.WriteLine(I18N.Core("Help_14"));
            //Console.WriteLine("  exit - exit bpm");
            Console.WriteLine("");
            Console.WriteLine(I18N.Core("Help_12"));
        }
    }
}
