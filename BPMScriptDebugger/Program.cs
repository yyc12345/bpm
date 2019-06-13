using ShareLib;
using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using System.Reflection;
using System.Collections.Generic;

namespace BPMScriptDebugger {
    class Program {
        static void Main(string[] args) {
            ConsoleAssistance.WriteLine("BPM Script Debugger", ConsoleColor.Yellow);

            //read template
            var fs2 = new StreamReader("ScriptBody.cs", Encoding.UTF8);
            General.CodeTemplate = fs2.ReadToEnd();
            fs2.Close();
            fs2.Dispose();

            //read config
            General.ConfigManager = new Config("settings.config", new Dictionary<string, string>() {
                {"GamePath", ""},
                {"I18N", "en-us"}
            });

            //setup script settings storage
            General.ScriptSettings = new SettingsManager();

            //================================================read circle
            string command = "";
            while (true) {
                ConsoleAssistance.Write($"BPMScriptDebugger ({General.CurrentAppStatus.ToString()})> ", ConsoleColor.Green);

                command = Console.ReadLine();
                if (CommandProcessor.Process(command)) break;

            }

            ConsoleAssistance.WriteLine("Debugger exit.", ConsoleColor.Yellow);
            
        }

        

    }
}
