using System;
using System.Collections.Generic;
using System.Linq;
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

            return parser.ParseArguments<ExitOption, ConfigOption, SwitchOption, ClientOption, ImportOption, LsOption, ShowOption, AddpkgOption, EditpkgOption, DelpkgOption, AddverOption, EditverOption, DelverOption, HelpOption>(
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

                        General.RecordFileManager.Close();
                        return true;
                    }

                },
                (ConfigOption opt) => {
                    if (opt.Key is null)
                        foreach (var item in General.ConfigManager.Configuration.Keys) {
                            Console.Write($"{item}: ");
                            Console.Write($"{General.ConfigManager.Configuration[item]}\n");
                        } else {
                        if (opt.NewValue is null) {
                            if (General.ConfigManager.Configuration.Keys.Contains(opt.Key))
                                Console.WriteLine(General.ConfigManager.Configuration[opt.Key]);
                        } else {
                            if (General.ConfigManager.Configuration.Keys.Contains(opt.Key)) {
                                General.ConfigManager.Configuration[opt.Key] = opt.NewValue;
                                General.ConfigManager.Save();
                                Console.WriteLine("New value has been applied");
                            }
                        }

                    }
                    return false;
                },
                (SwitchOption opt) => {
                    if (!General.IsMaintaining) {
                        General.CoreTcpProcessor.StopListen();
                        Console.WriteLine("Waiting the release of resources...");
                        if (General.ManualResetEventList.Count != 0)
                            WaitHandle.WaitAll(General.ManualResetEventList.ToArray());

                        General.GeneralDatabase.Open();

                        General.IsMaintaining = true;
                        ConsoleAssistance.WriteLine("Switch to maintain mode successfully.", ConsoleColor.Yellow);

                    } else {
                        General.GeneralDatabase.Close();
                        //force update verify code
                        ConsoleAssistance.WriteLine("Updating verify code....", ConsoleColor.White);
                        General.VerifyBytes = SignVerifyHelper.SignData(Information.WorkPath.Enter("package.db").Path, Information.WorkPath.Enter("pri.key").Path);
                        General.ConfigManager.Configuration["VerifyBytes"] = Convert.ToBase64String(General.VerifyBytes);
                        General.ConfigManager.Save();

                        General.CoreTcpProcessor.StartListen();

                        General.IsMaintaining = false;
                        ConsoleAssistance.WriteLine("Switch to running mode successfully.", ConsoleColor.Yellow);

                    }
                    return false;
                },
                (ClientOption opt) => {
                    if (!CheckStatus(false)) return false;

                    ConsoleAssistance.WriteLine($"Current client: {General.ManualResetEventList.Count}", ConsoleColor.Yellow);
                    return false;
                },
                (ImportOption opt) => {
                    if (!CheckStatus(true)) return false;

                    ConsoleAssistance.WriteLine("import is a dangerous command. It will load all script and run it without any error judgement! It couldn't be stopped before all of commands has been executed!", ConsoleColor.Yellow);
                    var confirm = new Random().Next(100, 9999);
                    ConsoleAssistance.WriteLine($"Type this random number to confirm your operation: {confirm}", ConsoleColor.Yellow);
                    if (Console.ReadLine() == confirm.ToString()) {
                        if (System.IO.File.Exists(opt.FilePath)) ImportStack.AppendImportedCommands(opt.FilePath);
                        else ConsoleAssistance.WriteLine("Cannot find specific file", ConsoleColor.Red);
                    }
                    return false;
                },
                (LsOption opt) => {
                    if (!CheckStatus(true)) return false;

                    if (opt.Condition is null) PackageManager.Ls(General.GeneralDatabase, "");
                    else PackageManager.Ls(General.GeneralDatabase, opt.Condition);
                    return false;
                },
                (ShowOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.Show(General.GeneralDatabase, opt.FullPackageName);
                    return false;
                },
                (AddpkgOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.AddPackage(General.GeneralDatabase, opt);
                    return false;
                },
                (EditpkgOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.EditPackage(General.GeneralDatabase, opt);
                    return false;
                },
                (DelpkgOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.RemovePackage(General.GeneralDatabase, opt.Name);
                    return false;
                },
                (AddverOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.AddVersion(General.GeneralDatabase, opt);
                    return false;
                },
                (EditverOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.EditVersion(General.GeneralDatabase, opt);
                    return false;
                },
                (DelverOption opt) => {
                    if (!CheckStatus(true)) return false;
                    PackageManager.RemoveVersion(General.GeneralDatabase, opt.Name);
                    return false;
                },
                (HelpOption opt) => {
                    OutputHelp();
                    return false;
                },
                errs => { ConsoleAssistance.WriteLine("Unknow command. Use help to find the correct command", ConsoleColor.Red); return false; });

        }

        static bool CheckStatus(bool isCheckMaintain) {
            if (isCheckMaintain) {
                if (General.IsMaintaining) return true;
                else {
                    ConsoleAssistance.WriteLine("This command is illegal in current status.", ConsoleColor.Red);
                    return false;
                }
            } else {
                if (General.IsMaintaining) {
                    ConsoleAssistance.WriteLine("This command is illegal in current status.", ConsoleColor.Red);
                    return false;
                } else return true;
            }
        }

        static void OutputHelp() {
            Console.WriteLine("Usage: command [options]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("General command");
            Console.WriteLine("\texit [-f] - exit server. if you choose -f switch, it will kill the server without any hesitation (for emergency). you will lost each of your modifications");
            Console.WriteLine("\tconfig [setting-name] [new-value] - edit server config(config will be applied in the next startup)");
            Console.WriteLine("\tswitch - switch server mode between maintain and running");
            Console.WriteLine("\thelp - print this message");
            Console.WriteLine("");
            Console.WriteLine("Running mode command");
            Console.WriteLine("\tclient - list the number of client which is connecting this server");
            Console.WriteLine("");
            Console.WriteLine("Maintain mode command");
            Console.WriteLine("\timport file-path - import a script file to maintain server quickly");
            Console.WriteLine("\tls condition - list related packages");
            Console.WriteLine("\tshow full-name - list specific packages' detail");
            Console.WriteLine("\taddpkg name aka type desc - add a new package");
            Console.WriteLine("\teditpkg name aka type desc - edit a package (use ~ to keep the original value)");
            Console.WriteLine("\tdelpkg name - remove a package");
            Console.WriteLine("\taddver name parent additional-desc timestamp suit-os dependency reverse-conflict conflict require-decompress internal-script package-path - add a new version (use + to define current time)");
            Console.WriteLine("\teditver name parent additional-desc timestamp suit-os dependency reverse-conflict conflict require-decompress internal-script package-path - edit a version (use + to define current time. use ~ to keep the original value)");
            Console.WriteLine("\tdelver name - remove a version");
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

    [Verb("switch")]
    public class SwitchOption {

    }

    [Verb("client")]
    public class ClientOption {

    }

    [Verb("import")]
    public class ImportOption {
        [Value(0, Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("ls")]
    public class LsOption {
        [Value(0)]
        public string Condition { get; set; }
    }

    [Verb("show")]
    public class ShowOption {
        [Value(0, Required = true)]
        public string FullPackageName { get; set; }
    }

    [Verb("addpkg")]
    public class AddpkgOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
        [Value(1, Required = true)]
        public string Aka { get; set; }
        [Value(2, Required = true)]
        public string Type { get; set; }
        [Value(3, Required = true)]
        public string Desc { get; set; }
    }

    [Verb("editpkg")]
    public class EditpkgOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
        [Value(1, Required = true)]
        public string Aka { get; set; }
        [Value(2, Required = true)]
        public string Type { get; set; }
        [Value(3, Required = true)]
        public string Desc { get; set; }


    }

    [Verb("delpkg")]
    public class DelpkgOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
    }

    [Verb("addver")]
    public class AddverOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
        [Value(1, Required = true)]
        public string Parent { get; set; }
        [Value(2, Required = true)]
        public string AdditionalDesc { get; set; }
        [Value(3, Required = true)]
        public string Timestamp { get; set; }
        [Value(4, Required = true)]
        public string SuitOS { get; set; }
        [Value(5, Required = true)]
        public string Dependency { get; set; }
        [Value(6, Required = true)]
        public string ReverseConflict { get; set; }
        [Value(7, Required = true)]
        public string Conflict { get; set; }
        [Value(8, Required = true)]
        public string RequireDecompress { get; set; }
        [Value(9, Required = true)]
        public string InternalScript { get; set; }
        //hash will automatically detect
        //[Value(10, Required = true)]
        //public string HASH { get; set; }
        [Value(10, Required = true)]
        public string PackagePath { get; set; }


    }

    [Verb("editver")]
    public class EditverOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
        [Value(1, Required = true)]
        public string Parent { get; set; }
        [Value(2, Required = true)]
        public string AdditionalDesc { get; set; }
        [Value(3, Required = true)]
        public string Timestamp { get; set; }
        [Value(4, Required = true)]
        public string SuitOS { get; set; }
        [Value(5, Required = true)]
        public string Dependency { get; set; }
        [Value(6, Required = true)]
        public string ReverseConflict { get; set; }
        [Value(7, Required = true)]
        public string Conflict { get; set; }
        [Value(8, Required = true)]
        public string RequireDecompress { get; set; }
        [Value(9, Required = true)]
        public string InternalScript { get; set; }
        //hash will automatically detect
        //[Value(10, Required = true)]
        //public string HASH { get; set; }
        [Value(10, Required = true)]
        public string PackagePath { get; set; }

    }

    [Verb("delpkg")]
    public class DelverOption {
        [Value(0, Required = true)]
        public string Name { get; set; }
    }

    [Verb("help")]
    public class HelpOption {

    }

    #endregion

}
