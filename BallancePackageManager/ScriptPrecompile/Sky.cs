using System;
using System.Collections.Generic;
using ShareLib;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPrecompile {
    public class Sky {

        private static readonly Dictionary<int, string> level_dict = new Dictionary<int, string>() {
            {1, "L"},
            {2, "E"},
            {3, "A"},
            {4, "F"},
            {5, "C"},
            {6, "H"},
            {7, "D"},
            {8, "G"},
            {9, "K"},
            {10, "B"},
            {11, "J"},
            {12, "I"},
            {13, "F"},
            {14, "F"},
            {15, "F"}
        };

        private static readonly List<string> legal_character_dict = new List<string>() {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L"
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

                
                var target_file = _gamePath.Enter("Textures").Enter("Sky").Enter($"Sky_{deploy_cache}").Path;
                ScriptCommon.RemoveWithRestore(target_file + "_Back.BMP");
                ScriptCommon.RemoveWithRestore(target_file + "_Down.BMP");
                ScriptCommon.RemoveWithRestore(target_file + "_Front.BMP");
                ScriptCommon.RemoveWithRestore(target_file + "_Left.BMP");
                ScriptCommon.RemoveWithRestore(target_file + "_Right.BMP");
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
                var target_folder = _gamePath.Enter("Textures").Enter("Sky").Path;

                //restore old
                var deploy_cache = ScriptCommon.ReadDeploy(cache_file);
                if (deploy_cache != "") {
                    var target_old_file = new FilePathBuilder(target_folder).Enter($"Sky_{deploy_cache}").Path;
                    ScriptCommon.RemoveWithRestore(target_old_file + "_Back.BMP");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_Down.BMP");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_Front.BMP");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_Left.BMP");
                    ScriptCommon.RemoveWithRestore(target_old_file + "_Right.BMP");
                }
                ScriptCommon.RecordDeploy(cache_file, "");

                //deploy new
                _gamePath = new FilePathBuilder(gamePath);
                _currentPath = new FilePathBuilder(currentPath);

                if (parameter == "") return (true, "");
                var sky = "A";
                var sp_param = parameter.Split('_');
                if (sp_param[0] == "level") {
                    var level = int.Parse(sp_param[1]);
                    if (!(level >= 1 && level <= 15)) return (false, "Illegal parameter range");
                    sky = level_dict[level];
                } else if (sp_param[0] == "sky") {
                    sky = sp_param[1].ToUpper();
                } else return (false, "Illegal formation");

                if (!legal_character_dict.Contains(sky)) return (false, "Illegal formation");

                
                var target_file = new FilePathBuilder(target_folder).Enter($"Sky_{sky}").Path;
                ScriptCommon.CopyWithBackups(target_file + "_Back.BMP", new FilePathBuilder(local_file_folder).Enter("back.bmp").Path);
                ScriptCommon.CopyWithBackups(target_file + "_Down.BMP", new FilePathBuilder(local_file_folder).Enter("down.bmp").Path);
                ScriptCommon.CopyWithBackups(target_file + "_Front.BMP", new FilePathBuilder(local_file_folder).Enter("front.bmp").Path);
                ScriptCommon.CopyWithBackups(target_file + "_Left.BMP", new FilePathBuilder(local_file_folder).Enter("left.bmp").Path);
                ScriptCommon.CopyWithBackups(target_file + "_Right.BMP", new FilePathBuilder(local_file_folder).Enter("right.bmp").Path);

                ScriptCommon.RecordDeploy(cache_file, sky);
            } catch (Exception e) {
                return (false, "Runtime error:" + Environment.NewLine + e.Message);
            }
            return (true, "");
        }

        public static string Help() {
            return "Sky deploy help:" + Environment.NewLine +
            "Parameter formation: [ level_LEVEL-INDEX | sky_SKY-INDEX ]" + Environment.NewLine +
            "Parameter example: level_6, sky_D" + Environment.NewLine +
            Environment.NewLine +
            "LEVEL-INDEX's legal value range is from 1 to 15" + Environment.NewLine +
            "SKY-INDEX's legal value range is from A to L" + Environment.NewLine +
            "Using level_ formation to deploy is easy and if you are a professor of Ballance, you can use sky_ deploy method.";
        }



    }
}
