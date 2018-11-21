import os
import shutil

file_list = [
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
]

# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    for item in file_list:
        if os.path.exists(current_folder + "\\" + item):
            copy_with_backups(game_path + "\\Sounds\\" + item, current_folder + "\\" + item)
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    return True

# Return true to report that package is intact, otherwise return false
def check(game_path, current_folder):
    for item in file_list:
        if not os.path.exists(current_folder + "\\" + item):
            continue
        if not os.path.exists(game_path + "\\Sounds\\" + item):
            return False
        if os.path.getsize(game_path + "\\Sounds\\" + item) != os.path.getsize(current_folder + "\\" + item):
            return False
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path, current_folder):
    for item in file_list:
        if os.path.exists(current_folder + "\\" + item):
            remove_with_restore(game_path + "\\Sounds\\" + item)
    return True

# ========================================== assistant function

def copy_with_backups(target, origin):
    if os.path.exists(target):
        if not os.path.exists(target + ".bak"):
            os.rename(target, target+ ".bak")
        else:
            os.remove(target)
    shutil.copyfile(origin, target)

def remove_with_restore(target):
    if os.path.exists(target):
        os.remove(target)
    if os.path.exists(target+".bak"):
        os.rename(target+".bak", target)
