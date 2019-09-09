using ShareLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO;
using ShareLib;

namespace BallancePackageManager {

    public partial class BPMInstance {

        #region general method

        public void InitInstance() {
            //detect local cache folder
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("download").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("download").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("installed").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("installed").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("decompress").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("decompress").Path);
            if (!Directory.Exists(Information.WorkPath.Enter("cache").Enter("dependency").Path))
                Directory.CreateDirectory(Information.WorkPath.Enter("cache").Enter("dependency").Path);

            //init config
            ConfigManager = new Config("config.cfg", new Dictionary<string, string>() {
                    {"Sources" , "yyc.bkt.moe:3850" },
                    {"GamePath" , "" },
                    {"Language", "en-us" }
                });

            //init i18n
            I18N.Init(ConfigManager.Configuration["Language"]);

            CurrentStatus = BPMInstanceStatus.Ready;
        }

        public void DisposeInstance() {

        }

        #endregion

        BPMInstanceStatus CurrentStatus = BPMInstanceStatus.ErrorInit;
        ShareLib.Config ConfigManager;

        #region event list

        public event Action<BPMInstanceMethod> BPMInstanceEvent_MethodDone;

        public event Action<BPMInstanceMethod, string> BPMInstanceEvent_Error;

        public event Action<BPMInstanceMethod, string> BPMInstanceEvent_Message;

        #endregion

        #region helper methods

        private bool HaveGamePath(BPMInstanceMethod invokeMethod) {
            if (ConfigManager.Configuration["GamePath"] != "") return true;
            else {
                BPMInstanceEvent_Error?.Invoke(invokeMethod, $"No vaild GamePath value.");//todo i18n
                return false;
            }
        }

        private bool HaveDatabase(BPMInstanceMethod invokeMethod) {
            if (File.Exists(Information.WorkPath.Enter("package.db").Path)) return true;
            else {
                BPMInstanceEvent_Error?.Invoke(invokeMethod, $"No database file.");//todo i18n
                return false;
            }
        }

        private bool CheckStatus(BPMInstanceMethod invokeMethod, BPMInstanceStatus expect) {
            if (expect == CurrentStatus) return true;
            else {
                BPMInstanceEvent_Error?.Invoke(invokeMethod ,$"Error status. Expect {expect} get {CurrentStatus}.");//todo i18n
                return false;
            }
        }

        #endregion


    }

    public enum BPMInstanceMethod {
        Update,
        Search,
        Install,
        List,
        Remove,
        Config,
        Show,
        Deploy,
        Guide,
        Clean
    }

    public enum BPMInstanceStatus {
        ErrorInit,
        Ready,
        Working,
        WaitInstall,
        WaitRemove
    }


}
