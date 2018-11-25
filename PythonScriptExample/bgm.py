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
            target_file_1 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_1.wav"
            target_file_2 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_2.wav"
            target_file_3 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_3.wav"
            remove_with_restore(target_file_1)
            remove_with_restore(target_file_2)
            remove_with_restore(target_file_3)
        record_deploy(current_folder + "deploy.cfg", "")

        # deploy new
        if parameter == "":
            return True, ""
        theme = '1'
        parameter.split('_')
        if (parameter[0] == 'level'):
            level = int(parameter[1])
            if not (level >= 1 and level <= 15):
                return False, "Illegal parameter range"
            theme = level_dict[parameter[1]]
        elif (parameter[0] == 'theme'):
            theme = parameter[1]
        else:
            return False, "Illegal formation"

        if not theme in legal_character_dict:
            return False, "Illegal parameter"
        
        target_file_1 = game_path + "Sounds\\Music_Theme_" + theme + "_1.wav"
        target_file_2 = game_path + "Sounds\\Music_Theme_" + theme + "_2.wav"
        target_file_3 = game_path + "Sounds\\Music_Theme_" + theme + "_3.wav"
        copy_with_backups(target_file_1, current_folder + "\\1.wav")
        copy_with_backups(target_file_2, current_folder + "\\2.wav")
        copy_with_backups(target_file_3, current_folder + "\\3.wav")

        record_deploy(current_folder + "deploy.cfg", theme)
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
        
        target_file_1 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_1.wav"
        target_file_2 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_2.wav"
        target_file_3 = game_path + "Sounds\\Music_Theme_" + deploy_cache + "_3.wav"
        remove_with_restore(target_file_1)
        remove_with_restore(target_file_2)
        remove_with_restore(target_file_3)
    except Exception as error:
        return False, ("Runtime error:\n" + error)
    return True, ""

# Return string. string is help message. If there are no help message which can be provided, return ""
def help():
    return "BGM deploy help:\n\
    Parameter formation: [ level_LEVEL-INDEX | theme_THEME-INDEX ]\n\
    Parameter example: level_5, theme_3\n\
    \n\
    LEVEL-INDEX's legal value range is from 1 to 15\n\
    THEME-INDEX's legal value range is from 1 to 5\n\
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
