import os
import shutil

file_list = [
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
]

# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    for item in file_list:
        if os.path.exists(current_folder + "\\" + item):
            copy_with_backups(game_path + "\\Textures\\" + item, current_folder + "\\" + item)
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    return True

# Return true to report that package is intact, otherwise return false
def check(game_path, current_folder):
    for item in file_list:
        if not os.path.exists(current_folder + "\\" + item):
            continue
        if not os.path.exists(game_path + "\\Textures\\" + item):
            return False
        if os.path.getsize(game_path + "\\Textures\\" + item) != os.path.getsize(current_folder + "\\" + item):
            return False
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path):
    for item in file_list:
        if os.path.exists(current_folder + "\\" + item):
            remove_with_restore(game_path + "\\Textures\\" + item)
    return True

def copy_with_backups(target, origin):
    if os.path.exists(target):
        if not os.path.exists(target + ".bak"):
            os.rename(target, target+ ".bak")
    shutil.copyfile(origin, target)

def remove_with_restore(target):
    os.remove(target)
    if os.path.exists(target+".bak"):
        os.rename(target+".bak", target)
