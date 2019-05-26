using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLib;
using BPMServer.Command;

namespace BPMServer {
    class Program {

        static void Main(string[] args) {

            if (!Directory.Exists(Information.WorkPath.Enter("package").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("package").Path);

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
                    if (CommandProcessor.Process(command)) break;
                    
                    //re-output words
                    General.GeneralOutput.Release();
                }
            }
            
            ConsoleAssistance.WriteLine("Server exit!", ConsoleColor.Yellow);
            //Environment.Exit(0);
        }
    }
}
