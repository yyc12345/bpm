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
            target_old_file = game_path + "Textures\\Sky\\Sky_" + deploy_cache
            remove_with_restore(target_old_file + "_Back.BMP")
            remove_with_restore(target_old_file + "_Down.BMP")
            remove_with_restore(target_old_file + "_Front.BMP")
            remove_with_restore(target_old_file + "_Left.BMP")
            remove_with_restore(target_old_file + "_Right.BMP")
        record_deploy(current_folder + "deploy.cfg", "")

        # deploy new
        if parameter == "":
            return True, ""
        sky = 'A'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                 return False, "Illegal parameter range"
            sky = level_dict[parameter[1]]
        elif (parameter[0] == 'sky'):
            sky = parameter[1].upper()
        else:
            return False, "Illegal formation"
        
        if not sky in legal_character_dict:
            return False, "Illegal parameter"

        target_file = game_path + "Textures\\Sky\\Sky_" + sky
        copy_with_backups(target_file + "_Back.BMP", current_folder + "\\back.bmp")
        copy_with_backups(target_file + "_Down.BMP", current_folder + "\\down.bmp")
        copy_with_backups(target_file + "_Front.BMP", current_folder + "\\front.bmp")
        copy_with_backups(target_file + "_Left.BMP", current_folder + "\\left.bmp")
        copy_with_backups(target_file + "_Right.BMP", current_folder + "\\right.bmp")

        record_deploy(current_folder + "deploy.cfg", sky)
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return (bool, string). Bool indicate whether the operation have done. String indicate that some message.
def remove(game_path, current_folder):
    try:
        # restore old
        deploy_cache = read_deploy(current_folder + "deploy.cfg")
        if deploy_cache == "":
            return True, ""
        
        target_file = game_path + "Textures\\Sky\\Sky_" + deploy_cache
        remove_with_restore(target_file + "_Back.BMP")
        remove_with_restore(target_file + "_Down.BMP")
        remove_with_restore(target_file + "_Front.BMP")
        remove_with_restore(target_file + "_Left.BMP")
        remove_with_restore(target_file + "_Right.BMP")
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return string. string is help message. If there are no help message which can be provided, return ""
def help():
    return "Sky deploy help:\n\
    Parameter formation: [ level_LEVEL-INDEX | theme_THEME-INDEX ]\n\
    Parameter example: level_6, theme_D\n\
    \n\
    LEVEL-INDEX's legal value range is from 1 to 15\n\
    THEME-INDEX's legal value range is from A to L\n\
    Using level_ formation to deploy is easy and if you are a professor of Ballance, you can use theme_ deploy method."


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
