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
            var verifyCache= Information.WorkPath.Enter("verify.dat").Path

            //detect database
            if (!File.Exists(databasePath)) {
                ConsoleAssistance.WriteLine("No existing database file. A empty database file will be created.", ConsoleColor.Yellow);
                General.GeneralDatabase.Open();
                General.GeneralDatabase.Close();
            }

            //detect encrypt file
            if(!File.Exists(rsaPublic) || !File.Exists(rsaPrivate)) {
                //ensure there are no file.
                ConsoleAssistance.WriteLine("No existing RSA key. A new RSA key will be created.", ConsoleColor.Yellow);
                File.Delete(rsaPublic);
                File.Delete(rsaPrivate);

                SignVerifyHelper.GenerateKey(rsaPublic, rsaPrivate);
            }

            //detect database verify cache
            if (!File.Exists(verifyCache)) {
                ConsoleAssistance.WriteLine("No verify cache file. A new cache file will be created.", ConsoleColor.Yellow);

                var str = Convert.ToBase64String(SignVerifyHelper.SignData(databasePath, rsaPublic));
                File.WriteAllText(verifyCache, str);
            }

            #endregion

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
