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
        if not (level >= 1 and level <= 15):
            return False
        target_file = game_path + "3D Entities\\Level\\Level_" + ("0" if level < 10 else "" ) + str(level) + ".NMO"
        copy_with_backups(target_file, current_folder + "\\MapNameInPackage.nmo")
    except:
        return False
    return True

# Return true to report that package is intact, otherwise return false
def check(game_path, current_folder):
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path):
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