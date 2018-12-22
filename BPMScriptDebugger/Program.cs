using ShareLib;
using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using System.Reflection;

namespace BPMScriptDebugger {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("BPM Script Debugger");

            //read template
            var fs2 = new StreamReader("ScriptBody.cs", Encoding.UTF8);
            var originCode = fs2.ReadToEnd();
            fs2.Close();
            fs2.Dispose();

            var game_path = "";
            if (File.Exists("settings.config")) {
                var fs = new StreamReader("settings.config", Encoding.UTF8);
                game_path = fs.ReadLine();
                fs2.Close();
                fs2.Dispose();
            }
            if (game_path == "") {
                Console.WriteLine("You should input game_path now");
                game_path = Console.ReadLine();

                var wfs = new StreamWriter("settings.config", false, Encoding.UTF8);
                wfs.WriteLine(game_path);
                wfs.Close();
                wfs.Dispose();
            }

            int round = 0;

            while (true) {
                round++;
                ConsoleAssistance.WriteLine($"Compile Round {round}", ConsoleColor.Green);
                File.Delete("Test.dll");

                //input

                Console.WriteLine("If you are ready, press any key to compile your code. (Press tab to quit)");
                var cache = Console.ReadKey(true);
                if (cache.Key == ConsoleKey.Tab) break;

                //compile

                try {
                    //read code
                    var fs = new StreamReader("Test.cs", Encoding.UTF8);
                    var code = originCode.Replace("{PersonalCode}", fs.ReadToEnd());
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

                    if (!res.Success) {
                        string r = "";
                        foreach (var item in res.Diagnostics) {
                            r += item.ToString() + Environment.NewLine;
                        }
                        ConsoleAssistance.WriteLine("Compile error:" + Environment.NewLine + r, ConsoleColor.Yellow);
                        continue;
                    } else {
                        string r = "";
                        foreach (var item in res.Diagnostics) {
                            r += item.ToString() + Environment.NewLine;
                        }
                        ConsoleAssistance.WriteLine("Compile OK:" + Environment.NewLine + r);
                    }
                } catch (Exception e) {
                    ConsoleAssistance.WriteLine("Compile runtime error:" + Environment.NewLine + e.Message, ConsoleColor.Yellow);
                    continue;
                }


                //read assembly
                Assembly engine;
                try {
                    engine = Assembly.Load(File.ReadAllBytes("Test.dll"));
                    //switch (method) {
                    //    case InvokeMethod.Install:
                    //        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Install").Invoke(null, new object[] { game_path, current_folder }));
                    //    case InvokeMethod.Deploy:
                    //        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Deploy").Invoke(null, new object[] { game_path, current_folder, parameter }));
                    //    case InvokeMethod.Remove:
                    //        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Remove").Invoke(null, new object[] { game_path, current_folder }));
                    //    case InvokeMethod.Help:
                    //        return (true, (string)(engine.GetType("Plugin").GetMethod("Help").Invoke(null, null)));
                    //    default:
                    //        return (false, I18N.Core("ScriptInvoker_NoMethod"));
                    //}
                } catch (Exception e) {
                    ConsoleAssistance.WriteLine("Loading assembly error" + Environment.NewLine + e.Message, ConsoleColor.Yellow);
                    continue;
                }


                //invoke
                var command = "";
                var current_folder = Information.WorkPath.Path;
                Console.WriteLine("Compile and read assembly OK. Input function name to invoke correspond function. Input empty words to start next compile round.");
                while (true) {
                 
                    command = Console.ReadLine();
                    if (command == "") break;

                    var real_command = CommandSplitter.SplitCommand(command);

                    switch (real_command[0]) {
                        case "Install":
                            GeneralOutput(((bool status, string desc))(engine.GetType("Plugin").GetMethod("Install").Invoke(null, new object[] { game_path, current_folder })));
                            break;
                        case "Deploy":
                            GeneralOutput(((bool status, string desc))(engine.GetType("Plugin").GetMethod("Deploy").Invoke(null, new object[] { game_path, current_folder, real_command[1] })));
                            break;
                        case "Remove":
                            GeneralOutput(((bool status, string desc))(engine.GetType("Plugin").GetMethod("Remove").Invoke(null, new object[] { game_path, current_folder })));
                            break;
                        case "Help":
                            GeneralOutput((true, (string)(engine.GetType("Plugin").GetMethod("Help").Invoke(null, null))));
                            break;
                        default:
                            Console.WriteLine("No matched command");
                            break;
                    }
                }

            }

        }

        static void GeneralOutput((bool status,string desc) data) {
            ConsoleAssistance.Write("Status:", ConsoleColor.Yellow);
            Console.WriteLine(data.status);
            ConsoleAssistance.WriteLine("Description:", ConsoleColor.Yellow);
            Console.WriteLine(data.desc);
            Console.WriteLine();
        }

    }
}
