using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager.BPMCore {
    static class Update {

        public static void Core() {
            var config = Config.Read()["Sources"].Split(':');
            int port = int.Parse(config[config.Length - 1]);
            var host = string.Join(":", config, 0, config.Length - 2);

            var res = Download.DownloadDatabase();
            Console.WriteLine(Download.JudgeDownloadResult(res));

            if (res == Download.DownloadResult.OK)
                Console.WriteLine("Update package list successfully.");
            else
                Console.WriteLine("Fail to update package list");

        }

    }
}
