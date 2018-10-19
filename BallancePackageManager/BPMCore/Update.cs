using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager.BPMCore {
    static class Update {

        public static void Core() {
            var config = Config.Read()["Sources"].Split(':');
            int port = int.Parse(config[config.Length - 1]);
            var host = string.Join(":", config, 0, config.Length - 2);

            //backup old database
            File.Move(ConsoleAssistance.WorkPath + @"package.db", ConsoleAssistance.WorkPath + @"package.db.old");
            var res = Download.DownloadDatabase();
            Console.WriteLine(Download.JudgeDownloadResult(res));

            if (res == Download.DownloadResult.OK) {
                File.Delete(ConsoleAssistance.WorkPath + @"package.db.old");
                Console.WriteLine("Update package list successfully.");
            } else {
                File.Delete(ConsoleAssistance.WorkPath + @"package.db");
                File.Move(ConsoleAssistance.WorkPath + @"package.db.old", ConsoleAssistance.WorkPath + @"package.db");
                Console.WriteLine("Fail to update package list");
            }
                

        }

    }
}
