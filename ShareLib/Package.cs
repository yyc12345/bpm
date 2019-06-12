using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib {
    //public class Package {
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string GUID { get; set; }
    //    public string Author { get; set; }
    //    public long ReleaseTime { get; set; }
    //    public int Version { get; set; }
    //    public List<string> Dependency { get; set; }
    //    public List<string> Conflict { get; set; }
    //    public PackageType Type { get; set; }
    //}

    public enum PackageType : int {
        Mod,
        Map,
        Sky,
        Texture,
        SoundEffect,
        BGM,
        App,
        Miscellaneous
    }

    [Flags]
    public enum OSType : int {
        None = 0b000,
        Windows = 0b001,
        Unix = 0b010,
        macOS = 0b100
    }

    public enum RemoteFileType : int {
        Package,
        PackageDatabase
    }

    public static class Transport {
        public static readonly int SIGN = 61;
        public static readonly int TRANSPORT_VER = 2;
        public static readonly int SEGMENT_LENGTH = 1024 * 512;
        public static readonly int SOCKET_BUFFER_SIZE = 1024 * 1024;
        public static readonly int SIGNATURE_LENGTH = 256; //change this when change HASH method
    }

    public static class DateTimeHelper {
        public static long ToUNIXTimestamp(this DateTime t) {
            return (t.Ticks - 621355968000000000) / 10000000;
        }

        public static DateTime ToDateTime(this long unixTimestamp) {
            return new DateTime(unixTimestamp * 10000000 + 621355968000000000);
        }
    }


}
