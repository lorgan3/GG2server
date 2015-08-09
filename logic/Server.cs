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
        public readonly byte[] id;
        private readonly Dictionary<byte, sbyte> commandBytes;

        private string currentMap;
        private string mapMD5;
        private Stopwatch stopwatch;
        private List<byte> sendbuffer;

        public Random random;

        public Server() {
            this.random = new Random();
            this.stopwatch = new Stopwatch();
            sendbuffer = new List<byte>();
            if (GG2server.announce) NetworkHelper.RegisterServerport(GG2server.port);

            currentMap = GG2server.maps[0];

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

        private void RickRoll(Object source, ElapsedEventArgs e) {
            string msg = GG2server.Message;
            if (msg != null) {
                LogHelper.Log(msg, LogLevel.info);
                ServerMessage(msg, sendbuffer);
            }
        }

        /// <summary>
        /// Run the server
        /// </summary>
        public void Run() {
            Socket serverSocket;

            try {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, GG2server.port);
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
                if (GG2server.announce) {
                    // Keep sending a lobby registration
                    sendLobbyRegistration(null, null);
                    System.Timers.Timer timer = new System.Timers.Timer(30000);
                    timer.Elapsed += sendLobbyRegistration;
                    timer.Enabled = true;
                }

                System.Timers.Timer timer2 = new System.Timers.Timer(3000);
                timer2.Elapsed += RickRoll;
                timer2.Enabled = true;

                mapMD5 = "";
                long time = 0;
                int ticks = 0;

                stopwatch.Start();
                while (true) {
                    time = stopwatch.ElapsedMilliseconds;

                    //foreach (Player player in Player.Players) {
                    for(int i = Player.Players.Count-1; i>=0; i--) {
                        Player player = Player.Players[i];
                        Socket socket = player.Socket;

                        // See if the player is still connected
                        if (socket.Poll(1000, SelectMode.SelectError) || (socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0)) {
                            player.Remove();
                            continue;
                        }

                        while (socket.Available > 0) {
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
                                                if (player.Character != null) {
                                                    player.Character.KeyState = socket.Read_ubyte();
                                                    player.Character.AimDirection = socket.Read_ushort();
                                                    player.Character.AimDistance = socket.Read_ubyte();
                                                } else socket.Read(4);
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

                    if (ticks % 2 == 0) {
                        if (ticks % 14 == 0) {
                            ticks = 0;
                              Serialize(UpdateType.quick, sendbuffer);
                        } else Serialize(UpdateType.inputstate, sendbuffer);
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
                    int delay = (int)(stopwatch.ElapsedMilliseconds - time);
                    //if (delay >= 16) LogHelper.Log("Can't keep up! (" + delay + "ms)", LogLevel.warning);
                    Thread.Sleep(Math.Max(0, 16 - delay));
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
            buffer.AddRange(NetworkHelper.GetBytes((short)GG2server.port, false));
            buffer.AddRange(NetworkHelper.GetBytes((short)GG2server.playerLimit, false));
            buffer.AddRange(NetworkHelper.GetBytes((short)Player.Players.Count, false));
            buffer.AddRange(NetworkHelper.GetBytes((short)0, false));  // Number of bots
            if (GG2server.password != "") buffer.AddRange(NetworkHelper.GetBytes((short)1, false));
            else buffer.AddRange(NetworkHelper.GetBytes((short)0, false));

            buffer.AddRange(NetworkHelper.GetBytes((short)7, false));  // Amount of keypairs
            buffer.AddRange(NetworkHelper.KeyValueToBytes("name", GG2server.serverName));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game", Constants.GAME_NAME_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_short", "c\\# gg2"));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_ver", Constants.GAME_VERSION_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("game_url", Constants.GAME_URL_STRING));
            buffer.AddRange(NetworkHelper.KeyValueToBytes("map", currentMap));
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

        public void Serialize(UpdateType type, List<byte> buffer) {
            buffer.Add((byte)type);
            buffer.Add((byte)Player.Players.Count);

            if (type != UpdateType.caps) {
                foreach (Player player in Player.Players) {
                    player.Serialize(type, buffer);
                }
            }

            if (type == UpdateType.full) {
                buffer.AddRange(NetworkHelper.GetBytes((short)0, true));  // Red intel
                buffer.AddRange(NetworkHelper.GetBytes((short)0, true));  // Blue intel

                buffer.Add(GG2server.capLimit);
                buffer.Add(0);  // redcaps
                buffer.Add(0);  // bluecaps
                buffer.Add(GG2server.respawnTime);
                // ONLY WORKS FOR CTF FOR NOW!
                buffer.Add(5);  // Timelimit
                buffer.AddRange(NetworkHelper.GetBytes((uint)300, true));  // Time left

                for (byte i = 0; i < 10; i++) {
                    buffer.Add(255);    // Class limits
                }
            }

            if (type == UpdateType.caps) {
                buffer.Add(0);  // redcaps
                buffer.Add(0);  // bluecaps
                buffer.Add(GG2server.respawnTime);
                // ONLY WORKS FOR CTF FOR NOW!
                buffer.Add(5);  // Timelimit
                buffer.AddRange(NetworkHelper.GetBytes((uint)300, true));  // Time left
            }
        }
        
        /// <summary>
        /// Add a servermessage to the buffer.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="buffer">The buffer to add the message to.</param>
        private void ServerMessage(string msg, List<byte> buffer) {
            if (msg.Length > 255) {
                LogHelper.Log("The message '" + msg + "' is too long, sending the first 255 bytes", LogLevel.info);
                msg = msg.Substring(0, 255);
            }

            buffer.Add(Constants.MESSAGE_STRING);
            buffer.Add((byte)msg.Length);
            buffer.AddRange(NetworkHelper.GetBytes(msg));
        }

        ~Server() {
            if (GG2server.announce) NetworkHelper.UnregServerport(GG2server.port);
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

        public List<byte> Sendbuffer {
            get {
                return this.sendbuffer;
            }
        }
    }
}
