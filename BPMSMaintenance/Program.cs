using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BPMSMaintenance {
    class Program {
        static void Main(string[] args) {

            //detect folder
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"package"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"package");
            if (!Directory.Exists(ConsoleAssistance.WorkPath + @"dependency"))
                Directory.CreateDirectory(ConsoleAssistance.WorkPath + @"dependency");

            //detect database
            bool isNew = false;
            if (!File.Exists(ConsoleAssistance.WorkPath + "package.db")) {
                SQLiteConnection.CreateFile(ConsoleAssistance.WorkPath + "package.db");
                isNew = true;
            }
                
            var packageDbConn = new SQLiteConnection($"Data Source = {ConsoleAssistance.WorkPath}package.db; Version = 3;");
            packageDbConn.Open();

            if (isNew) {
                var cachecursor = new SQLiteCommand($"create table package (name TEXT primary key not null, aka TEXT, type INTEGER not null, version TEXT not null, desc TEXT)", packageDbConn);
                cachecursor.ExecuteNonQuery();
            }

            ConsoleAssistance.WriteLine("Welcome to BPMS Maintenance app.", ConsoleColor.Yellow);

            string command = "";
            while (true) {
                ConsoleAssistance.Write("BPMSMaintenance> ", ConsoleColor.Green);

                command = Console.ReadLine();
                var param = new List<string>(CommandSplitter.SplitCommand(command));
                var head = param[0];
                param.RemoveAt(0);

                switch (head) {
                    case "exit":
                        packageDbConn.Close();
                        Environment.Exit(0);
                        break;
                    case "addpkg":
                        if (param.Count != 5) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.AddPackage(packageDbConn, param[0], param[1], param[2], param[3], param[4], ConsoleAssistance.WorkPath + $"new_package.zip", ConsoleAssistance.WorkPath + $"new_package.json");
                        break;
                    case "addver":
                        if (param.Count != 2) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.AddVersion(packageDbConn, param[0], param[1], ConsoleAssistance.WorkPath + $"new_package.zip", ConsoleAssistance.WorkPath + $"new_package.json");
                        break;
                    case "delpkg":
                        if (param.Count != 1) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.RemovePackage(packageDbConn, param[0]);
                        break;
                    case "delver":
                        if (param.Count != 2) {
                            ConsoleAssistance.WriteLine("Invalid parameter count", ConsoleColor.Red);
                            break;
                        }
                        PackageManager.RemoveVersion(packageDbConn, param[0], param[1]);
                        break;
                    case "help":
                        OutputHelp();
                        break;
                    default:
                        ConsoleAssistance.WriteLine("Invalid command", ConsoleColor.Red);
                        break;
                }
            }

        }

        private static void OutputHelp() {
            Console.WriteLine("Usage: bpm option [command]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("  exit - exit maintenance app");
            //Console.WriteLine("  crash - kill server without any hesitation (for emergency)");
            //Console.WriteLine("  config - edit server config(config will be applied in the next startup)");
            //Console.WriteLine("  client - list the number of client which is connecting this server");
            Console.WriteLine("  addpkg - add a package");
            Console.WriteLine("  addver - add a version of the specific package");
            Console.WriteLine("  delpkg - delete a package");
            Console.WriteLine("  delver - delete a version of the specific package");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }
}
