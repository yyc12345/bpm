using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLib;

namespace BPMServer {
    public static class General {

        //general config and status
        public static bool IsMaintaining = false;
        public static OutputStack GeneralOutput = new OutputStack();

        //transport
        public static TcpProcessor CoreTcpProcessor;
        public static FileReaderManager CoreFileReader;
        public static List<ManualResetEvent> ManualResetEventList = new List<ManualResetEvent>();
        public static object lockList = new object();

        //maintain
        public static PackageDatabase GeneralDatabase = new PackageDatabase();

        //verify
        public static byte[] VerifyBytes;

    }
}
