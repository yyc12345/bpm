using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPMServer {
    class Program {

        static void Main(string[] args) {

            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"package"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"package");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"dependency"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"dependency");

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
                        Environment.Exit(1);
                    }
                    if (command == "exit") {
                        General.CoreTcpProcessor.StopListen();
                        Console.WriteLine("Waiting the release of resources...");
                        if (General.ManualResetEventList.Count != 0)
                            WaitHandle.WaitAll(General.ManualResetEventList.ToArray());
                        Environment.Exit(0);
                    } 

                    Command.CommandExecute(command);

                    General.GeneralOutput.Release();
                }
            }


        }
    }
}
