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

        static (string host, int port) GetHostAndPort() {
            var config = Config.Read()["Sources"].Split(':');
            return (string.Join(":", config, 0, config.Length - 2), int.Parse(config[config.Length - 1]));
        }

        static (FileStream fs, DownloadResult res, string url) GetLocalFile(RemoteFileType remoteFileType, string remoteFile) {

            string localFile = "";
            switch (remoteFileType) {
                case RemoteFileType.Package:
                    localFile = ConsoleAssistance.WorkPath + @"cache\download\" + remoteFile + ".zip";
                    break;
                case RemoteFileType.PackageInfo:
                    localFile = ConsoleAssistance.WorkPath + @"cache\dependency\" + remoteFile + ".json";
                    break;
                case RemoteFileType.PackageDatabase:
                    localFile = ConsoleAssistance.WorkPath + @"package.db";
                    break;
                default:
                    localFile = ConsoleAssistance.WorkPath + @"package.db";
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

        public static DownloadResult DownloadDatabase() {
            var res1 = GetLocalFile(RemoteFileType.PackageDatabase, "");
            if (res1.res != DownloadResult.OK) return res1.res;

            var cache = DownloadDatabaseEx(res1.fs);
            if (cache != DownloadResult.OK) {
                res1.fs.Close();
                res1.fs.Dispose();
                File.Delete(res1.url);
            }

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
                int packageSize = 1024;
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
                }

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
                int packageSize = 1024;
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
                }

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
                int packageSize = 1024;
                byte[] realData;
                for (int i = 0; i < packageCount; i++) {
                    res2.ns.Read(recData, 0, 4);
                    packageSize = BitConverter.ToInt32(recData, 0);
                    realData = new byte[packageSize];
                    res2.ns.Read(realData, 0, packageSize);

                    fs.Write(realData, 0, packageSize);
                }

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
                    return "Download OK";
                case DownloadResult.ExistedLocalFile:
                    return "Detect existed local package cache. Jump downloading";
                case DownloadResult.LocalFileOperationError:
                    return "Couldn't operate local package cache";
                case DownloadResult.NetworkError:
                    return "Network error";
                case DownloadResult.VerificationError:
                    return "Un-matched verification code";
                case DownloadResult.Timeout:
                    return "Network timeout";
                case DownloadResult.NoPackage:
                    return "No matched package";
                case DownloadResult.RemoteServerError:
                    return "Remote server return a error";
                case DownloadResult.OutdatedVersion:
                    return "Outdated bpm version";
                case DownloadResult.UnexceptError:
                    return "Unknow error";
                default:
                    return "Unknow error";
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
}
