using CommandLine;
using ShareLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using System.Reflection;

namespace BPMScriptDebugger {

    public static class CommandProcessor {

        static Parser parser = new Parser(ps => {
            ps.AutoHelp = false;
            ps.AutoVersion = false;
            ps.HelpWriter = null;
            ps.EnableDashDash = true;
        });

        public static bool Process(string command) {
            return parser.ParseArguments<ExitOption, ConfigOption, LoadOption, BuildOption, ExecOption, HelpOption>(
                CommandSplitter.Split(command))
                .MapResult(
                (ExitOption opt) => {
                    return true;
                },
                (ConfigOption opt) => {
                    if (opt.Key is null)
                        foreach (var item in General.ConfigManager.Configuration.Keys) {
                            Console.Write($"{item}: ");
                            Console.Write($"{General.ConfigManager.Configuration[item]}\n");
                        } else {
                        if (opt.NewValue is null) {
                            if (General.ConfigManager.Configuration.Keys.Contains(opt.Key))
                                Console.WriteLine(General.ConfigManager.Configuration[opt.Key]);
                        } else {
                            if (General.ConfigManager.Configuration.Keys.Contains(opt.Key)) {
                                General.ConfigManager.Configuration[opt.Key] = opt.NewValue;
                                General.ConfigManager.Save();
                                Console.WriteLine("New value has been applied");
                            }
                        }

                    }
                    return false;
                },
                (LoadOption opt) => {
                    if (General.CurrentAppStatus == AppStatus.Build) {
                        //load
                        try {
                            General.LoadedModule = Assembly.Load(File.ReadAllBytes("Test.dll"));
                            General.ScriptSettings.CleanSettings();
                            General.CurrentAppStatus = AppStatus.Loaded;
                        } catch (Exception e) {
                            ConsoleAssistance.WriteLine("Loading assembly error" + Environment.NewLine + e.Message, ConsoleColor.Yellow);
                        }

                    } else {
                        //unload
                        General.CurrentAppStatus = AppStatus.Build;
                    }

                    return false;
                },
                (BuildOption opt) => {
                    if (!CheckStatus(AppStatus.Build)) return false;

                    File.Delete("Test.dll");
                    try {
                        //read code
                        var fs = new StreamReader("setup.cs", Encoding.UTF8);
                        var code = General.CodeTemplate.Replace("{PersonalCode}", fs.ReadToEnd());
                        fs.Close();
                        fs.Dispose();

                        //compile
                        var compiler = CSharpCompilation.Create("bpm_Plugin")
                       .WithOptions(new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary))
                       .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                       .AddReferences(MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location))
                       .AddReferences(MetadataReference.CreateFromFile(typeof(File).GetTypeInfo().Assembly.Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.IO")).Location))
                       //.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text")).Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Collections")).Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.ValueTuple")).Location))
                       .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime.Extensions")).Location))
                       //.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.IO.FileSystem")).Location))
                       .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

                        var res = compiler.Emit("Test.dll");
                        string r = "";
                        foreach (var item in res.Diagnostics) {
                            r += item.ToString() + Environment.NewLine;
                        }

                        if (!res.Success)
                            ConsoleAssistance.WriteLine("Compile error:" + Environment.NewLine + r, ConsoleColor.Yellow);
                        else {
                            ConsoleAssistance.WriteLine("Compile OK:" + Environment.NewLine + r);
                        }
                    } catch (Exception e) {
                        ConsoleAssistance.WriteLine("Compile runtime error:" + Environment.NewLine + e.Message, ConsoleColor.Yellow);
                    }

                    return false;
                },
                (ExecOption opt) => {
                    if (!CheckStatus(AppStatus.Loaded)) return false;
                    if (!CheckSettings()) return false;

                    switch (opt.MethodName) {
                        case "Install":
                            OutputRunningResult(((bool status, string desc))(General.LoadedModule.GetType("Plugin").GetMethod("Install").Invoke(null,
                                new object[] { General.ConfigManager.Configuration["GamePath"],
                                    Information.WorkPath.Path,
                                    (Func<Dictionary<string, string>>)(General.ScriptSettings.GetSettings),
                                    (Action<Dictionary<string, string>>)(General.ScriptSettings.SetSettings),
                                    General.ConfigManager.Configuration["I18N"]})));
                            OutputSettingsValue(General.ScriptSettings);
                            break;
                        case "Deploy":
                            OutputRunningResult(((bool status, string desc))(General.LoadedModule.GetType("Plugin").GetMethod("Deploy").Invoke(null,
                                new object[] { General.ConfigManager.Configuration["GamePath"],
                                    Information.WorkPath.Path,
                                    (Func<Dictionary<string, string>>)(General.ScriptSettings.GetSettings),
                                    (Action<Dictionary<string, string>>)(General.ScriptSettings.SetSettings),
                                    General.ConfigManager.Configuration["I18N"],
                                    opt.Parameters.ToList() })));
                            OutputSettingsValue(General.ScriptSettings);
                            break;
                        case "Remove":
                            OutputRunningResult(((bool status, string desc))(General.LoadedModule.GetType("Plugin").GetMethod("Remove").Invoke(null,
                                new object[] { General.ConfigManager.Configuration["GamePath"],
                                    Information.WorkPath.Path,
                                    (Func<Dictionary<string, string>>)(General.ScriptSettings.GetSettings),
                                    (Action<Dictionary<string, string>>)(General.ScriptSettings.SetSettings),
                                    General.ConfigManager.Configuration["I18N"] })));
                            OutputSettingsValue(General.ScriptSettings);
                            break;
                        case "Help":
                            OutputRunningResult((true, (string)(General.LoadedModule.GetType("Plugin").GetMethod("Help").Invoke(null,
                                new object[] { General.ConfigManager.Configuration["I18N"] }))));
                            OutputSettingsValue(General.ScriptSettings);
                            break;
                        default:
                            Console.WriteLine("No matched command");
                            break;
                    }

                    return false;
                },
                (HelpOption opt) => {
                    OutputHelp();
                    return false;
                },
                errs => { ConsoleAssistance.WriteLine("Unknow command. Use help to find the correct command", ConsoleColor.Red); return false; });

        }

