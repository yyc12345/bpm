using System;
using System.Collections.Generic;
using ShareLib;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPrecompile {
    public class BGM {

        private static readonly Dictionary<int, string> level_dict = new Dictionary<int, string>() {
            {1, "1"},
            {2, "5"},
            {3, "2"},
            {4, "3"},
            {5, "1"},
            {6, "5"},
            {7, "4"},
            {8, "2"},
            {9, "3"},
            {10, "1"},
            {11, "3"},
            {12, "4"},
            {13, "5"},
            {14, "5"},
            {15, "5"}
        };

        private static readonly List<string> legal_character_dict = new List<string>() {
            "1",
            "2",
            "3",
            "4",
            "5"
        };


        public static (bool status, string desc) Install(string gamePath, string currentPath) {
            return (true, "");
        }

        public static (bool status, string desc) Remove(string gamePath, string currentPath) {
            try {
                var _gamePath = new FilePathBuilder(gamePath);
                var _currentPath = new FilePathBuilder(currentPath);

                var deploy_cache = ScriptCommon.ReadDeploy(_currentPath.Enter("deploy.cfg").Path);
                if (deploy_cache == "") return (true, "");


                var target_file = _gamePath.Enter("Sounds").Enter($"Music_Theme_{deploy_cache}").Path;
                ScriptCommon.RemoveWithRestore(target_file + "_1.wav");
                ScriptCommon.RemoveWithRestore(target_file + "_2.wav");
                ScriptCommon.RemoveWithRestore(target_file + "_3.wav");
            } catch (Exception e) {
                return (false, "Runtime error:" + Environment.NewLine + e.Message);
            }
            return (true, "");
        }

        public static (bool status, string desc) Deploy(string gamePath, string currentPath, string parameter) {
            try {
                var _gamePath = new FilePathBuilder(gamePath, Information.OS);
                var _currentPath = new FilePathBuilder(currentPath);

                var cache_file = _currentPath.Enter("deploy.cfg").Path;
                _currentPath.Backtracking();
                var local_file_folder = _currentPath.Path;
                var target_folder = _gamePath.Enter("Sounds").Path;

                //restore old
                var deploy_cache = ScriptCommon.ReadDeploy(cache_file);
                if (deploy_cache != "") {
                    var target_old_file = new FilePathBuilder(target_folder).Enter($"Music_Theme_{deploy_cache}").Path;
                    ScriptCommon.RemoveWithRestore(target_old_file + "_1.wav");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_2.wav");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_3.wav");
                }
                ScriptCommon.RecordDeploy(cache_file, "");

                //deploy new
                _gamePath = new FilePathBuilder(gamePath);
                _currentPath = new FilePathBuilder(currentPath);

                if (parameter == "") return (true, "");
                var theme = "1";
                var sp_param = parameter.Split('_');
                if (sp_param[0] == "level") {
                    var level = int.Parse(sp_param[1]);
                    if (!(level >= 1 && level <= 15)) return (false, "Illegal parameter range");
                    theme = level_dict[level];
                } else if (sp_param[0] == "theme") {
                    theme = sp_param[1];
                } else return (false, "Illegal formation");

                if (!legal_character_dict.Contains(theme)) return (false, "Illegal formation");


                var target_file = new FilePathBuilder(target_folder).Enter($"Music_Theme_{theme}").Path;
                ScriptCommon.CopyWithBackups(target_file + "_1.wav", new FilePathBuilder(local_file_folder).Enter("1.wav").Path);
                ScriptCommon.CopyWithBackups(target_file + "_2.wav", new FilePathBuilder(local_file_folder).Enter("2.wav").Path);
                ScriptCommon.CopyWithBackups(target_file + "_3.wav", new FilePathBuilder(local_file_folder).Enter("3.wav").Path);

                ScriptCommon.RecordDeploy(cache_file, theme);
            } catch (Exception e) {
                return (false, "Runtime error:" + Environment.NewLine + e.Message);
            }
            return (true, "");
        }

        public static string Help() {
            return "BGM deploy help:" + Environment.NewLine +
            "Parameter formation: [ level_LEVEL-INDEX | theme_THEME-INDEX ]" + Environment.NewLine +
            "Parameter example: level_5, theme_3" + Environment.NewLine +
            Environment.NewLine +
            "LEVEL-INDEX's legal value range is from 1 to 15" + Environment.NewLine +
            "THEME-INDEX's legal value range is from 1 to 5" + Environment.NewLine +
            "Using level_ formation to deploy is easy and if you are a professor of Ballance, you can use theme_ deploy method.";
        }



    }
}

