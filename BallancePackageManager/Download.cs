using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ShareLib;
using BallancePackageManager.BPMCore;

namespace BallancePackageManager {

    public static class Download {

        static readonly int READ_INTERVAL = 200;
        static readonly int READ_TIMEOUT = 60 * 1000 / READ_INTERVAL;

        //assistant func
        static (string host, int port) GetHostAndPort() {
            var config = Config.Read()["Sources"].Split(':');
            return (string.Join(":", config, 0, config.Length - 1), int.Parse(config[config.Length - 1]));
        }

        static (FileStream fs, DownloadResult res, string url) GetLocalFile(RemoteFileType remoteFileType, string remoteFile) {

            string localFile = "";
            switch (remoteFileType) {
                case RemoteFileType.Package:
                    localFile = Information.WorkPath.Enter("cache").Enter("download").Enter(remoteFile + ".zip").Path;
                    break;
                case RemoteFileType.PackageInfo:
                    localFile = Information.WorkPath.Enter("cache").Enter("dependency").Enter(remoteFile + ".json").Path;
                    break;
                case RemoteFileType.PackageDatabase:
                    localFile = Information.WorkPath.Enter("package.db").Path;
                    break;
                default:
                    localFile = Information.WorkPath.Enter("package.db").Path;
                    break;
            }

            if (File.Exists(localFile) /*&& remoteFileType != RemoteFileType.Package*/) return (null, DownloadResult.ExistedLocalFile, localFile);

            try {
                var file = new FileStream(localFile, FileMode.Create, FileAccess.Write);
                return (file, DownloadResult.OK, localFile);
            } catch (Exception) {
                return (null, DownloadResult.LocalFileOperationError, localFile);
            }
        }

        static (NetworkStream ns, DownloadResult result) GetClient(string remote, int port) {
            try {
                var tcp = new TcpClient();
                tcp.Connect(remote, port);
                var ns = tcp.GetStream();
                tcp.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                tcp.Client.ReceiveBufferSize = ShareLib.Transport.SOCKET_BUFFER_SIZE;

                return (ns, DownloadResult.OK);
            } catch (Exception) {
                return (null, DownloadResult.NetworkError);
            }
        }

        //real function
        static DownloadResult PreDownload(RemoteFileType remoteFileType, string remoteFile) {
            var res1 = GetLocalFile(remoteFileType, remoteFile);
            if (res1.res != DownloadResult.OK) return res1.res;

            var cache = RealDownload(remoteFileType, remoteFile, res1.fs);
            if (cache != DownloadResult.OK) {
                res1.fs.Close();
                res1.fs.Dispose();
                File.Delete(res1.url);
            }

            Console.Write("\n");
            return cache;
        }
        static DownloadResult RealDownload(RemoteFileType remoteFileType, string remoteFile, FileStream fs) {

            //var res1 = GetLocalFile(RemoteFileType.Package, remoteFile);
            //if (res1.res != DownloadResult.OK) return res1.res;

            var remote = GetHostAndPort();
            var res2 = GetClient(remote.host, remote.port);
            if (res2.result != DownloadResult.OK) return res2.result;

            var rnd = new Random();
            var verificationCode = rnd.Next();
            try {
                //send

                //Package
                /*
                DATA

                Sign code | Transport version | Verification Code | Need resources type | Package name length | Package name
                */

                //PackageDatabase
                /*
                DATA

                Sign code | Transport version | Verification Code | Need resources type
                */
                res2.ns.Write(BitConverter.GetBytes(Transport.SIGN), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(Transport.TRANSPORT_VER), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(verificationCode), 0, 4);
                res2.ns.Write(BitConverter.GetBytes((int)remoteFileType), 0, 4);
                if (remoteFileType != RemoteFileType.PackageDatabase) {
                    var data = Encoding.UTF8.GetBytes(remoteFile);
                    res2.ns.Write(BitConverter.GetBytes(data.Length), 0, 4);
                    res2.ns.Write(data, 0, data.Length);
                }

                //receive
                /*
                DATA

                    PackageDatabase

                    Version is ok? | Verification Code | Package is existed? | Package Count | Sign is existed? | Sign

                    Package

                    Version is ok? | Verification Code | Package is existed? | Package Count

                PACKAGE

                Package Size | Data
                */
                var recData = new byte[1];
                res2.ns.Read(recData, 0, 1);
                if (!BitConverter.ToBoolean(recData, 0)) return DownloadResult.OutdatedVersion;
                recData = new byte[4];
                res2.ns.Read(recData, 0, 4);
                if (BitConverter.ToInt32(recData, 0) != verificationCode) return DownloadResult.VerificationError;
                recData = new byte[1];
                res2.ns.Read(recData, 0, 1);
                if (!BitConverter.ToBoolean(recData, 0)) return DownloadResult.NoPackage;

                recData = new byte[4];
                res2.ns.Read(recData, 0, 4);
                var packageCount = BitConverter.ToInt32(recData, 0);
                var consoleProgress = new DownloadDisplay(packageCount);
                int packageSize = Transport.SEGMENT_LENGTH;

                /*
                ---------------Send:

                REQUEST:

                Segment index (based on 1)(Send 0 to cut wire)

                ---------------Receive:

                PACKAGE

                Package Size | Data
                */
                byte[] netbuffer = new byte[1024];
                int readBytes = 0;
                for (int i = 0; i < packageCount; i++) {
                    readBytes = 0;
                    //order data
                    res2.ns.Write(BitConverter.GetBytes(i + 1), 0, 4);

                    //read length
                    var circleCount = 0;
                    while (true) {
                        if (circleCount >= READ_TIMEOUT) throw new SocketException();
                        if (!res2.ns.DataAvailable) {
                            circleCount++;
                            System.Threading.Thread.Sleep(READ_INTERVAL);
                        } else break;
                    }
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);

                    //read body
                    circleCount = 0;
                    while (readBytes < packageSize) {
                        while (true) {
                            if (circleCount >= READ_TIMEOUT) throw new SocketException();
                            if (!res2.ns.DataAvailable) {
                                circleCount++;
                                System.Threading.Thread.Sleep(READ_INTERVAL);
                            } else break;
                        }
                        var readedSize = res2.ns.Read(netbuffer, 0, 1024);
                        fs.Write(netbuffer, 0, readedSize);
                        readBytes += readedSize;
                    }

                    consoleProgress.Next();
                }
                res2.ns.Write(BitConverter.GetBytes(0), 0, 4);

            } catch (Exception) {
                return DownloadResult.RemoteServerError;
            }

