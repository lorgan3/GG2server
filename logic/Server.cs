using GG2server.logic.data;
using GG2server.objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;

namespace GG2server.logic {
    class Server {
        private readonly short PORT;
        public readonly byte[] id;

        private string password;
        private byte playerLimit;
        private string serverName;
        private bool upnp;
        private string mapMD5;
        private Stopwatch timer;

        public Random random;

        public Server(string serverName, byte playerLimit, short port, string password, bool upnp) {
            this.random = new Random();
            this.PORT = port;
            this.password = password;
            this.playerLimit = playerLimit;
            this.serverName = serverName;
            this.timer = new Stopwatch();
            this.upnp = upnp;
            if (upnp) NetworkHelper.RegisterServerport(port);

            id = new byte[16];
            for (byte i = 0; i < 16; i++) {
                id[i] = (byte)(random.Next()%256);
            }
        }
        public Server() : this("My server", 12, 8190, "", true) { }

        /// <summary>
        /// Run the server
        /// </summary>
        public void Run() {
            Socket serverSocket;

            try {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, PORT);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(ep);
                serverSocket.Listen(50);
                serverSocket.Blocking = false;
                LogHelper.Log("Server started", LogLevel.title);
            } catch (Exception ex) {
                LogHelper.Log("Failed to start server: " + ex.ToString(), LogLevel.error);
                return;
            }

            try {
                // Keep sending a lobby registration
                sendLobbyRegistration(null, null);
                System.Timers.Timer timer = new System.Timers.Timer(30000);
                timer.Elapsed += sendLobbyRegistration;
                timer.Enabled = true;

                while (true) {
                    // Basic handling of joining clients.
                    try {
                        Socket clientSocket = serverSocket.Accept();
                        LogHelper.Log("Connection", LogLevel.debug);
                        JoiningPlayer player = new JoiningPlayer(clientSocket);
                        Thread t = new Thread(player.Service);
                        t.Start();
                        t.IsBackground = true;
                        Thread.Sleep(10);
                    } catch (SocketException) { }

                    // Do stuff
                }
            } catch(ThreadAbortException) {
                LogHelper.Log("Server is shutting down", LogLevel.title);
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.error);
            } finally {
                sendLobbyUnreg();
                serverSocket.Close();
            }
        }

        /// <summary>
        /// Tell the lobby server about this server.
        /// </summary>
        private void sendLobbyRegistration(Object source, ElapsedEventArgs e) {
            UdpClient udpClient = new UdpClient(Constants.LOBBY_SERVER_HOST, Constants.LOBBY_SERVER_PORT);
            
            List<byte> buffer = new List<byte>();
            buffer.AddRange(NetworkHelper.ParseUuid("b5dae2e8-424f-9ed0-0fcb-8c21c7ca1352"));
            buffer.AddRange(this.id);
            buffer.AddRange(GG2server.gg2lobbyId);
            buffer.Add((byte)0);  // TCP
            buffer.AddRange(NetworkHelper.GetBytes((short)PORT));
            buffer.AddRange(NetworkHelper.GetBytes((short)this.playerLimit));
            buffer.AddRange(NetworkHelper.GetBytes((short)Player.Length));
            buffer.AddRange(NetworkHelper.GetBytes((short)0));  // Number of bots
            if (password != "") buffer.AddRange(NetworkHelper.GetBytes((short)1));
            else buffer.AddRange(NetworkHelper.GetBytes((short)0));

            buffer.AddRange(NetworkHelper.GetBytes((short)7));  // Amount of keypairs
            buffer.AddRange(NetworkHelper.KeyValueToBytes("name", serverName));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game", Constants.GAME_NAME_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_short", "c\\# gg2"));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_ver", Constants.GAME_VERSION_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_url", Constants.GAME_URL_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("map", "Not implemented lol"));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("protocol_id", GG2server.protocolUuid));
            try {
                udpClient.Send(buffer.ToArray(), buffer.Count);
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.error);
            }
        }

        /// <summary>
        /// Tell the lobby server that this server shut down.
        /// </summary>
        private void sendLobbyUnreg() {
            UdpClient udpClient = new UdpClient(Constants.LOBBY_SERVER_HOST, Constants.LOBBY_SERVER_PORT);

            List<byte> buffer = new List<byte>();
            buffer.AddRange(NetworkHelper.ParseUuid("488984ac-45dc-86e1-9901-98dd1c01c064"));
            buffer.AddRange(this.id);

            try {
                udpClient.Send(buffer.ToArray(), buffer.Count);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Calculate the new md5 and parse the map
        /// </summary>
        /// <param name="map">The filename of the map</param>
        private void GotoMap(string map) {
            MD5 md5 = MD5.Create();
            using (FileStream stream = File.OpenRead(map)) {
                mapMD5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                loadMap(stream);
            }
        }

        /// <summary>
        /// Load the leveldata from a map and parse it.
        /// </summary>
        /// <param name="stream">A filestream that contains the map</param>
        private void loadMap(FileStream stream) {
            string keyword = "Gang Garrison 2 Level Data";
            // TODO
        }

        ~Server() {
            if (upnp) NetworkHelper.UnregServerport(PORT);
        }

        public string Password {
            get {
                return this.password;
            }
            set {
                this.password = value;
            }
        }

        public string ServerName {
            get {
                return this.serverName;
            }
            set {
                this.serverName = value;
            }
        }
    }
}
