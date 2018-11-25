import os
import shutil
# both of game_path and current_folder have slash
# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def install(game_path, current_folder):
    try:
        pass
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def deploy(game_path, current_folder, parameter):
    return True, ""

# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def remove(game_path, current_folder):
    try:
        pass
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return string. string is help message. If there are no help message which can be provided, return ""
def help():
    return ""

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