        static bool CheckStatus(AppStatus check) {
            if (check == General.CurrentAppStatus) return true;
            else {
                ConsoleAssistance.WriteLine("This command is illegal in current status.", ConsoleColor.Red);
                return false;
            }

        }
        static bool CheckSettings() {
            if (General.ConfigManager.Configuration["GamePath"] == "" ||
                General.ConfigManager.Configuration["I18N"] == "") {
                ConsoleAssistance.WriteLine("Your config is not correct. Please use config to set correct config.", ConsoleColor.Red);
                return false;
            }
            return true;
        }

        static void OutputRunningResult((bool status, string desc) data) {
            ConsoleAssistance.Write("Status:", ConsoleColor.Yellow);
            Console.WriteLine(data.status);
            ConsoleAssistance.WriteLine("Description:", ConsoleColor.Yellow);
            Console.WriteLine(data.desc);
            Console.WriteLine();
        }

        static void OutputSettingsValue(SettingsManager sm) {
            var cache = sm.GetSettings();
            ConsoleAssistance.WriteLine("Settings value:", ConsoleColor.Yellow);
            foreach (var item in cache.Keys) {
                ConsoleAssistance.Write($"{item}: ", ConsoleColor.Yellow);
                Console.WriteLine(cache[item]);
            }
        }

        static void OutputHelp() {
            Console.WriteLine("Usage: command [options]");
            Console.WriteLine("");
            Console.WriteLine("bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages.");
            Console.WriteLine("");
            Console.WriteLine("Most used commands:");
            Console.WriteLine("General command");
            Console.WriteLine("\texit - exit debugger");
            Console.WriteLine("\tconfig [setting-name] [new-value] - edit debugger config");
            Console.WriteLine("\tload - load/unload built script module (a successful loading will wipe out all of script settings)");
            Console.WriteLine("\texec [method-name] [parameter] - exec specific method when you load script module successfully");
            Console.WriteLine("\thelp - print this message");
            Console.WriteLine("");
            Console.WriteLine("Glory to BKT.");
        }
    }

    public enum AppStatus {
        Build,
        Loaded
    }

    #region command structure

    [Verb("exit")]
    public class ExitOption {
    }

    [Verb("config")]
    public class ConfigOption {
        [Value(0)]
        public string Key { get; set; }
        [Value(1)]
        public string NewValue { get; set; }
    }

    [Verb("build")]
    public class BuildOption {

    }

    [Verb("load")]
    public class LoadOption {

    }

    [Verb("exec")]
    public class ExecOption {
        [Value(0, Required = true)]
        public string MethodName { get; set; }
        [Value(1)]
        public IEnumerable<string> Parameters { get; set; }
    }

    [Verb("help")]
    public class HelpOption {

    }

    #endregion

}

