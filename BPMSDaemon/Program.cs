using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMSDaemon {
    class Program {
        static void Main(string[] args) {
            /*
            while (true) {

                var cache = Process.GetProcessesByName("BPMServer");
                if (!cache.Any()) {
                    Process.Start(ConsoleAssistance.WorkPath +  "BPMServer.exe");
                    Console.WriteLine("Restart server...");
                }

                Console.WriteLine("Waiting next check...");
                System.Threading.Thread.Sleep(30 * 1000);
            }
            */
            Console.WriteLine("Remove temporary...");
        }
    }
}
