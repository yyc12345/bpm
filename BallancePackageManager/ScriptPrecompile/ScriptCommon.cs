using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPrecompile {
    public static class ScriptCommon {
        //============================================= assistant function

        public static void CopyWithBackups(string target, string origin) {
            if (File.Exists(target)) {
                if (!File.Exists(target + ".bak")) File.Move(target, target + ".bak");
                else File.Delete(target);
            }

            File.Copy(origin, target);
        }

        public static void RemoveWithRestore(string target) {
            if (File.Exists(target))
                File.Delete(target);
            if (File.Exists(target + ".bak"))
                File.Move(target + ".bak", target);
        }

        public static void RecordDeploy(string file, string value) {
            using (var fs = new StreamWriter(file, false, Encoding.UTF8)) {
                fs.Write(value);
                fs.Close();
            }
        }

        public static string ReadDeploy(string file) {
            if (!File.Exists(file)) return "";
            string res = "";
            using (var fs = new StreamReader(file, Encoding.UTF8)) {
                res = fs.ReadToEnd();
                fs.Close();
            }

            return res;
        }
    }
}
