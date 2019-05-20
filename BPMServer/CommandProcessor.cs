using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommandLine;
using ShareLib;

namespace BPMServer.Command {
    public static class CommandProcessor {

        static Parser parser = new Parser(ps => {
            ps.AutoHelp = false;
            ps.AutoVersion = false;
            ps.HelpWriter = null;
            ps.EnableDashDash = true;
        });

        public static bool Process(string command) {
            return parser.ParseArguments<ExitOption, ConfigOption, ModeOption, ClientOption, UpdateOption>(
                CommandSplitter.Split(command))
                .MapResult(
                (ExitOption opt) => {
                    if (opt.IsForce) {
                        return true;
                    } else {
                        if (!General.IsMaintaining) {
                            General.CoreTcpProcessor.StopListen();
                            Console.WriteLine("Waiting the release of resources...");
                            if (General.ManualResetEventList.Count != 0)
                                WaitHandle.WaitAll(General.ManualResetEventList.ToArray());
                        } else General.GeneralDatabase.Close();
                        return false;
                    }

                },
                (ConfigOption opt) => {
                    if (opt.Key is null) 
                        Config.Core();
                    else {
                        if (opt.NewValue is null)
                            Config.Core(opt.Key);
                        else
                            Config.Core(opt.Key, opt.NewValue);
                    }
                    return false;
                },
                (ModeOption opt) => {
                    if (opt.Mode == "maintain") {
                        if (General.IsMaintaining) ConsoleAssistance.WriteLine("You are in maintain mode now.", ConsoleColor.Red);
                        else {
                            General.CoreTcpProcessor.StopListen();
                            Console.WriteLine("Waiting the release of resources...");
                            if (General.ManualResetEventList.Count != 0)
                                WaitHandle.WaitAll(General.ManualResetEventList.ToArray());

                            General.GeneralDatabase.Open();

                            General.IsMaintaining = true;
                            ConsoleAssistance.WriteLine("Switch to maintain mode successfully.", ConsoleColor.Yellow);
                        }
                    } else if (opt.Mode == "running") {
                        if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now.", ConsoleColor.Red);
                        else {
                            General.GeneralDatabase.Close();
                            General.CoreTcpProcessor.StartListen();

                            General.IsMaintaining = false;
                            ConsoleAssistance.WriteLine("Switch to running mode successfully.", ConsoleColor.Yellow);
                        }
                    } else ConsoleAssistance.WriteLine("Invalid parameter", ConsoleColor.Red);
                    return false;
                },
                (ClientOption opt) => {
                    if (!General.IsMaintaining) ConsoleAssistance.WriteLine($"Current client: {General.ManualResetEventList.Count}", ConsoleColor.Yellow);
                    else ConsoleAssistance.WriteLine("Server is being maintained. There are no any client.", ConsoleColor.Red);
                    return false;
                },
                (UpdateOption opt) => {
                    ;//todo: need finish update function
                    return false;
                },
                errs => { OutputHelp(); return false; });
        }

        static void OutputHelp() {
            Console.WriteLine("Usage: command [options]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("General command");
            Console.WriteLine("\texit [-f] - exit server. if you choose -f switch, it will kill the server without any hesitation (for emergency). you will lost each of your modifications");
            Console.WriteLine("\tconfig - edit server config(config will be applied in the next startup)");
            Console.WriteLine("\tmode [maintain | running] - switch server mode");
            Console.WriteLine("");
            Console.WriteLine("Running mode command");
            Console.WriteLine("\tclient - list the number of client which is connecting this server");
            Console.WriteLine("");
            Console.WriteLine("Maintain mode command");
            Console.WriteLine("\tupdate new_database package_folder - update the whole of this server's data");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }

    #region command structure

    [Verb("exit")]
    public class ExitOption {
        [Option('f', "force", Default = false)]
        public bool IsForce { get; set; }
    }

    [Verb("config")]
    public class ConfigOption {
        [Value(0)]
        public string Key { get; set; }
        [Value(1)]
        public string NewValue { get; set; }
    }

    [Verb("mode")]
    public class ModeOption {
        [Value(0, Required = true)]
        public string Mode { get; set; }
    }

    [Verb("client")]
    public class ClientOption {

    }

    [Verb("update")]
    public class UpdateOption {
        [Value(0, Required = true)]
        public string NewDatabasePath { get; set; }
        [Value(1, Required = true)]
        public string PackageFolderPath { get; set; }
    }

    #endregion

}
