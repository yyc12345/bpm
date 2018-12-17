using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareLib {
    public class FilePathBuilder {

        private Stack<string> pathStack;

        public FilePathBuilder(string defaultPath, PlatformID os) {
            pathStack = new Stack<string>();

            if (defaultPath == string.Empty) throw new ArgumentException();

            switch (os) {
                case PlatformID.Win32NT:
                    foreach (var item in defaultPath.Split('\\')) {
                        if (item != string.Empty && item != "") pathStack.Push(item);
                    }
                    break;
                case PlatformID.Unix:
                    foreach (var item in defaultPath.Split('/')) {
                        if (item != string.Empty)
                            pathStack.Push(item);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public FilePathBuilder(Stack<string> generalPath) {
            this.pathStack = generalPath;
        }

        /// <summary>
        /// Backtracking to previous path
        /// </summary>
        public void Backtracking() {
            if (pathStack.Count <= 0) return;
            pathStack.Pop();
        }

        public FilePathBuilder Enter(string name) {
            pathStack.Push(name);
            return this;
        }

        public FilePathBuilder Enter(List<string> name) {
            foreach (var item in name) {
                pathStack.Push(item);
            }
            return this;
        }

        public string Path {
            get {
                return this.PathEx(Information.OS);
            }
        }

        /// <summary>
        /// get the path without slash
        /// </summary>
        public string PathEx(PlatformID os) {
            if (pathStack.Count == 0) return "";
            else {
                //reserve the list because the ground of stack is the top of stack
                List<string> GetPathList() {
                    var cache = pathStack.ToList();
                    cache.Reverse();
                    return cache;
                }

                switch (os) {
                    case PlatformID.Win32NT:
                        if (pathStack.Count == 0) throw new InvalidOperationException("Path stack is empty. It is illegal for Windows.");
                        return string.Join(@"\", GetPathList());
                    case PlatformID.Unix:
                        return "/" + string.Join(@"/", GetPathList());
                    default:
                        return "";
                }
            }

        }

        public Stack<string> GeneralPath() {
            return pathStack;
        }
    }

}
