import os
import shutil

level_dict = {
    "1": "1",
    "2": "5",
    "3": "2",
    "4": "3",
    "5": "1",
    "6": "5",
    "7": "4",
    "8": "2",
    "9": "3",
    "10": "1",
    "11": "3",
    "12": "4",
    "13": "5",
    "14": "5",
    "15": "5"
}

legal_character_dict = [
    '1',
    '2',
    '3',
    '4',
    '5',
]

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
            target_file_1 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_1.wav"
            target_file_2 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_2.wav"
            target_file_3 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_3.wav"
            remove_with_restore(target_file_1)
            remove_with_restore(target_file_2)
            remove_with_restore(target_file_3)

        # deploy new
        theme = '1'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                return False
            theme = level_dict[parameter[1]]
        elif (parameter[0] == 'theme'):
            theme = parameter[1]
        else:
            return Flase

        if not theme in legal_character_dict:
            return False
        
        target_file_1 = game_path + "Sounds\\Music_Theme_" + theme + "_1.wav"
        target_file_2 = game_path + "Sounds\\Music_Theme_" + theme + "_2.wav"
        target_file_3 = game_path + "Sounds\\Music_Theme_" + theme + "_3.wav"
        copy_with_backups(target_file_1, current_folder + "\\1.wav")
        copy_with_backups(target_file_2, current_folder + "\\2.wav")
        copy_with_backups(target_file_3, current_folder + "\\3.wav")

        record_deploy(current_folder + "\\deploy.cfg", theme)
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
        
        target_file_1 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_1.wav"
        target_file_2 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_2.wav"
        target_file_3 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_3.wav"
        remove_with_restore(target_file_1)
        remove_with_restore(target_file_2)
        remove_with_restore(target_file_3)
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
