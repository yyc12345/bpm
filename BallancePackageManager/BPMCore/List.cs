using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ShareLib;

namespace BallancePackageManager {

    public partial class BPMInstance {

        public List<string> List_Export;

        public void List_Wrapper() {
            if (!CheckStatus(BPMInstanceMethod.List, BPMInstanceStatus.Ready)) return;
            if (!HaveDatabase(BPMInstanceMethod.List)) return;
            OnBPMInstanceEvent_MethodBegin(BPMInstanceMethod.Config);
            List_Core();
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.List);
        }

        private void List_Core() {

            var packageDbConn = new InstalledDatabase();
            packageDbConn.Open();

            OnBPMInstanceEvent_Message(BPMInstanceMethod.List, "Querying installed table...");//todo: i18n
            List_Export = (from item in packageDbConn.CoreDbContext.installed
                           select item.name).ToList();

            packageDbConn.Close();
            
        }

    }
    
}
