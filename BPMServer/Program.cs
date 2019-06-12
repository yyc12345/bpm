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
            if (!Directory.Exists(Information.WorkPath.Enter("logs").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("logs").Path);

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
            General.ConfigManager = new ShareLib.Config("config.cfg", new Dictionary<string, string>() {
                    {"IPv4Port" , "3850" },
                    {"IPv6Port" , "3851" },
                    {"VerifyBytes", "" },
                    {"EnableRecordFile", "True" }
                });
            General.CoreFileReader = new FileReaderManager();
            General.RecordFileManager = new RecordFile(bool.Parse(General.ConfigManager.Configuration["EnableRecordFile"]));
            General.CoreTcpProcessor = new TcpProcessor(int.Parse(General.ConfigManager.Configuration["IPv4Port"]), int.Parse(General.ConfigManager.Configuration["IPv6Port"]));

            //check parameter
            if (args.Length != 0 && args[0] == "maintain") {
                General.GeneralDatabase.Open();
                General.IsMaintaining = true;
                ConsoleAssistance.WriteLine("Start with maintain mode", ConsoleColor.Yellow);
            } else {
                //force update verify code
                ConsoleAssistance.WriteLine("Updating verify code....", ConsoleColor.White);
                General.VerifyBytes = SignVerifyHelper.SignData(databasePath, rsaPrivate);
                General.ConfigManager.Configuration["VerifyBytes"] = Convert.ToBase64String(General.VerifyBytes);
                General.ConfigManager.Save();

                General.CoreTcpProcessor.StartListen();
                General.IsMaintaining = false;
                ConsoleAssistance.WriteLine("Start with running mode", ConsoleColor.Yellow);
            }

            //================================================read circle
            string command = "";
            while (true) {
                ConsoleAssistance.Write($"BPMServer ({(General.IsMaintaining ? "Maintaining" : "Running")})> ", ConsoleColor.Green);

                command = ImportStack.ReadLine();
                if (CommandProcessor.Process(command)) break;

            }

            ConsoleAssistance.WriteLine("Server exit!", ConsoleColor.Yellow);
            //Environment.Exit(0);
        }
    }
}
