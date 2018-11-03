using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallancePackageManager.BPMCore;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System.IO;
using ShareLib;

namespace BallancePackageManager {
    public static class ScriptInvoker {

        public static bool Core(string invokePath, InvokeMethod method, string parameter) {
            var gamePath = Config.Read()["GamePath"];
            var folder = new DirectoryInfo(invokePath);

            var file = folder.GetFiles("setup-*.*");
            if (!file.Any()) return false;
            for(int i = 1; i <= file.Count(); i++) {
                var cacheFile = folder.GetFiles($"setup-{i}.*")[0];
                var cache = PackageAssistance.GetScriptInfo(cacheFile.Name);
                switch (cache.suffix) {
                    case ".py":
                        if (!PythonInvoker(cacheFile.FullName, method, gamePath, invokePath, parameter))
                            return false;
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        static bool PythonInvoker(string file, InvokeMethod method, string game_path, string current_folder, string parameter) {
            ScriptEngine pyEngine = Python.CreateEngine();
            dynamic dd = pyEngine.ExecuteFile(file);
            switch (method) {
                case InvokeMethod.Install:
                    return (bool)dd.install(game_path, current_folder);
                case InvokeMethod.Check:
                    return (bool)dd.check(game_path, current_folder);
                case InvokeMethod.Deploy:
                    return (bool)dd.deploy(game_path, current_folder, parameter);
                case InvokeMethod.Remove:
                    return (bool)dd.remove(game_path);
                default:
                    return false;
            }
        }

        public enum InvokeMethod {
            Install,
            Check,
            Deploy,
            Remove
        }
    }
}
