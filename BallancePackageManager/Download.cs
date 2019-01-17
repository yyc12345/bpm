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

                return (ns, DownloadResult.OK);
            } catch (Exception) {
                return (null, DownloadResult.NetworkError);
            }
        }
        
        //real func
        public static DownloadResult DownloadDatabase() {
            var res1 = GetLocalFile(RemoteFileType.PackageDatabase, "");
            if (res1.res != DownloadResult.OK) return res1.res;

            var cache = DownloadDatabaseEx(res1.fs);
            if (cache != DownloadResult.OK) {
                res1.fs.Close();
                res1.fs.Dispose();
                File.Delete(res1.url);
            }

            Console.Write("\n");
            return cache;
        }
        static DownloadResult DownloadDatabaseEx(FileStream fs) {

            //var res1 = GetLocalFile(RemoteFileType.PackageDatabase, "");
            //if (res1.res != DownloadResult.OK) return res1.res;

            var remote = GetHostAndPort();
            var res2 = GetClient(remote.host, remote.port);
            if (res2.result != DownloadResult.OK) return res2.result;

            var rnd = new Random();
            var verificationCode = rnd.Next();
            try {
                //send
                /*
                DATA

                Sign code | Transport version | Verification Code | Need resources type
                */
                res2.ns.Write(BitConverter.GetBytes(Transport.SIGN), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(Transport.TRANSPORT_VER), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(verificationCode), 0, 4);
                res2.ns.Write(BitConverter.GetBytes((int)RemoteFileType.PackageDatabase), 0, 4);
                //var data = Encoding.UTF8.GetBytes(remoteFile);
                //ns.Write(BitConverter.GetBytes(data.Length), 0, 4);
                //ns.Write(data, 0, data.Length);

                //receive
                /*
                DATA

                Version is ok? | Verification Code | Package is existed? | Package Count
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
                int packageSize = 1024;

                /*
                ---------------Send:

                REQUEST:

                Segment index (based on 1)(Send 0 to cut wire)

                ---------------Receive:

                PACKAGE

                Package Size | Data
                */
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Write(BitConverter.GetBytes(i + 1), 0, 4);
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
                    consoleProgress.Next();
                }
                res2.ns.Write(BitConverter.GetBytes(0), 0, 4);

            } catch (Exception) {
                return DownloadResult.RemoteServerError;
            }

            fs.Close();
            res2.ns.Close();
            //server close tcp.
            //tcp.Close();
            return DownloadResult.OK;

        }

        public static DownloadResult DownloadPackageInfo(string remoteFile) {
            var res1 = GetLocalFile(RemoteFileType.PackageInfo, remoteFile);
            if (res1.res != DownloadResult.OK) return res1.res;

            var cache = DownloadPackageInfoEx(remoteFile, res1.fs);
            if (cache != DownloadResult.OK) {
                res1.fs.Close();
                res1.fs.Dispose();
                File.Delete(res1.url);
            }

            Console.Write("\n");
            return cache;
        }
        static DownloadResult DownloadPackageInfoEx(string remoteFile, FileStream fs) {

            //var res1 = GetLocalFile(RemoteFileType.PackageInfo, remoteFile);
            //if (res1.res != DownloadResult.OK) return res1.res;

            var remote = GetHostAndPort();
            var res2 = GetClient(remote.host, remote.port);
            if (res2.result != DownloadResult.OK) return res2.result;

            var rnd = new Random();
            var verificationCode = rnd.Next();
            try {
                //send
                /*
                DATA

                Sign code | Transport version | Verification Code | Need resources type | Package name length | Package name
                */
                res2.ns.Write(BitConverter.GetBytes(Transport.SIGN), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(Transport.TRANSPORT_VER), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(verificationCode), 0, 4);
                res2.ns.Write(BitConverter.GetBytes((int)RemoteFileType.PackageInfo), 0, 4);
                var data = Encoding.UTF8.GetBytes(remoteFile);
                res2.ns.Write(BitConverter.GetBytes(data.Length), 0, 4);
                res2.ns.Write(data, 0, data.Length);

                //receive
                /*
                DATA

                Version is ok? | Verification Code | Package is existed? | Package Count
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
                int packageSize = 1024;

                /*
                ---------------Send:

                REQUEST:

                Segment index (based on 1)(Send 0 to cut wire)

                ---------------Receive:

                PACKAGE

                Package Size | Data
                */
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Write(BitConverter.GetBytes(i + 1), 0, 4);
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
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

        public static DownloadResult DownloadPackage(string remoteFile) {
            var res1 = GetLocalFile(RemoteFileType.Package, remoteFile);
            if (res1.res != DownloadResult.OK) return res1.res;

            var cache = DownloadPackageEx(remoteFile, res1.fs);
            if (cache != DownloadResult.OK) {
                res1.fs.Close();
                res1.fs.Dispose();
                File.Delete(res1.url);
            }

            Console.Write("\n");
            return cache;
        }
        static DownloadResult DownloadPackageEx(string remoteFile, FileStream fs) {

            //var res1 = GetLocalFile(RemoteFileType.Package, remoteFile);
            //if (res1.res != DownloadResult.OK) return res1.res;

            var remote = GetHostAndPort();
            var res2 = GetClient(remote.host, remote.port);
            if (res2.result != DownloadResult.OK) return res2.result;

            var rnd = new Random();
            var verificationCode = rnd.Next();
            try {
                //send
                /*
                DATA

                Sign code | Transport version | Verification Code | Need resources type | Package name length | Package name
                */
                res2.ns.Write(BitConverter.GetBytes(Transport.SIGN), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(Transport.TRANSPORT_VER), 0, 4);
                res2.ns.Write(BitConverter.GetBytes(verificationCode), 0, 4);
                res2.ns.Write(BitConverter.GetBytes((int)RemoteFileType.Package), 0, 4);
                var data = Encoding.UTF8.GetBytes(remoteFile);
                res2.ns.Write(BitConverter.GetBytes(data.Length), 0, 4);
                res2.ns.Write(data, 0, data.Length);

                //receive
                /*
                DATA

                Version is ok? | Verification Code | Package is existed? | Package Count | Package

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
                int packageSize = 1024;


                /*
                ---------------Send:

                REQUEST:

                Segment index (based on 1)(Send 0 to cut wire)

                ---------------Receive:

                PACKAGE

                Package Size | Data
                */
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Write(BitConverter.GetBytes(i + 1), 0, 4);
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
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
