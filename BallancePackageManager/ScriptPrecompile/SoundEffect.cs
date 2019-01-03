using System;
using System.Collections.Generic;
using ShareLib;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPrecompile {
    public class SoundEffect {

        private static readonly List<string> file_list = new List<string>() {
            "ATARI.wav",
            "Extra_Hit.wav",
            "Extra_Life_Blob.wav",
            "Extra_Start.wav",
            "Hit_Paper.wav",
            "Hit_Stone_Kuppel.wav",
            "Hit_Stone_Metal.wav",
            "Hit_Stone_Stone.wav",
            "Hit_Stone_Wood.wav",
            "Hit_Wood_Dome.wav",
            "Hit_WoodenFlap.wav",
            "Hit_Wood_Metal.wav",
            "Hit_Wood_Stone.wav",
            "Hit_Wood_Wood.wav",
            "Menu_atmo.wav",
            "Menu_click.wav",
            "Menu_counter.wav",
            "Menu_dong.wav",
            "Menu_load.wav",
            "Misc_Checkpoint.wav",
            "Misc_extraball.wav",
            "Misc_Fall.wav",
            "Misc_Lightning.wav",
            "Misc_RopeTears.wav",
            "Misc_StartLevel.wav",
            "Misc_Trafo.wav",
            "Misc_UFO_anim.wav",
            "Misc_UFO.wav",
            "Misc_Ventilator.wav",
            "Music_Atmo_1.wav",
            "Music_Atmo_2.wav",
            "Music_Atmo_3.wav",
            "Music_EndCheckpoint.wav",
            "Music_Final.wav",
            "Music_Highscore.wav",
            "Music_LastFinal.wav",
            "Music_thunder.wav",
            "Pieces_Paper.wav",
            "Pieces_Stone.wav",
            "Pieces_Wood.wav",
            "Roll_Paper.wav",
            "Roll_Stone_Metal.wav",
            "Roll_Stone_Stone.wav",
            "Roll_Stone_Wood.wav",
            "Roll_Wood_Metal.wav",
            "Roll_Wood_Stone.wav",
            "Roll_Wood_Wood.wav"
        };


        public static (bool status, string desc) Install(string gamePath, string currentPath) {
            try {
                var _gamePath = new FilePathBuilder(gamePath).Enter("Sounds");
                var _currentPath = new FilePathBuilder(currentPath);

                foreach (var item in file_list) {
                    if (File.Exists(_currentPath.Enter(item).Path)) {
                        ScriptCommon.CopyWithBackups(_gamePath.Enter(item).Path, _currentPath.Path);
                        _gamePath.Backtracking();
                    }
                    _currentPath.Backtracking();
                }
            } catch (Exception e) {
                return (false, "Runtime error:" + Environment.NewLine + e.Message);
            }
            return (true, "");
        }

        public static (bool status, string desc) Remove(string gamePath, string currentPath) {
            try {
                var _gamePath = new FilePathBuilder(gamePath).Enter("Sounds");
                var _currentPath = new FilePathBuilder(currentPath);

                foreach (var item in file_list) {
                    if (File.Exists(_currentPath.Enter(item).Path)) {
                        ScriptCommon.RemoveWithRestore(_gamePath.Enter(item).Path);
                        _gamePath.Backtracking();
                    }
                    _currentPath.Backtracking();
                }
            } catch (Exception e) {
                return (false, "Runtime error:" + Environment.NewLine + e.Message);
            }
            return (true, "");
        }

        public static (bool status, string desc) Deploy(string gamePath, string currentPath, string parameter) {
            return (true, "");
        }

        public static string Help() {
            return "";
        }



    }
}

