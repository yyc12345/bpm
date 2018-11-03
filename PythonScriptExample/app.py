import shutil
# game_path have slash
# but current_folder don't have slash
#
# Return true to report a successful install, otherwise return false
def install(game_path, current_folder):
    shutil.copytree(current_folder, game_path+ "Apps\\YourAppName@YourAppVer")
    return True

# Return true to report a successful config, otherwise return false
def deploy(game_path, current_folder, parameter):
    return True

# Return true to report that package is intact, otherwise return false
def check(game_path, current_folder):
    return True

# Return true to report that package is removed successfully, otherwise return false
def remove(game_path):
    shutil.retree(game_path+ "Apps\\YourAppName@YourAppVer")
    return True
