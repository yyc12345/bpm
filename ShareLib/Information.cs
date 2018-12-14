using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShareLib {

    public class Information {

        /// <summary>
        /// get the app's work path
        /// </summary>
        public static FilePathBuilder WorkPath {
            get {
                return new FilePathBuilder(Environment.CurrentDirectory, Information.OS);
            }
        }

        public static PlatformID OS {
            get {
                return Environment.OSVersion.Platform;
            }
        }

    }

}
