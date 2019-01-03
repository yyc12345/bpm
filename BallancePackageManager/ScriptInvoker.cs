using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallancePackageManager.BPMCore;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using ShareLib;

namespace BallancePackageManager {
    public static class ScriptInvoker {

        public static (bool status, string desc) Core(string invokePath, InvokeMethod method, string parameter) {
            var gamePath = Config.Read()["GamePath"];
            var folder = new DirectoryInfo(invokePath);

            var folder_build = new FilePathBuilder(invokePath, Information.OS);
            folder_build.Enter("setup.cs");
            var script_file = folder_build.Path;
            folder_build.Backtracking();
            folder_build.Enter("setup.dll");
            var dll_file = folder_build.Path;

            if (!File.Exists(script_file)) return (false, I18N.Core("ScriptInvoker_NoScriptFile"));

            if (!File.Exists(dll_file)) {
                //try compile
                var test = CompileScript(script_file, dll_file);
                if (!test.status) return (false, I18N.Core("ScriptInvoker_CompileError", test.desc)); //todo: translation
            }

            return RealInvoker(dll_file, method, gamePath, invokePath, parameter);
        }

        static (bool status, string desc) CompileScript(string origin, string target) {
            try {
                //read code
                var fs = new StreamReader(origin, Encoding.UTF8);
                var user_code = fs.ReadToEnd();
                fs.Close();
                fs.Dispose();

                //read code body
                var fs2 = new StreamReader(Information.WorkPath.Enter("ScriptBody.cs").Path, Encoding.UTF8);
                var code = fs2.ReadToEnd().Replace("{PersonalCode}", user_code);
                fs2.Close();
                fs2.Dispose();

                //compile
                var compiler = CSharpCompilation.Create("bpm_Plugin")
               .WithOptions(new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary))
               .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
               .AddReferences(MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.IO")).Location))
               //.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text")).Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Collections")).Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.ValueTuple")).Location))
               .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime.Extensions")).Location))
               .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
                
                var res = compiler.Emit(target);

                if (!res.Success) {
                    string r = "";
                    foreach(var item in res.Diagnostics) {
                        r += item.ToString() + "\n";
                    }
                    return (false, r);
                }
            } catch (Exception e) {
                return (false, e.Message);
            }

            return (true, "");
        }

        static (bool status, string desc) RealInvoker(string file, InvokeMethod method, string game_path, string current_folder, string parameter) {
            try {
                var engine = Assembly.Load(File.ReadAllBytes(file));
                switch (method) {
                    case InvokeMethod.Install:
                        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Install").Invoke(null, new object[] { game_path, current_folder }));
                    case InvokeMethod.Deploy:
                        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Deploy").Invoke(null, new object[] { game_path, current_folder, parameter }));
                    case InvokeMethod.Remove:
                        return ((bool status, string desc))(engine.GetType("Plugin").GetMethod("Remove").Invoke(null, new object[] { game_path, current_folder }));
                    case InvokeMethod.Help:
                        return (true, (string)(engine.GetType("Plugin").GetMethod("Help").Invoke(null, null)));
                    default:
                        return (false, I18N.Core("ScriptInvoker_NoMethod"));
                }
            } catch (Exception) {
                return (false, I18N.Core("ScriptInvoker_InvokeError"));
            }

        }

        public enum InvokeMethod {
            Install,
            Deploy,
            Remove,
            Help
        }
    }
}
