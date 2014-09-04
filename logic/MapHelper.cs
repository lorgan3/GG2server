using System;
using System.IO;
using System.Security.Cryptography;

namespace GG2server.logic {
    class MapHelper {
        public const string KEYWORD = "Gang Garrison 2 Level Data";
        public const string ENTITIES = "{ENTITIES}";
        public const string ENDENTITIES = "{END ENTITIES}";
        public const string  WALKMASK = "{WALKMASK}";
        public const string ENDWALKMASK = "{END WALKMASK}";

        public static string LoadMap(string name) {
            using(FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read)) {
                /*byte[] buffer = NetworkHelper.CopyByteBlock(fs, 0, (int)fs.Length);

                int pos = NetworkHelper.GetString(buffer).IndexOf(KEYWORD);
                int length = (int)NetworkHelper.GetLong(NetworkHelper.CopyByteBlock(buffer, pos - 8, 4)) - KEYWORD.Length - 2;

                buffer = NetworkHelper.CopyByteBlock(buffer, pos + KEYWORD.Length + 2, length);

                MemoryStream output = new MemoryStream();
                using (MemoryStream ms = new MemoryStream(buffer)) {
                    using (ZlibStream zs = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Decompress)) {
                        zs.CopyTo(output);
                    }
                }

                buffer = NetworkHelper.CopyByteBlock(output, 0, (int)output.Length);
                output.Close();*/

                return ComputeMD5(fs);
            }
        }

        private static string ComputeMD5(Stream stream) {
            MD5 md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }
}
