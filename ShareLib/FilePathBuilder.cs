using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShareLib {
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
