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
                return new FilePathBuilder(Environment.CurrentDirectory);
            }
        }

        public static OSType OS {
            get {
                switch (Environment.OSVersion.Platform) {
                    case PlatformID.MacOSX:
                        return OSType.macOS;
                    case PlatformID.Unix:
                        return OSType.Unix;
                    case PlatformID.Win32NT:
                    case PlatformID.Win32Windows:
                        return OSType.Windows;
                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                    case PlatformID.Win32S:
                    default:
                        return OSType.None;
                }
            }
        }
        
    }

}
