using GG2server.data;
using GG2server.logic;
using GG2server.logic.data;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GG2server {
    class GG2server {
        public static byte[] protocolUuid;
        public static byte[] gg2lobbyId;
        private static Server server;
        private static Config config;
        private static IEnumerator<string> messages;

        // TODO make getters for these when I'm less lazy.
        public static byte capLimit;
        public static byte respawnTime;
        public static byte playerLimit;
        public static string serverName;
        public static short port;
        public static bool announce;
        public static string password;
        public static string[] maps;

        static void Main(string[] args) {
            config = Config.ConfigFactory();

            // Read the config
            capLimit = (byte)config.ReadConfig("server", "capLimit", (byte)5);
            respawnTime = (byte)config.ReadConfig("server", "respawnTime", (byte)5);
            playerLimit = (byte)config.ReadConfig("server", "playerLimit", (byte)10);
            serverName = (string)config.ReadConfig("server", "serverName", "My c# server");
            port = (short)config.ReadConfig("server", "port", (short)8190);
            announce = (bool)config.ReadConfig("server", "announceToLobby", true);
            password = (string)config.ReadConfig("server", "password","");
            maps = new string[1];
            maps[0] = "ctf_truefort";
            maps = (string[])config.ReadConfig("server", "maps", maps);

            // Read troll messages
            messages = new FileReader("messages.txt").GetEnumerator();
            
            // parse the protocol version UUID for later use
            protocolUuid = NetworkHelper.ParseUuid(Constants.PROTOCOL_UUID);
            gg2lobbyId = NetworkHelper.ParseUuid(Constants.GG2_LOBBY_UUID);

            server = new Server();
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();

            Console.ReadKey(true);
            serverThread.Abort();   // Stop the server
            LogHelper.SaveLogs("logs.txt", false);
            Console.ReadKey(true);
        }

        public static Server Server {
            get {
                return server;
            }
        }

        public static Config Config {
            get {
                return config;
            }
        }

        public static string Message {
            get {
                if (messages == null) {
                    return null;
                }

                if (messages.MoveNext()) return messages.Current;
                messages.Reset();
                messages.MoveNext();
                return messages.Current;
            }
        }
    }
}
