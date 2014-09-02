using GG2server.data;
using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Threading;

namespace GG2server {
    class GG2server {
        public static byte[] protocolUuid;
        public static byte[] gg2lobbyId;
        private static Server server;

        static void Main(string[] args) {
            Config c = Config.ConfigFactory();

            // parse the protocol version UUID for later use
            protocolUuid = NetworkHelper.ParseUuid(Constants.PROTOCOL_UUID);
            gg2lobbyId = NetworkHelper.ParseUuid(Constants.GG2_LOBBY_UUID);

            server = new Server("Test server (don't join)", 10, 8199, "test", false);
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();

            Console.ReadKey(true);
            serverThread.Abort();   // Stop the server
            LogHelper.SaveLogs("logs.txt", false);
        }

        public static Server Server {
            get {
                return server;
            }
        }
    }
}
