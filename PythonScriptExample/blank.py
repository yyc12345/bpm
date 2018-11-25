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
    try:
        pass
    except Exception as error:
        return False, ("Runtime error:\n" + error)
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