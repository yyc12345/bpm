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

            /*
            //detect daemon
            var td = new System.Threading.Thread(() => {
                while (true) {

                    var cache = Process.GetProcessesByName("BPMSDaemon");
                    if (!cache.Any()) {
                        Process.Start(ConsoleAssistance.WorkPath + "BPMSDaemon.exe");
                        Console.WriteLine("Start daemon...");
                    }

                    System.Threading.Thread.Sleep(30 * 1000);
                }
            });
            td.IsBackground = true;
            td.Start();
            */

            if (!Directory.Exists(Information.WorkPath.Enter("package").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("package").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("dependency").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("dependency").Path);

            //detect database
            if (!File.Exists(Information.WorkPath.Enter("package.db").Path)) {
                var createDb = new Database();
                createDb.Open();
                createDb.Close();
            }

            var config = Config.Read();
            General.CoreFileReader = new FileReaderManager();
            General.CoreTcpProcessor = new TcpProcessor(int.Parse(config["IPv4Port"]), int.Parse(config["IPv6Port"]));
            General.CoreTcpProcessor.StartListen();

            //read circle
            string command = "";
            while (true) {
                if (Console.ReadKey(true).Key == ConsoleKey.Tab) {
                    ConsoleAssistance.Write("BPMServer> ", ConsoleColor.Green);

                    General.GeneralOutput.Stop();
                    command = Console.ReadLine();

                    if (command == "crash") {
                        /*
                        try {
                            td.Abort();
                            foreach (var item in Process.GetProcessesByName("BPMSDaemon")) {
                                item.Kill();
                            }
                        } catch (Exception) {
                            //pass
                        }
                        */
                        Environment.Exit(1);
                    }
                    if (command == "exit") {
                        General.CoreTcpProcessor.StopListen();
                        Console.WriteLine("Waiting the release of resources...");
                        if (General.ManualResetEventList.Count != 0)
                            WaitHandle.WaitAll(General.ManualResetEventList.ToArray());
                        /*
                        try {
                            td.Abort();
                            foreach (var item in Process.GetProcessesByName("BPMSDaemon")) {
                                item.Kill();
                            }
                        } catch (Exception) {
                            //pass
                        }
                        */
                        Environment.Exit(0);
                    }

                    Command.CommandExecute(command);

                    General.GeneralOutput.Release();
                }
            }


        }
    }
}
