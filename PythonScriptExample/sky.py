import os
import shutil

level_dict = {
    "1": "L",
    "2": "E",
    "3": "A",
    "4": "F",
    "5": "C",
    "6": "H",
    "7": "D",
    "8": "G",
    "9": "K",
    "10": "B",
    "11": "J",
    "12": "I",
    "13": "F",
    "14": "F",
    "15": "F"
}

# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    return True

# Return true to report that package is intact, otherwise return false
def check(game_path, current_folder):
    try:
        sky = 'A'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                return False
            sky = level_dict[parameter[1]]
        elif (parameter[0] == 'sky'):
            test_theme = int(parameter[1])
            if not (test_theme >= 1 and test_theme <= 5):
                return False
            sky = parameter[1]
        else:
            return Flase
        
        target_file = game_path + "Textures\\Sky\\Sky_" + theme
        copy_with_backups(target_file + "_Back.BMP", current_folder + "\\back.bmp")
        copy_with_backups(target_file + "_Down.BMP", current_folder + "\\down.bmp")
        copy_with_backups(target_file + "_Front.BMP", current_folder + "\\front.bmp")
        copy_with_backups(target_file + "_Left.BMP", current_folder + "\\left.bmp")
        copy_with_backups(target_file + "_Right.BMP", current_folder + "\\right.bmp")
    except:
        return False

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