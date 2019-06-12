using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ShareLib;
using System.Diagnostics;

namespace BPMServer {
    public class RecordFile {

        public RecordFile(bool isEnable) {
            _isEnable = isEnable;
            fs = new StreamWriter(Information.WorkPath.Enter("logs").Enter($"{DateTime.Now.Ticks}.log").Path, true, Encoding.UTF8);
        }

        bool _isEnable;
        StreamWriter fs = null;
        object fileLock = new object();

        public void WriteNewRecord(string value) {
            lock (fileLock) {
                if (fs is null) return;
                if (!_isEnable) return;

                fs.WriteLine(value);
#if DEBUG
                Debug.WriteLine(value);
#endif
            }
        }

        public void Close() {
            lock (fileLock) {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

    }
}
