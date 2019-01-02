using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ShareLib {
    public static class CommandHelper {
        public static string RunCommand(string file, string argument, bool wait) {
            var process = new Process() {
                StartInfo = new ProcessStartInfo {
                    FileName = file,
                    Arguments = argument,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            process.Start();
            var res = process.StandardOutput.ReadToEnd();
            if (wait) process.WaitForExit();

            return res;
        }
    }
}
