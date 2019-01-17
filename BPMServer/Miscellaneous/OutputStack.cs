using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMServer {
    public class OutputStack {

        Queue<string> queue = new Queue<string>();
        object globalLock = new object();
        bool isStopped = false;

        public void Add(string msg) {
            lock (globalLock) {
                if (isStopped) queue.Enqueue(msg);
                else Console.WriteLine(msg);
            }
        }

        public void Stop() {
            lock (globalLock) {
                if (isStopped) return;
                isStopped = true;
            }
        }

        public void Release() {
            lock (globalLock) {
                if (!isStopped) return;
                while (queue.Count != 0) {
                    Console.WriteLine(queue.Dequeue());
                }
                isStopped = false;
            }
        }

    }
}
