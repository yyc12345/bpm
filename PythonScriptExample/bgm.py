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

# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    try:
        theme = '1'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                return False
            theme = level_dict[parameter[1]]
        elif (parameter[0] == 'theme'):
            test_theme = int(parameter[1])
            if not (test_theme >= 1 and test_theme <= 5):
                return False
            theme = parameter[1]
        else:
            return Flase
        
        target_file_1 = game_path + "Sounds\\Music_Theme_" + theme + "_1.wav"
        target_file_2 = game_path + "Sounds\\Music_Theme_" + theme + "_2.wav"
        target_file_3 = game_path + "Sounds\\Music_Theme_" + theme + "_3.wav"
        copy_with_backups(target_file_1, current_folder + "\\1.wav")
        copy_with_backups(target_file_2, current_folder + "\\2.wav")
        copy_with_backups(target_file_3, current_folder + "\\3.wav")
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