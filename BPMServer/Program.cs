using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"package"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"package");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"dependency"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"dependency");

            //detect database
            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                SQLiteConnection.CreateFile(ConsoleAssistance.WorkPath + "package.db");
                var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
                packageDbConn.Open();
                var cachecursor = new SQLiteCommand($"create table package (name TEXT primary key not null, aka TEXT, type INTEGER not null, version TEXT not null, desc TEXT)", packageDbConn);
                cachecursor.ExecuteNonQuery();
                packageDbConn.Close();
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
                        try {
                            td.Abort();
                            foreach (var item in Process.GetProcessesByName("BPMSDaemon")) {
                                item.Kill();
                            }
                        } catch (Exception) {
                            //pass
                        }
                        Environment.Exit(1);
                    }
                    if (command == "exit") {
                        General.CoreTcpProcessor.StopListen();
                        Console.WriteLine("Waiting the release of resources...");
                        if (General.ManualResetEventList.Count != 0)
                            WaitHandle.WaitAll(General.ManualResetEventList.ToArray());

                        try {
                            td.Abort();
                            foreach (var item in Process.GetProcessesByName("BPMSDaemon")) {
                                item.Kill();
                            }
                        } catch (Exception) {
                            //pass
                        }
                        Environment.Exit(0);
                    }

                    Command.CommandExecute(command);

                    General.GeneralOutput.Release();
                }
            }


        }
    }
}
