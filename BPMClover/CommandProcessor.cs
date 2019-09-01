using CommandLine;
using ShareLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMClover {
    public static class CommandProcessor {

        static Parser parser = new Parser(ps => {
            ps.AutoHelp = false;
            ps.AutoVersion = false;
            ps.HelpWriter = null;
            ps.EnableDashDash = true;
        });

        public static bool Process(string[] command) {
            return parser.ParseArguments<HelpOption, UpdateOption, SearchOption, InstallOption, ListOption, RemoveOption, ConfigOption, ShowOption, DeployOption, GuideOption, CleanOption>(command)
                .MapResult(//todo: finish command processor
                (UpdateOption opt) => {
                    
                    return false;
                },
                (SearchOption opt) => {
                    //
                    return false;
                },
                (InstallOption opt) => {
                    //
                    return false;
                },
                (ListOption opt) => {
                    //
                    return false;
                },
                (RemoveOption opt) => {
                    //
                    return false;
                },
                (ConfigOption opt) => {
                    //
                    return false;
                },
                (ShowOption opt) => {
                    //
                    return false;
                },
                (DeployOption opt) => {
                    //
                    return false;
                },
                (GuideOption opt) => {
                    //
                    return false;
                },
                (CleanOption opt) => {
                    //
                    return false;
                },
                (HelpOption opt) => {
                    //OutputHelp();
                    return false;
                },
                errs => { ConsoleAssistance.WriteLine("Unknow command. Use help to find the correct command", ConsoleColor.Red); return false; });//todo: i18n

        }

    }


    #region command structure

    [Verb("help")]
    public class HelpOption {

    }

    [Verb("update")]
    public class UpdateOption {

    }

    [Verb("search")]
    public class SearchOption {
        [Value(0, Required = true, Min = 1)]
        public IEnumerable<string> Parameters { get; set; }
    }

    [Verb("list")]
    public class ListOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
    }

    [Verb("remove")]
    public class RemoveOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
    }

    [Verb("install")]
    public class InstallOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
    }

    [Verb("config")]
    public class ConfigOption {
        [Value(0)]
        public string Key { get; set; }
        [Value(1)]
        public string NewValue { get; set; }
    }

    [Verb("show")]
    public class ShowOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
    }

    [Verb("deploy")]
    public class DeployOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
        [Value(1, Required = true, Min = 1)]
        public IEnumerable<string> Parameters { get; set; }
    }

    [Verb("guide")]
    public class GuideOption {
        [Value(0, Required = true)]
        public string PackageName { get; set; }
    }

    [Verb("clean")]
    public class CleanOption {
        [Value(0)]
        public string PackageName { get; set; }
    }

    #endregion

}
