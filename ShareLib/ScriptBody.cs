using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public static class Plugin {
    {PersonalCode}
}

public static class CommandHelper {

    public class FilePathBuilder {

        public string Path { get; private set; } = "";

        public FilePathBuilder(string path) {
            Path = path;
        }

        public FilePathBuilder Backtracking() {
            Path = System.IO.Path.GetDirectoryName(Path);
            return this;
        }

        public FilePathBuilder Enter(string name) {
            Path = System.IO.Path.Combine(Path, name);
            return this;
        }

    }

}