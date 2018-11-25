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

        public static (bool status, string desc) Core(string invokePath, InvokeMethod method, string parameter) {
            var gamePath = Config.Read()["GamePath"];
            var folder = new DirectoryInfo(invokePath);

            //correct slash error
            if (invokePath[invokePath.Count()-1] != '\\') {
                invokePath += "\\";
            }

            var file = folder.GetFiles("setup-*.*");
            if (!file.Any()) return (false, I18N.Core("ScriptInvoker_NoScriptFile"));
            for (int i = 1; i <= file.Count(); i++) {
                var cacheFile = folder.GetFiles($"setup-{i}.*")[0];
                var cache = PackageAssistance.GetScriptInfo(cacheFile.Name);
                (bool status, string desc) result;
                switch (cache.suffix) {
                    case ".py":
                        result = PythonInvoker(cacheFile.FullName, method, gamePath, invokePath, parameter);
                        break;
                    default:
                        return (false, I18N.Core("ScriptInvoker_UnsupportedScript"));
                }
                if (!result.status) return result;
            }

            return (true, "");
        }

        static (bool status, string desc) PythonInvoker(string file, InvokeMethod method, string game_path, string current_folder, string parameter) {
            try {
                ScriptEngine pyEngine = Python.CreateEngine();
                dynamic dd = pyEngine.ExecuteFile(file);
                switch (method) {
                    case InvokeMethod.Install:
                        var cache1 = dd.install(game_path, current_folder);
                        return ((bool)cache1[0], (string)cache1[1]);
                    case InvokeMethod.Deploy:
                        var cache2 = dd.deploy(game_path, current_folder, parameter);
                        return ((bool)cache2[0], (string)cache2[1]);
                    case InvokeMethod.Remove:
                        var cache3 = dd.remove(game_path, current_folder);
                        return ((bool)cache3[0], (string)cache3[1]);
                    case InvokeMethod.Help:
                        return (true, (string)dd.help());
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
