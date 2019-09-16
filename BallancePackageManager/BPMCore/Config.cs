﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ShareLib;

namespace BallancePackageManager {

    public partial class BPMInstance {

        public Dictionary<string, string> Config_Export = new Dictionary<string, string>();

        public string Config_ExportItem;

        #region wrapper

        public void Config_Wrapper() {
            CurrentStatus = BPMInstanceStatus.Working;
            Config_Core();
            CurrentStatus = BPMInstanceStatus.Ready;
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.Search);
        }

        public void Config_Wrapper(string item) {
            CurrentStatus = BPMInstanceStatus.Working;
            Config_Core(item);
            CurrentStatus = BPMInstanceStatus.Ready;
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.Search);
        }

        public void Config_Wrapper(string item, string newValue) {
            CurrentStatus = BPMInstanceStatus.Working;
            Config_Core(item, newValue);
            CurrentStatus = BPMInstanceStatus.Ready;
            OnBPMInstanceEvent_MethodDone(BPMInstanceMethod.Search);
        }

        #endregion
        
        #region core

        private void Config_Core() {
            Config_Export.Clear();
            foreach(var item in ConfigManager.Configuration.Keys) 
                Config_Export.Add(item, ConfigManager.Configuration[item]);
        }

        private void Config_Core(string item) {
            if (ConfigManager.Configuration.ContainsKey(item)) Config_ExportItem = ConfigManager.Configuration[item];
            else OnBPMInstanceEvent_Error(BPMInstanceMethod.Config, "No matched item.");//todo:i18n
        }

        private void Config_Core(string item, string newValue) {
            if (ConfigManager.Configuration.ContainsKey(item)) ConfigManager.Configuration[item] = newValue;
            else OnBPMInstanceEvent_Error(BPMInstanceMethod.Config, "No matched item.");//todo:i18n
        }

        #endregion

    }
    
}
