using System;
using System.Collections.Generic;
using ShareLib;
using System.IO;
using System.Text;

namespace BallancePackageManager.ScriptPrecompile {
    public class Texture {

        private static readonly List<string> file_list = new List<string>() {
            "atari.avi",
            "atari.bmp",
            "Ball_LightningSphere1.bmp",
            "Ball_LightningSphere2.bmp",
            "Ball_LightningSphere3.bmp",
            "Ball_Paper.bmp",
            "Ball_Stone.bmp",
            "Ball_Wood.bmp",
            "Brick.bmp",
            "Button01_deselect.tga",
            "Button01_select.tga",
            "Button01_special.tga",
            "Column_beige.bmp",
            "Column_beige_fade.tga",
            "Column_blue.bmp",
            "Cursor.tga",
            "Dome.bmp",
            "DomeEnvironment.bmp",
            "DomeShadow.tga",
            "E_Holzbeschlag.bmp",
            "ExtraBall.bmp",
            "ExtraParticle.bmp",
            "FloorGlow.bmp",
            "Floor_Side.bmp",
            "Floor_Top_Border.bmp",
            "Floor_Top_Borderless.bmp",
            "Floor_Top_Checkpoint.bmp",
            "Floor_Top_Flat.bmp",
            "Floor_Top_Profil.bmp",
            "Floor_Top_ProfilFlat.bmp",
            "Font_1.tga",
            "Gravitylogo_intro.bmp",
            "HardShadow.bmp",
            "Laterne_Glas.bmp",
            "Laterne_Schatten.tga",
            "Laterne_Verlauf.tga",
            "Logo.bmp",
            "Metal_stained.bmp",
            "Misc_Ufo.bmp",
            "Misc_UFO_Flash.bmp",
            "Modul03_Floor.bmp",
            "Modul03_Wall.bmp",
            "Modul11_13_Wood.bmp",
            "Modul11_Wood.bmp",
            "Modul15.bmp",
            "Modul16.bmp",
            "Modul18.bmp",
            "Modul18_Gitter.tga",
            "Modul30_d_Seiten.bmp",
            "Particle_Flames.bmp",
            "Particle_Smoke.bmp",
            "PE_Bal_balloons.bmp",
            "PE_Bal_platform.bmp",
            "PE_Ufo_env.bmp",
            "P_Extra_Life_Oil.bmp",
            "P_Extra_Life_Particle.bmp",
            "P_Extra_Life_Shadow.bmp",
            "Pfeil.tga",
            "Rail_Environment.bmp",
            "sandsack.bmp",
            "SkyLayer.bmp",
            "Sky_Vortex.bmp",
            "Stick_Bottom.tga",
            "Stick_Stripes.bmp",
            "Target.bmp",
            "Tower_Roof.bmp",
            "Trafo_Environment.bmp",
            "Trafo_FlashField.bmp",
            "Trafo_Shadow_Big.tga",
            "Tut_Pfeil01.tga",
            "Tut_Pfeil_Hoch.tga",
            "Wolken_intro.tga",
            "Wood_Metal.bmp",
            "Wood_MetalStripes.bmp",
            "Wood_Misc.bmp",
            "Wood_Nailed.bmp",
            "Wood_Old.bmp",
            "Wood_Panel.bmp",
            "Wood_Plain2.bmp",
            "Wood_Plain.bmp",
            "Wood_Raft.bmp"
        };


        public static (bool status, string desc) Install(string gamePath, string currentPath) {
            try {
                var _gamePath = new FilePathBuilder(gamePath).Enter("Textures");
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
                var _gamePath = new FilePathBuilder(gamePath).Enter("Textures");
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

