using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BallancePackageManager.BPMCore {
    static class Update {

        public static void Core() {
            var config = Config.Read()["Sources"].Split(':');
            int port = int.Parse(config[config.Length - 1]);
            var host = string.Join(":", config, 0, config.Length - 2);

            //backup old database
            File.Move(Information.WorkPath.Enter("package.db").Path, Information.WorkPath.Enter("package.db.old").Path);
            var res = Download.DownloadDatabase();
            Console.WriteLine(Download.JudgeDownloadResult(res));

            if (res == Download.DownloadResult.OK) {
                File.Delete(Information.WorkPath.Enter("package.db.old").Path);
                Console.WriteLine(I18N.Core("Update_Success"));
            } else {
                //File.Delete(Information.WorkPath.Enter("package.db.old").Path);
                File.Move(Information.WorkPath.Enter("package.db.old").Path, Information.WorkPath.Enter("package.db").Path);
                Console.WriteLine(I18N.Core("Update_Fail"));
            }
                

        }

    }
}
