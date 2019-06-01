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

            #region comfirm necessary file
            ConsoleAssistance.WriteLine("Server is checking necessary files...", ConsoleColor.Yellow);

            //package storage
            if (!Directory.Exists(Information.WorkPath.Enter("package").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("package").Path);

            var databasePath = Information.WorkPath.Enter("package.db").Path;
            var rsaPublic = Information.WorkPath.Enter("pub.key").Path;
            var rsaPrivate = Information.WorkPath.Enter("pri.key").Path;

            //detect database
            if (!File.Exists(databasePath)) {
                ConsoleAssistance.WriteLine("No existing database file. A empty database file will be created.", ConsoleColor.Yellow);
                General.GeneralDatabase.Open();
                General.GeneralDatabase.Close();
            }

            //detect encrypt file
            if (!File.Exists(rsaPublic) || !File.Exists(rsaPrivate)) {
                //ensure there are no file.
                ConsoleAssistance.WriteLine("No existing RSA key. A new RSA key will be created.", ConsoleColor.Yellow);
                File.Delete(rsaPublic);
                File.Delete(rsaPrivate);

                SignVerifyHelper.GenerateKey(rsaPublic, rsaPrivate);
            }

            #endregion

            //init config and file pool
            var config = Config.Read();
            General.CoreFileReader = new FileReaderManager();
            General.CoreTcpProcessor = new TcpProcessor(int.Parse(config["IPv4Port"]), int.Parse(config["IPv6Port"]));

            //check parameter
            if (args.Length != 0 && args[0] == "maintain") {
                General.GeneralDatabase.Open();
                General.IsMaintaining = true;
                ConsoleAssistance.WriteLine("Start with maintain mode", ConsoleColor.Yellow);
            } else {
                //force update verify code
                ConsoleAssistance.WriteLine("Updating verify code....", ConsoleColor.White);
                General.VerifyBytes = SignVerifyHelper.SignData(databasePath, rsaPrivate);
                config["VerifyBytes"] = Convert.ToBase64String(General.VerifyBytes);
                Config.Save(config);

                General.CoreTcpProcessor.StartListen();
                General.IsMaintaining = false;
                ConsoleAssistance.WriteLine("Start with running mode", ConsoleColor.Yellow);
            }

            //================================================read circle
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
