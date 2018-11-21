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

legal_character_dict = [
    'A',
    'B',
    'C',
    'D',
    'E',
    'F',
    'G',
    'H',
    'I',
    'J',
    'K',
    'L',
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
            target_old_file = game_path + "Textures\\Sky\\Sky_" + deploy_cache
            remove_with_restore(target_old_file + "_Back.BMP")
            remove_with_restore(target_old_file + "_Down.BMP")
            remove_with_restore(target_old_file + "_Front.BMP")
            remove_with_restore(target_old_file + "_Left.BMP")
            remove_with_restore(target_old_file + "_Right.BMP")

        # deploy new
        sky = 'A'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                return False
            sky = level_dict[parameter[1]]
        elif (parameter[0] == 'sky'):
            sky = parameter[1].upper()
        else:
            return False
        
        if not sky in legal_character_dict:
            return False

        target_file = game_path + "Textures\\Sky\\Sky_" + sky
        copy_with_backups(target_file + "_Back.BMP", current_folder + "\\back.bmp")
        copy_with_backups(target_file + "_Down.BMP", current_folder + "\\down.bmp")
        copy_with_backups(target_file + "_Front.BMP", current_folder + "\\front.bmp")
        copy_with_backups(target_file + "_Left.BMP", current_folder + "\\left.bmp")
        copy_with_backups(target_file + "_Right.BMP", current_folder + "\\right.bmp")

        record_deploy(current_folder + "\\deploy.cfg", sky)
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
        
        target_file = game_path + "Textures\\Sky\\Sky_" + deploy_cache
        remove_with_restore(target_file + "_Back.BMP")
        remove_with_restore(target_file + "_Down.BMP")
        remove_with_restore(target_file + "_Front.BMP")
        remove_with_restore(target_file + "_Left.BMP")
        remove_with_restore(target_file + "_Right.BMP")
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
