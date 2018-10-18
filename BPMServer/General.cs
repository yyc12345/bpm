using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPMServer {
    public static class General {

        public static TcpProcessor CoreTcpProcessor;

        public static FileReaderManager CoreFileReader;

        public static List<ManualResetEvent> ManualResetEventList = new List<ManualResetEvent>();

        public static object lockList = new object();

    }
}
