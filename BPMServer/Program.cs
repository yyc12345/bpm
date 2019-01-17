using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLib;

namespace BPMServer {
    class Program {

        static void Main(string[] args) {

            if (!Directory.Exists(Information.WorkPath.Enter("package").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("package").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("dependency").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("dependency").Path);

            //detect database
            if (!File.Exists(Information.WorkPath.Enter("package.db").Path)) {
                General.GeneralDatabase.Open();
                General.GeneralDatabase.Close();
            }

            var config = Config.Read();
            General.CoreFileReader = new FileReaderManager();
            General.CoreTcpProcessor = new TcpProcessor(int.Parse(config["IPv4Port"]), int.Parse(config["IPv6Port"]));
            General.CoreTcpProcessor.StartListen();

            //read circle
            string command = "";
            while (true) {
                if (Console.ReadKey(true).Key == ConsoleKey.Tab) {
                    General.GeneralOutput.Stop();
                    ConsoleAssistance.Write($"BPMServer ({(General.IsMaintaining ? "Maintaining" : "Running")})> ", ConsoleColor.Green);

                    command = Console.ReadLine();
                    if (command.Length == 0) {
                        ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                        OutputHelp();

                        //re-output words
                        General.GeneralOutput.Release();
                        continue;
                    }

                    var param = new List<string>(CommandSplitter.SplitCommand(command));
                    var head = param[0];
                    param.RemoveAt(0);

                    switch (head) {
                        //==================================================general operation
                        case "crash":
                            goto app_end;
                        case "exit":
                            if (!General.IsMaintaining) {
                                General.CoreTcpProcessor.StopListen();
                                Console.WriteLine("Waiting the release of resources...");
                                if (General.ManualResetEventList.Count != 0)
                                    WaitHandle.WaitAll(General.ManualResetEventList.ToArray());
                            } else General.GeneralDatabase.Close();
                            goto app_end;
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
                        //===================================================switch command
                        case "mode":
                            if (param.Count != 1) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            else {
                                if (param[0] == "maintain") {
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
                                } else if (param[0] == "running") {
                                    if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now.", ConsoleColor.Red);
                                    else {
                                        General.GeneralDatabase.Close();
                                        General.CoreTcpProcessor.StartListen();

                                        General.IsMaintaining = false;
                                        ConsoleAssistance.WriteLine("Switch to running mode successfully.", ConsoleColor.Yellow);
                                    }
                                } else ConsoleAssistance.WriteLine("Invalid parameter", ConsoleColor.Red);
                            }
                            break;
                        //====================================================running command
                        case "client":
                            if (!General.IsMaintaining) ConsoleAssistance.WriteLine($"Current client: {General.ManualResetEventList.Count}", ConsoleColor.Yellow);
                            else ConsoleAssistance.WriteLine("Server is being maintained. There are no any client.", ConsoleColor.Red);
                            break;
                        //======================================================maintain command
                        case "addpkg":
                            if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now. You couldn't invoke any maintain command.", ConsoleColor.Red);
                            else {
                                if (param.Count != 5) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                                else PackageManager.AddPackage(General.GeneralDatabase, param[0], param[1], param[2], param[3], param[4], Information.WorkPath.Enter("new_package.zip").Path, Information.WorkPath.Enter("new_package.json").Path);
                            }
                            break;
                        case "addver":
                            if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now. You couldn't invoke any maintain command.", ConsoleColor.Red);
                            else {
                                if (param.Count != 2) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                                else PackageManager.AddVersion(General.GeneralDatabase, param[0], param[1], Information.WorkPath.Enter("new_package.zip").Path, Information.WorkPath.Enter("new_package.json").Path);
                            }
                            break;
                        case "delpkg":
                            if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now. You couldn't invoke any maintain command.", ConsoleColor.Red);
                            else {
                                if (param.Count != 1) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                                else PackageManager.RemovePackage(General.GeneralDatabase, param[0]);
                            }
                            break;
                        case "delver":
                            if (!General.IsMaintaining) ConsoleAssistance.WriteLine("You are in running mode now. You couldn't invoke any maintain command.", ConsoleColor.Red);
                            else {
                                if (param.Count != 2) ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                                else PackageManager.RemoveVersion(General.GeneralDatabase, param[0], param[1]);
                            }
                            break;
                        case "help":
                            OutputHelp();
                            break;
                        default:
                            ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                            OutputHelp();
                            break;
                    }

                    //re-output words
                    General.GeneralOutput.Release();
                }
            }

            app_end:
            ConsoleAssistance.WriteLine("Server exit!", ConsoleColor.Yellow);
            //Environment.Exit(0);
        }

        static void OutputHelp() {
            Console.WriteLine("Usage: command [options]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("General command");
            Console.WriteLine("\texit - exit server");
            Console.WriteLine("\tcrash - kill server without any hesitation (for emergency). you will lost each of your modifications");
            Console.WriteLine("\tconfig - edit server config(config will be applied in the next startup)");
            Console.WriteLine("\tmode [maintain | running] - switch server mode");
            Console.WriteLine("");
            Console.WriteLine("Running mode command");
            Console.WriteLine("\tclient - list the number of client which is connecting this server");
            Console.WriteLine("");
            Console.WriteLine("Maintain mode command");
            Console.WriteLine("\taddpkg name aka type version desc - add a package");
            Console.WriteLine("\taddver name version - add a version of the specific package");
            Console.WriteLine("\tdelpkg name - delete a package");
            Console.WriteLine("\tdelver name version - delete a version of the specific package");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }
}
