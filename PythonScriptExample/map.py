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
        # restore old
        deploy_cache = read_deploy(current_folder + "\\deploy.cfg")
        if not deploy_cache == "":
            old_level = int(deploy_cache)
            target_old_file = game_path + "3D Entities\\Level\\Level_" + ("0" if old_level < 10 else "" ) + deploy_cache + ".NMO"
            remove_with_restore(target_old_file)

        # deploy new
        level = int(parameter)
        if not (level >= 1 and level <= 15):
            return False
        target_file = game_path + "3D Entities\\Level\\Level_" + ("0" if level < 10 else "" ) + str(level) + ".NMO"
        copy_with_backups(target_file, current_folder + "\\MapNameInPackage.nmo")
        record_deploy(current_folder + "\\deploy.cfg", str(level))
    except:
        return False
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path, current_folder):
    try:
        # restore old
        deploy_cache = read_deploy(current_folder + "\\deploy.cfg")
        if deploy_cache == "":
            return True
        int_level = int(deploy_cache)
        target_file = game_path + "3D Entities\\Level\\Level_" + ("0" if int_level < 10 else "" ) + deploy_cache + ".NMO"
        remove_with_restore(target_file)
    except:
        return False
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

def record_deploy(file, value):
    with opne(file, "w", encoding="utf-8") as f:
        f.write(value)

def read_deploy(file):
    if not os.path.exists(file):
        return ""
    
    with opne(file, "r", encoding="utf-8") as f:
        return f.read()
