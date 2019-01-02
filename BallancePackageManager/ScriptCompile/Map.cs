using ShareLib;
using System;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPreCompile {
    public class Map {
        public static (bool status, string desc) Install(string gamePath, string currentPath) {
            try {
                //Pass
            } catch (Exception e) {
                return (false, "Runtime error:\n" + e.Message);
            }
            return (true, "");
        }

        public static (bool status, string desc) Remove(string gamePath, string currentPath) {
            try {
                var _gamePath = new FilePathBuilder(gamePath);
                var _currentPath = new FilePathBuilder(currentPath);

                var deploy_cache = ScriptCommon.ReadDeploy(_currentPath.Enter("deploy.cfg").Path);
                if (deploy_cache == "") return (true, "");
                var int_level = int.Parse(deploy_cache);
                var target_file = _gamePath.Enter("3D Entities").Enter("Level").Enter($"Level_{(int_level < 10 ? "0" : "")}{deploy_cache}.NMO").Path;
                ScriptCommon.RemoveWithRestore(target_file);
            } catch (Exception e) {
                return (false, "Runtime error:\n" + e.Message);
            }
            return (true, "");
        }

        public static (bool status, string desc) Deploy(string gamePath, string currentPath, string parameter) {
            try {
                var _gamePath = new FilePathBuilder(gamePath, Information.OS);
                var _currentPath = new FilePathBuilder(currentPath);

                var cache_file = _currentPath.Enter("deploy.cfg").Path;
                _currentPath.Backtracking();
                var local_map_file = _currentPath.Enter("MapNameInPackage.nmo").Path;
                var target_map_folder = _gamePath.Enter("3D Entities").Enter("Level").Path;

                //restore old
                var deploy_cache = ScriptCommon.ReadDeploy(cache_file);
                if (deploy_cache != "") {
                    var old_level = int.Parse(deploy_cache);
                    var target_old_file = new FilePathBuilder(target_map_folder).Enter($"Level_{(old_level < 10 ? "0" : "")}{deploy_cache}.NMO").Path;
                    ScriptCommon.RemoveWithRestore(target_old_file);
                }
                ScriptCommon.RecordDeploy(cache_file, "");

                //deploy new
                _gamePath = new FilePathBuilder(gamePath);
                _currentPath = new FilePathBuilder(currentPath);

                if (parameter == "") return (true, "");
                var level = int.Parse(parameter);
                if (!(level >= 1 && level <= 15)) return (false, "Illegal parameter range");
                var target_file = new FilePathBuilder(target_map_folder).Enter($"Level_{(level < 10 ? "0" : "")}{deploy_cache}.NMO").Path;
                ScriptCommon.CopyWithBackups(target_file, local_map_file);
                ScriptCommon.RecordDeploy(cache_file, level.ToString());
            } catch (Exception e) {
                return (false, "Runtime error:\n" + e.Message);
            }
            return (true, "");
        }

        public static string Help() {
            return "Level deploy help:" + Environment.NewLine +
            "Parameter formation: LEVEL-INDEX" + Environment.NewLine +
            "Parameter example: 1, 12" + Environment.NewLine +
            Environment.NewLine +
            "LEVEL-INDEX's legal value range is from 1 to 15";
        }



    }
}