using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager {
    public static class I18N {

        public static void Init(string name) {
            if (SupportLanguage.Contains(name)) CurrentLanguage = name;
        }

        static string CurrentLanguage = "en-us";

        public static string Core(string indexName, params string[] parameters) {
            return string.Format(GetDictionary()[indexName], parameters);
        }

        static List<string> SupportLanguage = new List<string>() {
            "en-us",
            "zh-cn"
        };

        static Dictionary<string, string> GetDictionary() {
            switch (CurrentLanguage) {
                case "en-us":
                    return Language_en_us;
                case "zh-cn":
                    return Language_zh_cn;
                default:
                    return Language_en_us;
            }
        }

        #region language

        static Dictionary<string, string> Language_en_us = new Dictionary<string, string>() {
            //program.cs
            {"Init_TypePath", "Type a proper path to place Ballance and its package pls. :)"},

            //command.cs
            {"Command_InvalidCommand", "Invalid command"},
            {"Command_InvalidParameterCount", "Invalid parameter count"},

            //*help
            {"Help_1", "Usage: bpm command [option]"},
            {"Help_2", "bpm is a commandline package manager and provides commands for searching and managing as well as querying information about Ballance packages."},
            {"Help_3", "Most used commands:"},
            {"Help_4", "  list - list packages based on package names"},
            {"Help_5", "  search - search in package descriptions"},
            {"Help_6", "  show - show package details"},
            {"Help_7", "  install - install packages"},
            {"Help_8", "  remove - remove packages"},
            {"Help_9", "  update - update list of available packages"},
            {"Help_10", "  deploy - deploy package (especially for map and resources)"},
            {"Help_11", "  config - edit the config"},
            {"Help_12", "Glory to BKT."},
            {"Help_13", "  guide - show specific package's help"},
            {"Help_14", "  help - print this help"},

            //config
            {"Config_InvalidConfig", "Invalid config"},
            {"Config_AppliedNewConfig", "New config has been applied"},

            //install
            {"Install_CollectingPackageInfo", "Collecting pakcage infomation..."},
            {"Install_BuildingDependencyTree", "Building dependency tree..."},
            {"Install_DetectConflict", "Detecting package conflict..."},
            {"Install_SortingDependencyTree", "Sorting dependency tree..."},
            {"Install_RemovingSelectedPackage", "Removing selected packages..."},
            {"Install_InstallingSelectedPackage", "Installing selected packages..."},

            {"Install_InstalledPackage", "Package is installed"},
            {"Install_SelfConflict", "The package, which you want to install, is self-conflict"},
            {"Install_DownloadInfo", "Downloading {0}'s infomation..."},
            {"Install_CloseLoop", "Closed-loop package dependency"},
            {"Install_InstallList", "There are the packages which will be installed: "},
            {"Install_RemoveList", "There are the packages which will be removed due to the conflict: "},
            {"Install_InstallItem", "Installing {0}..."},
            {"Install_ExtractItem", "Extracting {0}..."},
            {"Install_RunScriptItem", "Running {0} script..."},
            {"Install_RecordItem", "Recording {0} info..."},
            {"Install_Success", "{0} is installed successfully"},

            //list
            {"List_Upgradable", "upgradable"},
            {"List_Broken", "broken"},
            {"List_Total", "Total {0} packages."},

            //remove
            {"Remove_RemoveList", "There are the package which will be deleted: "},
            {"Remove_Removing", "Removing {0}..."},
            {"Remove_Success", "Remove {0} successfully"},

            //search & Show
            {"Search&Show_Aka", "aka: "},
            {"Search&Show_Type", "type: "},
            {"Search&Show_Desc", "description: "},
            {"Search_InstalledVersion", "{0} installed version"},
            {"Search_Count", "Total {0} matched packages"},
            {"Show_Dependency", "dependency: "},
            {"Show_Conflict", "conflict: "},
            {"Show_Compatible", "only compatible with: "},
            {"Show_FailJson", "Fail to read JSON file"},

            //update
            {"Update_Success", "Update package list successfully."},
            {"Update_Fail", "Fail to update package list"},
            
            //download.cs
            {"Download_OK", "Download OK" },
            {"Download_ExistedLocalFile", "Detect existed local package cache. Skip downloading" },
            {"Download_LocalFileOperationError", "Couldn't operate local package cache" },
            {"Download_NetworkError", "Network error" },
            {"Download_VerificationError", "Un-matched verification code" },
            {"Download_Timeout", "Network timeout" },
            {"Download_NoPackage", "No matched package" },
            {"Download_RemoteServerError", "Remote server return a error" },
            {"Download_OutdatedVersion", "Outdated bpm version" },
            {"Download_UnexceptError", "Unknow error" },

            //general words
            {"General_SpecificVersion", "You should specific a version of your package"},
            {"General_NoMatchedPackage", "No matched package"},
            {"General_ScriptError", "A error is occured when executing package script. Error detail:"},
            {"General_AllOperationDown", "All operation done!"},
            {"General_None", "None"},
            {"General_Continue", "Do you want to continue (Y/N): "},
            {"General_CancelOperation", "You cancel the operation."},
            {"General_NetworkError", "A network error occured."},
            {"General_OperationAborted", "Operation is aborted"},
            {"General_NoDatabase", "No package database. Please use *bpm update* to update your local package database."},

            //type
            {"PackageType_Mod", "Mod"},
            {"PackageType_Map", "Map"},
            {"PackageType_Sky", "Sky"},
            {"PackageType_Texture", "Texture"},
            {"PackageType_SoundEffect", "SoundEffect"},
            {"PackageType_BGM", "BGM"},
            {"PackageType_App", "App"},
            {"PackageType_Miscellaneous", "Miscellaneous"},

            //script invoker
            {"ScriptInvoker_NoScriptFile", "No script file"},
            {"ScriptInvoker_UnsupportedScript", "Unsupported script file"},
            {"ScriptInvoker_NoMethod", "No matched method"},
            {"ScriptInvoker_InvokeError", "Invoke error"},
            {"ScriptInvoker_CompileError", "Compile error\nDetail:\n{0}"},

            //help
            {"Help_NoHelp", "There are no help provided by package."}
        };

        static Dictionary<string, string> Language_zh_cn = new Dictionary<string, string>() {
            //program.cs
            {"Init_TypePath", "请输入一个合适的路径用于放置Ballance以及包 :)"},

            //command.cs
            {"Command_InvalidCommand", "无效的命令"},
            {"Command_InvalidParameterCount", "无效的参数个数"},

            //*help
            {"Help_1", "用法：bpm 命令 参数"},
            {"Help_2", "bpm 是一个命令行包管理器，并且提供了一些命令用于搜索，管理，又或者是查询与 Ballance 包有关的信息"},
            {"Help_3", "常用命令："},
            {"Help_4", "  list - 基于包的名称列出已安装的包"},
            {"Help_5", "  search - 在包名与包别名中搜索包"},
            {"Help_6", "  show - 展示包的细节"},
            {"Help_7", "  install - 安装包"},
            {"Help_8", "  remove - 移除包"},
            {"Help_9", "  update - 更新可用包列表"},
            {"Help_10", "  deploy - 部署包（尤其对地图以及资源而言）"},
            {"Help_11", "  config - 编辑设置"},
            {"Help_12", "荣耀属于 BKT"},
            {"Help_13", "  guide - 显示指定包的帮助信息"},
            {"Help_14", "  help - 显示这段帮助信息"},

            //config
            {"Config_InvalidConfig", "无效的设置"},
            {"Config_AppliedNewConfig", "新的设置已被应用"},

            //install
            {"Install_CollectingPackageInfo", "收集包信息..."},
            {"Install_BuildingDependencyTree", "构建依赖树..."},
            {"Install_DetectConflict", "检测包冲突..."},
            {"Install_SortingDependencyTree", "排序依赖树..."},
            {"Install_RemovingSelectedPackage", "移除选中的包..."},
            {"Install_InstallingSelectedPackage", "安装选中的包..."},

            {"Install_InstalledPackage", "包已被安装"},
            {"Install_SelfConflict", "您想要安装的包是自我冲突的"},
            {"Install_DownloadInfo", "下载 {0} 的信息..."},
            {"Install_CloseLoop", "包存在闭环依赖"},
            {"Install_InstallList", "这些包将会被安装："},
            {"Install_RemoveList", "这些包将会因为冲突而被移除："},
            {"Install_InstallItem", "安装 {0}..."},
            {"Install_ExtractItem", "解压 {0}..."},
            {"Install_RunScriptItem", "运行 {0} 脚本..."},
            {"Install_RecordItem", "记录 {0} 信息..."},
            {"Install_Success", "{0} 成功安装"},

            //list
            {"List_Upgradable", "可升级的"},
            {"List_Broken", "损坏的"},
            {"List_Total", "共 {0} 个包。"},

            //remove
            {"Remove_RemoveList", "下列包将会被移除："},
            {"Remove_Removing", "移除 {0}..."},
            {"Remove_Success", "成功移除 {0}"},

            //search & Show
            {"Search&Show_Aka", "别名: "},
            {"Search&Show_Type", "类型: "},
            {"Search&Show_Desc", "描述: "},
            {"Search_InstalledVersion", "{0} 个已安装的版本"},
            {"Search_Count", "共 {0} 个匹配的包"},
            {"Show_Dependency", "依赖: "},
            {"Show_Conflict", "冲突: "},
            {"Show_Compatible", "仅兼容: "},
            {"Show_FailJson", "读取 JSON 文件失败"},

            //update
            {"Update_Success", "成功更新包列表"},
            {"Update_Fail", "更新包列表失败"},

            //download.cs
            {"Download_OK", "下载完成" },
            {"Download_ExistedLocalFile", "检测到本地包缓存，跳过下载" },
            {"Download_LocalFileOperationError", "无法操作本地包缓存" },
            {"Download_NetworkError", "网络错误" },
            {"Download_VerificationError", "不匹配的验证码" },
            {"Download_Timeout", "网络超时" },
            {"Download_NoPackage", "远程服务器没有匹配的包" },
            {"Download_RemoteServerError", "远程服务器返回了一个错误" },
            {"Download_OutdatedVersion", "过时的 bpm 版本" },
            {"Download_UnexceptError", "未知错误" },

            //general words
            {"General_SpecificVersion", "您应当为您当前指定的包指定一个特定的版本"},
            {"General_NoMatchedPackage", "没有匹配的包"},
            {"General_ScriptError", "在执行包脚本期间发生了错误。错误详情："},
            {"General_AllOperationDown", "所有操作都已完成！"},
            {"General_None", "无"},
            {"General_Continue", "您想要继续吗？ (Y/N): "},
            {"General_CancelOperation", "您取消了操作"},
            {"General_NetworkError", "发生了网络错误"},
            {"General_OperationAborted", "操作已终止"},
            {"General_NoDatabase", "没有包数据库，请使用 bpm update 更新您的本地的包数据库"},
            
            //type
            {"PackageType_Mod", "模组"},
            {"PackageType_Map", "地图"},
            {"PackageType_Sky", "天空背景"},
            {"PackageType_Texture", "材质"},
            {"PackageType_SoundEffect", "音效"},
            {"PackageType_BGM", "背景音乐"},
            {"PackageType_App", "应用程序"},
            {"PackageType_Miscellaneous", "杂项"},

            //script invoker
            {"ScriptInvoker_NoScriptFile", "没有脚本文件"},
            {"ScriptInvoker_UnsupportedScript", "不支持的脚本文件"},
            {"ScriptInvoker_NoMethod", "没有匹配的函数"},
            {"ScriptInvoker_InvokeError", "调用错误"},
            {"ScriptInvoker_CompileError", "编译错误\n详情：\n{0}"},

            //help
            {"Help_NoHelp", "没有由包提供的帮助"}
        };

        #endregion

    }
}
