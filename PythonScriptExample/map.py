import os
import shutil
# both of game_path and current_folder have slash
# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def install(game_path, current_folder):
    return True, ""

# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def deploy(game_path, current_folder, parameter):
    try:
        # restore old
        deploy_cache = read_deploy(current_folder + "deploy.cfg")
        if not deploy_cache == "":
            old_level = int(deploy_cache)
            target_old_file = game_path + "3D Entities\\Level\\Level_" + ("0" if old_level < 10 else "" ) + deploy_cache + ".NMO"
            remove_with_restore(target_old_file)
        record_deploy(current_folder + "deploy.cfg", "")

        # deploy new
        if parameter == "":
            return True, ""
        level = int(parameter)
        if not (level >= 1 and level <= 15):
            return False, "Illegal parameter range"
        target_file = game_path + "3D Entities\\Level\\Level_" + ("0" if level < 10 else "" ) + str(level) + ".NMO"
        copy_with_backups(target_file, current_folder + "\\MapNameInPackage.nmo")
        record_deploy(current_folder + "deploy.cfg", str(level))
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def remove(game_path, current_folder):
    try:
        # restore old
        deploy_cache = read_deploy(current_folder + "deploy.cfg")
        if deploy_cache == "":
            return True
        int_level = int(deploy_cache)
        target_file = game_path + "3D Entities\\Level\\Level_" + ("0" if int_level < 10 else "" ) + deploy_cache + ".NMO"
        remove_with_restore(target_file)
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return string. string is help message. If there are no help message which can be provided, return ""
def help():
    return "Level deploy help:\n\
    Parameter formation: LEVEL-INDEX\n\
    Parameter example: 1, 12\n\
    \n\
    LEVEL-INDEX's legal value range is from 1 to 15\n"

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
    with open(file, "w", encoding="utf-8") as f:
        f.write(value)

def read_deploy(file):
    if not os.path.exists(file):
        return ""
    
    with open(file, "r", encoding="utf-8") as f:
        return f.read()
