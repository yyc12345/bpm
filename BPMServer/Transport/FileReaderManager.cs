using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BPMServer {
    public class FileReaderManager {

        public FileReaderManager() {
            fileList = new Dictionary<string, FileReaderItem>();
        }

        Dictionary<string, FileReaderItem> fileList;

        object listOperation = new object();
        readonly int FILE_BLOCK_SIZE = 1024;

        public (bool status, int blockCount) AddFile(string url) {
            lock (listOperation) {
                if (fileList.ContainsKey(url)) {
                    fileList[url].UsageCount += 1;
                    return (true, fileList[url].BlockCount);
                } else {
                    if (!File.Exists(url)) return (false, -1);
                    var cache = new FileStream(url, FileMode.Open, FileAccess.Read);
                    var length = (int)cache.Length;
                    fileList.Add(url, new FileReaderItem() { fs = cache, BlockSize = FILE_BLOCK_SIZE, BlockCount = (length % FILE_BLOCK_SIZE == 0 ? length / FILE_BLOCK_SIZE : (length / FILE_BLOCK_SIZE) + 1), LastBlockLength = length % FILE_BLOCK_SIZE, UsageCount = 1 });
                    return (true, fileList[url].BlockCount);
                }
            }
        }

        public void RemoveFile(string url) {
            lock (listOperation) {
                if (fileList.ContainsKey(url)) {
                    fileList[url].UsageCount -= 1;
                    if (fileList[url].UsageCount == 0) {
                        fileList[url].fs.Close();
                        fileList[url].fs.Dispose();
                        fileList.Remove(url);
                    }
                }
            }
        }

        public byte[] Read(string name, int index) {
            lock (listOperation) {
                if (!fileList.ContainsKey(name)) return null;
            }
            lock (fileList[name].lockFS) {
                fileList[name].fs.Seek(FILE_BLOCK_SIZE * (index - 1), SeekOrigin.Begin);
                byte[] data;
                if (index == fileList[name].BlockCount) {
                    data = new byte[fileList[name].LastBlockLength];
                    fileList[name].fs.Read(data, 0, fileList[name].LastBlockLength);
                } else {
                    data = new byte[FILE_BLOCK_SIZE];
                    fileList[name].fs.Read(data, 0, FILE_BLOCK_SIZE);
                }
                return data;
            }
        }

    }

    class FileReaderItem {
        public FileStream fs { get; set; }
        public int BlockSize { get; set; }
        public int BlockCount { get; set; }
        public int LastBlockLength { get; set; }
        public int UsageCount { get; set; }
        public object lockFS = new object();
    }
}
