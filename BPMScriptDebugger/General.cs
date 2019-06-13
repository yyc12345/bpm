using ShareLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BPMScriptDebugger {
    public static class General {
        public static Config ConfigManager;
        public static SettingsManager ScriptSettings;

        public static AppStatus CurrentAppStatus;

        public static string CodeTemplate;
        public static Assembly LoadedModule;

    }
}
