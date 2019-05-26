using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace ShareLib {
    //Origin: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider.signdata?view=netstandard-2.0
    public static class SignVerifyHelper {

        public static void GenerateKey(string pubPath, string priPath) {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(2048/*15360*/);
            var pubFs = new StreamWriter(pubPath, false, Encoding.UTF8);
            pubFs.Write(RSAalg.ToJsonString(false));
            pubFs.Close();
            pubFs.Dispose();

            var priFs = new StreamWriter(priPath, false, Encoding.UTF8);
            priFs.Write(RSAalg.ToJsonString(true));
            priFs.Close();
            priFs.Dispose();
        }

        public static byte[] SignData(string file, string pubKey) {
            //read key
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            var pubFs = new StreamReader(pubKey, Encoding.UTF8);
            RSAalg.FromJsonString(pubFs.ReadToEnd());
            pubFs.Close();
            pubFs.Dispose();

            var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
            var res = RSAalg.SignData(fs, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            fs.Close();
            fs.Dispose();
            return res;
        }

        public static bool VerifyData(byte[] verifiedByte, string file, string priKey) {
            //read key
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            var priFs = new StreamReader(priKey, Encoding.UTF8);
            RSAalg.FromJsonString(priFs.ReadToEnd());
            priFs.Close();
            priFs.Dispose();

            var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
            var res = RSAalg.VerifyData(fs, verifiedByte, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            fs.Close();
            fs.Dispose();
            return res;
        }

        //Origin: https://stackoverflow.com/questions/13569406/how-should-i-compute-files-hashmd5-sha1-in-c-sharp
        public static string GetFileHash(string file) {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None)) {
                SHA256Managed sha = new SHA256Managed();
                return Convert.ToBase64String(sha.ComputeHash(stream));
            }
        }
    }

    //Origin: https://github.com/dotnet/corefx/issues/23686
    //Due to .net core 2.0 don't support this function. I use the alternative.
    public static class RSAKeyExtensions {

        public static void FromJsonString(this RSA rsa, string jsonString) {
            //Check.Argument.IsNotEmpty(jsonString, nameof(jsonString));
            try {
                var paramsJson = JsonConvert.DeserializeObject<RSAParametersJson>(jsonString);

                RSAParameters parameters = new RSAParameters();

                parameters.Modulus = paramsJson.Modulus != null ? Convert.FromBase64String(paramsJson.Modulus) : null;
                parameters.Exponent = paramsJson.Exponent != null ? Convert.FromBase64String(paramsJson.Exponent) : null;
                parameters.P = paramsJson.P != null ? Convert.FromBase64String(paramsJson.P) : null;
                parameters.Q = paramsJson.Q != null ? Convert.FromBase64String(paramsJson.Q) : null;
                parameters.DP = paramsJson.DP != null ? Convert.FromBase64String(paramsJson.DP) : null;
                parameters.DQ = paramsJson.DQ != null ? Convert.FromBase64String(paramsJson.DQ) : null;
                parameters.InverseQ = paramsJson.InverseQ != null ? Convert.FromBase64String(paramsJson.InverseQ) : null;
                parameters.D = paramsJson.D != null ? Convert.FromBase64String(paramsJson.D) : null;
                rsa.ImportParameters(parameters);
            } catch {
                throw new Exception("Invalid JSON RSA key.");
            }
        }

        public static string ToJsonString(this RSA rsa, bool includePrivateParameters) {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            var parasJson = new RSAParametersJson() {
                Modulus = parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                Exponent = parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                P = parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                Q = parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                DP = parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                DQ = parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                InverseQ = parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                D = parameters.D != null ? Convert.ToBase64String(parameters.D) : null
            };

            return JsonConvert.SerializeObject(parasJson);
        }

    }

    public class RSAParametersJson {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public string DP { get; set; }
        public string DQ { get; set; }
        public string InverseQ { get; set; }
        public string D { get; set; }
    }

}
