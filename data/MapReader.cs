using GG2server.data;
using GG2server.logic.data;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace GG2server.logic {
    class MapReader {
        public const string KEYWORD = "Gang Garrison 2 Level Data";
        public const string ENTITIES = "{ENTITIES}";
        public const string ENDENTITIES = "{END ENTITIES}";
        public const string  WALKMASK = "{WALKMASK}";
        public const string ENDWALKMASK = "{END WALKMASK}";

        public static Map LoadMap(string name) {
            if (File.Exists(name + ".bin")) {
                LogHelper.Log(String.Format("Found {0}.bin, loading data...", name), LogLevel.debug);
                return Deserialize(name);
            } else if (File.Exists(name + ".png")) {
                LogHelper.Log(String.Format("Found {0}.map, generating data...", name), LogLevel.debug);
                Map map = Generate(name);
                Serialize(name, map);
                return map;
            } else {
                throw new MapException(String.Format("Could not find {0}.png or {0}.bin, aborting", name));
            }
        }

        private static Map Generate(string name) {
            string md5;
            byte[] buffer;
            using (FileStream fs = new FileStream(name + ".png", FileMode.Open, FileAccess.Read)) {
                md5 = ComputeMD5(fs);

                buffer = NetworkHelper.CopyByteBlock(fs, 0, (int)fs.Length);

                int pos = NetworkHelper.GetString(buffer).IndexOf(KEYWORD);
                int length = (int)NetworkHelper.GetLong(NetworkHelper.CopyByteBlock(buffer, pos - 8, 4), false) - KEYWORD.Length - 2;

                buffer = NetworkHelper.CopyByteBlock(buffer, pos + KEYWORD.Length + 2, length);

                MemoryStream output = new MemoryStream();
                using (MemoryStream ms = new MemoryStream(buffer)) {
                    using (ZlibStream zs = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Decompress)) {
                        zs.CopyTo(output);
                    }
                }

                buffer = NetworkHelper.CopyByteBlock(output, 0, (int)output.Length);
                output.Close();
            }

            string result = NetworkHelper.GetString(buffer);
            int start = result.IndexOf(ENTITIES) + ENTITIES.Length;
            string leveldata = result.Substring(start, result.IndexOf(ENDENTITIES) - start);
            start = result.IndexOf(WALKMASK) + WALKMASK.Length;
            string[] walkmask = result.Substring(start + 1, result.IndexOf(ENDWALKMASK) - start - 1).Split((char)10);

            return new Map(md5, GaygonParser.Decode(leveldata), Int32.Parse(walkmask[0]), Int32.Parse(walkmask[1]), walkmask[2]);
        }

        private static Dictionary<string, object> parseEntities(string leveldata) {
            return GaygonParser.Decode(leveldata);
        }

        private static Map Deserialize(string name) {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".bin", FileMode.Open, FileAccess.Read)) {
                return (Map)bf.Deserialize(fs);
            }
        }

        private static void Serialize(string name, Map map) {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".bin", FileMode.Create, FileAccess.Write)) {
                bf.Serialize(fs, map);
            }
        }

        private static string ComputeMD5(Stream stream) {
            MD5 md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }
}
