using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ShareLib;

namespace BPMServer {
    public class TcpProcessor {

        public TcpProcessor(int port4, int port6) {
            ThreadPool.SetMaxThreads(1, 10);
            this.Port4 = port4;
            this.Port6 = port6;
        }

        public void Close() {
            StopListen();
            //tdClientListCleaner.Abort();

        }

        #region listen

        int Port4;
        int Port6;

        Socket socket4;
        Socket socket6;

        public void StartListen() {

            socket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            var endPoint4 = new IPEndPoint(IPAddress.Any, Port4);
            var endPoint6 = new IPEndPoint(IPAddress.IPv6Any, Port6);

            socket4.Bind(endPoint4);
            socket6.Bind(endPoint6);

            socket4.Listen(5);
            GetCaller(socket4);
            ConsoleAssistance.WriteLine($"[Network] Listening on port {Port4} for ipv4 connection.");
            socket6.Listen(5);
            GetCaller(socket6);
            ConsoleAssistance.WriteLine($"[Network] Listening on port {Port6} for ipv6 connection.");
        }

        public void StopListen() {
            socket4.Close();
            ConsoleAssistance.WriteLine("[Network] Stop listening ipv4 connection.");
            socket6.Close();
            ConsoleAssistance.WriteLine("[Network] Stop listening ipv6 connection.");
        }

        void GetCaller(Socket s) {
            Task.Run(() => {
                try {
                    Socket client = s.Accept();
                    ManualResetEvent mre = new ManualResetEvent(false);
                    lock (General.lockList) {
                        General.ManualResetEventList.Add(mre);
                    }
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientProcessor), (client, mre));
                } catch (Exception) {
                    //jump
                    return;
                }

                //accept next
                this.GetCaller(s);
            });
        }

        #endregion

        #region socket

        void ClientProcessor(object s) {
            var (client, mre) = ((Socket client, ManualResetEvent mre))s;

            try {
                //check sign
                byte[] data = new byte[4];
                //byte[] send_data;
                client.Receive(data, 0, 4, SocketFlags.None);
                if (BitConverter.ToInt32(data, 0) != Transport.SIGN) goto end; //invalid code
                client.Receive(data, 0, 4, SocketFlags.None);
                if (BitConverter.ToInt32(data, 0) != Transport.TRANSPORT_VER) {
                    //outdated version
                    client.Send(BitConverter.GetBytes(false), 0, 1, SocketFlags.None);
                    goto end;
                } else client.Send(BitConverter.GetBytes(true), 0, 1, SocketFlags.None);

                //verfication code
                //int verificationCode;
                client.Receive(data, 0, 4, SocketFlags.None);
                //verificationCode = BitConverter.ToInt32(data, 0);
                client.Send(data, 0, 4, SocketFlags.None);
                //package type
                RemoteFileType packageType;
                client.Receive(data, 0, 4, SocketFlags.None);
                packageType = (RemoteFileType)BitConverter.ToInt32(data, 0);

                string packageName = "";
                //package name
                if (packageType != RemoteFileType.PackageDatabase) {
                    client.Receive(data, 0, 4, SocketFlags.None);
                    int nameLength = BitConverter.ToInt32(data, 0);
                    data = new byte[nameLength];
                    client.Receive(data, 0, nameLength, SocketFlags.None);
                    packageName = Encoding.UTF8.GetString(data);
                }

                string dataUrl = "";
                switch (packageType) {
                    case RemoteFileType.Package:
                        dataUrl = ConsoleAssistance.WorkPath + @"package\" + packageName + ".zip";
                        break;
                    case RemoteFileType.PackageInfo:
                        dataUrl = ConsoleAssistance.WorkPath + @"dependency\" + packageName + ".json";
                        break;
                    case RemoteFileType.PackageDatabase:
                        dataUrl = ConsoleAssistance.WorkPath + @"package.db";
                        break;
                    default:
                        goto end;
                }

                var res = General.CoreFileReader.AddFile(dataUrl);
                if (!res.status) {
                    client.Send(BitConverter.GetBytes(false), 0, 1, SocketFlags.None);
                    goto end;
                } else client.Send(BitConverter.GetBytes(true), 0, 1, SocketFlags.None);

                client.Send(BitConverter.GetBytes(res.blockCount), 0, 4, SocketFlags.None);

                for (int i = 1; i <= res.blockCount; i++) {
                    var cache = General.CoreFileReader.Read(dataUrl, i);
                    client.Send(BitConverter.GetBytes(cache.Length), 0, 4, SocketFlags.None);
                    client.Send(cache, 0, cache.Length, SocketFlags.None);
                }

                General.CoreFileReader.RemoveFile(dataUrl);

            } catch (Exception) {
                ConsoleAssistance.WriteLine("A error was raised when communicate with client.");
                //pass
            }

            end:
            client.Close();

            //release flag
            mre.Set();
            lock (General.lockList) {
                General.ManualResetEventList.Remove(mre);
            }
        }


        #endregion

    }
}
