using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BPMScriptDebugger {
    public class SettingsManager {

        public SettingsManager() {
            reset();
        }

        string settings;

        void reset() {
            var blank = new Dictionary<string, string>();
            settings = JsonConvert.SerializeObject(blank);
        }

        public Dictionary<string, string> GetSettings() {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(settings);
        }

        public void SetSettings(Dictionary<string, string> data) {
            settings = JsonConvert.SerializeObject(data);
        }

        public void CleanSettings() {
            reset();
        }

    }
}
