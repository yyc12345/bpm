import os
import shutil
# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    try:
        level = int(parameter)
        target_file = "3D Entities\\Level\\Level_" + ("0" if level < 10 else "" ) + str(level)
        if not os.path.exists(target_file + ".bak" ):
            shutil.copyfile(target_file + ".NMO",target_file + ".bak")
        
        os.remove()
        shutil.copyfile(current_folder + "\\MapNameInPackage.nmo",target_file + ".NMO")
    except:
        return False

    return True

# Return true to report that package is intact, otherwise return false
def check(game_path):
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path):
    return True
