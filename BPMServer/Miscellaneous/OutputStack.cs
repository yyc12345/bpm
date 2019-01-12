using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMServer {
    public class OutputStack {

        Queue<string> queue = new Queue<string>();
        object lockQueue = new object();
        bool isStopped = false;

        public void Add(string msg) {
            if (isStopped) {
                lock (lockQueue) {
                    queue.Enqueue(msg);
                }
            } else Console.WriteLine(msg);
        }

        public void Stop() {
            if (isStopped) return;
            isStopped = true;
        }

        public void Release() {
            if (!isStopped) return;
            lock (lockQueue) {
                while (queue.Count != 0) {
                    Console.WriteLine(queue.Dequeue());
                }
                isStopped = false;
            }
        }

    }
}
