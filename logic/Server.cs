using GG2server.logic.data;
using GG2server.objects;
using System;
using System.Collections;
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
        private readonly Dictionary<byte, sbyte> commandBytes;

        private string password;
        private byte playerLimit;
        private string serverName;
        private string currentMap;
        private bool upnp;
        private string mapMD5;
        private Stopwatch stopwatch;
        private List<byte> sendbuffer;

        public Random random;

        public Server(string serverName, byte playerLimit, short port, string password, bool upnp) {
            this.random = new Random();
            this.PORT = port;
            this.password = password;
            this.playerLimit = playerLimit;
            this.serverName = serverName;
            this.stopwatch = new Stopwatch();
            sendbuffer = new List<byte>();
            this.upnp = upnp;
            if (upnp) NetworkHelper.RegisterServerport(port);

            currentMap = "ctf_truefort";

            id = new byte[16];
            for (byte i = 0; i < 16; i++) {
                id[i] = (byte)(random.Next()%256);
            }

            commandBytes = new Dictionary<byte, sbyte>();
            commandBytes.Add(Constants.PLAYER_LEAVE, 0);
            commandBytes.Add(Constants.PLAYER_CHANGECLASS, 1);
            commandBytes.Add(Constants.PLAYER_CHANGETEAM, 1);
            commandBytes.Add(Constants.CHAT_BUBBLE, 1);
            commandBytes.Add(Constants.BUILD_SENTRY, 0);
            commandBytes.Add(Constants.DESTROY_SENTRY, 0);
            commandBytes.Add(Constants.DROP_INTEL, 0);
            commandBytes.Add(Constants.OMNOMNOMNOM, 0);
            commandBytes.Add(Constants.TOGGLE_ZOOM, 0);
            commandBytes.Add(Constants.PLAYER_CHANGENAME, Constants.commandBytesPrefixLength1);
            commandBytes.Add(Constants.INPUTSTATE, 4);
            commandBytes.Add(Constants.REWARD_REQUEST, Constants.commandBytesPrefixLength1);
            commandBytes.Add(Constants.REWARD_CHALLENGE_RESPONSE, 16);
            commandBytes.Add(Constants.PLUGIN_PACKET, Constants.commandBytesPrefixLength2);
            commandBytes.Add(Constants.CLIENT_SETTINGS, 1);

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

                mapMD5 = "";
                long time = 0;
                int ticks = 0;

                stopwatch.Start();
                while (true) {
                    time = stopwatch.ElapsedMilliseconds;

                    foreach (Player player in Player.Players) {
                        Socket socket = player.Socket;
                        if (socket.Poll(100, SelectMode.SelectError)) {
                            Player.Players.Remove(player);
                            continue;
                        }

                        if (socket.Available > 0) {
                            LogHelper.Log(socket.Available + " bytes available", LogLevel.debug);
                            switch (player.commandReceiveState) {
                                case 0:
                                    player.commandReceiveCommand = socket.Read_ubyte();
                                        try {
                                            switch(commandBytes[player.commandReceiveCommand]) {
                                                case Constants.commandBytesPrefixLength1:
                                                    player.commandReceiveState = 1;
                                                    player.commandReceiveExpectedBytes = 1;
                                                    break;

                                                case Constants.commandBytesPrefixLength2:
                                                    player.commandReceiveState = 3;
                                                    player.commandReceiveExpectedBytes = 2;
                                                    break;

                                                default:
                                                    player.commandReceiveState = 2;
                                                    player.commandReceiveExpectedBytes = (byte)commandBytes[player.commandReceiveCommand];
                                                    break;
                                            }
                                        } catch (KeyNotFoundException) { }
                                        break;

                                    case 1:
                                        player.commandReceiveState = 2;
                                        player.commandReceiveExpectedBytes = socket.Read_ubyte();
                                        break;

                                    case 3:
                                        player.commandReceiveState = 2;
                                        player.commandReceiveExpectedBytes = socket.Read_ushort();
                                        break;

                                    case 2:
                                        switch (player.commandReceiveCommand) {
                                            case Constants.PLAYER_LEAVE:
                                                Player.Players.Remove(player);
                                                break;
                                            case Constants.PLAYER_CHANGECLASS:
                                                player.Class = (Class)socket.Read_ubyte();
                                                break;
                                            case Constants.PLAYER_CHANGETEAM:
                                                player.Team = (Team)socket.Read_ubyte();
                                                break;
                                            case Constants.CHAT_BUBBLE:
                                                byte bubble = socket.Read_ubyte();
                                                // TODO
                                                break;
                                            case Constants.BUILD_SENTRY:
                                                // TODO
                                                break;
                                            case Constants.DESTROY_SENTRY:
                                                // TODO
                                                break;
                                            case Constants.DROP_INTEL:
                                                // TODO
                                                break;
                                            case Constants.OMNOMNOMNOM:
                                                // TODO
                                                break;
                                            case Constants.TOGGLE_ZOOM:
                                                // TODO
                                                break;
                                            case Constants.PLAYER_CHANGENAME:
                                                player.Name = NetworkHelper.GetString(socket.Read(player.commandReceiveExpectedBytes));
                                                break;
                                            case Constants.INPUTSTATE:
                                                socket.Read(4);
                                                // TODO
                                                break;
                                            case Constants.REWARD_REQUEST:
                                                socket.Read(socket.Read_ubyte());
                                                // TODO
                                                break;
                                            case Constants.REWARD_CHALLENGE_RESPONSE:
                                                socket.Read(16);
                                                // TODO
                                                break;
                                            case Constants.PLUGIN_PACKET:
                                                LogHelper.Log("Received a pluginpacket.", LogLevel.warning);
                                                break;
                                            case Constants.CLIENT_SETTINGS:
                                                socket.Read_ubyte();
                                                LogHelper.Log("Received client settings.", LogLevel.debug);
                                                // TODO
                                                break;
                                        }
                                        player.commandReceiveState = 0;
                                        player.commandReceiveExpectedBytes = 1;
                                        break;
                                }
                            }
                        }

                    // Do stuff
                    Player.doPlayerStep();

                    if (ticks % 7 == 0) {
                        ticks = 0;
                    }

                    // Send data
                    if (sendbuffer.Count > 0) {
                        byte[] buffer = sendbuffer.ToArray();
                        foreach (Player player in Player.Players) {
                            player.Socket.Send(buffer);
                        }
                        sendbuffer.Clear();
                    }

                    // Handle joining clients.
                    try {
                        Socket clientSocket = serverSocket.Accept();
                        LogHelper.Log("Connection", LogLevel.debug);
                        JoiningPlayer player = new JoiningPlayer(clientSocket);
                        Thread t = new Thread(player.Service);
                        t.Start();
                        t.IsBackground = true;
                    } catch (SocketException) { }

                    ticks++;
                    Thread.Sleep(Math.Max(0, 1000 / 60 - (int)(stopwatch.ElapsedMilliseconds - time)));
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
            buffer.AddRange(NetworkHelper.GetBytes((short)Player.Players.Count));
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
                //udpClient.Send(buffer.ToArray(), buffer.Count);
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

        ~Server() {
            if (upnp) NetworkHelper.UnregServerport(PORT);
        }

        public string Password {
            get {
                return this.password;
            }
        }

        public string ServerName {
            get {
                return this.serverName;
            }
        }

        public string CurrentMap {
            get {
                return this.currentMap;
            }
        }

        public string MapMD5 {
            get {
                return this.mapMD5;
            }
        }

        public int PlayerLimit {
            get {
                return this.playerLimit;
            }
        }

        public List<byte> Sendbuffer {
            get {
                return this.sendbuffer;
            }
        }
    }
}
