using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallancePackageManager {

    public static class ConsoleAssistance {

        public static string WorkPath {
            get {
                var res = Environment.CurrentDirectory;
                res += res[res.Length - 1] == '\\' ? "" : "\\";
                return res;
            }
        }

        public static (string packageName, string version, string suffix) GetScriptInfo(string fileName) {
            var name = Path.GetFileNameWithoutExtension(fileName);

            if (name.Contains("@")) {
                var cache = name.Split('@');
                return (cache[0], cache[1], Path.GetExtension(fileName));
            } else return (name, "", Path.GetExtension(fileName));
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files) {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// the update of console.writeline()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="previousForeground"></param>
        /// <param name="previousBackground"></param>
        public static void WriteLine(string str, ConsoleColor foreground = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black) {

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;

            Console.WriteLine(str);

            //restore
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// the update of console.write()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="previousForeground"></param>
        /// <param name="previousBackground"></param>
        public static void Write(string str, ConsoleColor foreground = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black) {

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;

            Console.Write(str);

            //restore
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


    }

}
