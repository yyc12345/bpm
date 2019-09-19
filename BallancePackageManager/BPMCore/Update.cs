using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib;

namespace BallancePackageManager {

    public partial class BPMInstance {

        public void Update_Wrapper() {
            if (!CheckStatus(BPMInstanceMethod.Update, BPMInstanceStatus.Ready)) return;
            OnBPMInstanceEvent_MethodBegin(BPMInstanceMethod.Config);
            Update_Core();
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.Update);
        }

        private void Update_Core() {
            var config = ConfigManager.Configuration["Sources"].Split(':');
            int port = int.Parse(config[config.Length - 1]);
            var host = string.Join(":", config, 0, config.Length - 2);

            //backup old database
            if (File.Exists(Information.WorkPath.Enter("package.db").Path))
                File.Move(Information.WorkPath.Enter("package.db").Path, Information.WorkPath.Enter("package.db.old").Path);
            var res = Download.DownloadDatabase();
            OnBPMInstanceEvent_Message(BPMInstanceMethod.Update, Download.JudgeDownloadResult(res));

            if (res == Download.DownloadResult.OK) {
                File.Delete(Information.WorkPath.Enter("package.db.old").Path);
                OnBPMInstanceEvent_Message(BPMInstanceMethod.Update, I18N.Core("Update_Success"));
            } else {
                //File.Delete(Information.WorkPath.Enter("package.db.old").Path);
                if (File.Exists(Information.WorkPath.Enter("package.db.old").Path))
                    File.Move(Information.WorkPath.Enter("package.db.old").Path, Information.WorkPath.Enter("package.db").Path);
                OnBPMInstanceEvent_Error(BPMInstanceMethod.Update, I18N.Core("Update_Fail"));
            }


        }

    }
}