            fs.Close();
            res2.ns.Close();
            return DownloadResult.OK;

        }

        //invoke func
        public static DownloadResult DownloadDatabase() => PreDownload(RemoteFileType.PackageDatabase, "");
        public static DownloadResult DownloadPackageInfo(string remoteFile) => PreDownload(RemoteFileType.PackageInfo, remoteFile);
        public static DownloadResult DownloadPackage(string remoteFile) => PreDownload(RemoteFileType.Package, remoteFile);

        public static string JudgeDownloadResult(DownloadResult res) {
            switch (res) {
                case DownloadResult.OK:
                //return "Download OK";
                case DownloadResult.ExistedLocalFile:
                //return "Detect existed local package cache. Jump downloading";
                case DownloadResult.LocalFileOperationError:
                //return "Couldn't operate local package cache";
                case DownloadResult.NetworkError:
                //return "Network error";
                case DownloadResult.VerificationError:
                //return "Un-matched verification code";
                case DownloadResult.Timeout:
                //return "Network timeout";
                case DownloadResult.NoPackage:
                //return "No matched package";
                case DownloadResult.RemoteServerError:
                //return "Remote server return a error";
                case DownloadResult.OutdatedVersion:
                //return "Outdated bpm version";
                case DownloadResult.UnexceptError:
                    //return "Unknow error";
                    return I18N.Core($"Download_{res.ToString()}");
                default:
                    return I18N.Core("Download_UnexceptError");
            }
        }

        public enum DownloadResult {
            OK,
            ExistedLocalFile,
            LocalFileOperationError,
            NetworkError,
            VerificationError,
            Timeout,
            NoPackage,
            RemoteServerError,
            OutdatedVersion,
            UnexceptError
        }
    }

    public class DownloadDisplay {

        public DownloadDisplay(int packageCount) {
            count = packageCount;
            Next();
        }

        int count = 0;
        int current = -1;

        string beforeWords = "";

        public void Next() {
            current++;
            var duration = (int)(((double)current / (double)count) * 100);
            var realDuration = duration / 2;
            string str = "";
            for (int q = 0; q < beforeWords.Count(); q++) {
                str += "\b";
            }
            str += "[";
            for (int i = 0; i < realDuration; i++) {
                str += "#";
            }
            for (int j = 0; j < (50 - realDuration); j++) {
                str += "=";
            }
            str += $"] {duration}%";

            Console.Write(str);
            beforeWords = str;
        }
    }

}
