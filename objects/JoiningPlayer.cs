﻿using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace GG2server.objects {
    class JoiningPlayer {
        public const byte STATE_EXPECT_HELLO = 1; // Hello message: 17 bytes (HELLO+UUID)
        public const byte STATE_EXPECT_MESSAGELEN = 2; // 1 byte Message length header 
        public const byte STATE_EXPECT_NAME = 3;
        public const byte STATE_EXPECT_PASSWORD = 4;
        public const byte STATE_CLIENT_AUTHENTICATED = 5;
        public const byte STATE_EXPECT_COMMAND = 6;
        public const byte STATE_CLIENT_DOWNLOADING = 7;

        private Socket socket;
        private byte state;
        private byte messageState;
        private byte expectedBytes;
        private DateTime lastContact;
        //private int cumulativeMapBytes;

        public JoiningPlayer(Socket socket) {
            this.socket = socket;
            this.socket.Blocking = true;
            this.state = STATE_EXPECT_HELLO;
            this.expectedBytes = 17;
            this.lastContact = DateTime.Now;
            //this.cumulativeMapBytes = 0;
        }

        /// <summary>
        /// Initialize a joining player
        /// </summary>
        public void Service() {
            bool error = false;
            byte[] buffer;

            try {
                while (!error) {
                    if ((DateTime.Now - lastContact).TotalSeconds > 30 || socket.Poll(100, SelectMode.SelectError)) {
                        break;
                    }

                    if (socket.Available >= expectedBytes) {
                        lastContact = DateTime.Now;
                        switch (state) {
                            case STATE_EXPECT_HELLO:
                                buffer = socket.Read(expectedBytes);
                                byte hello = buffer[0];
                                buffer = buffer.Where((x, i) => i != 0).ToArray();

                                if (hello == Constants.HELLO && buffer.SequenceEqual(GG2server.protocolUuid)) {
                                    if (GG2server.password == "") {
                                        state = STATE_CLIENT_AUTHENTICATED;
                                        expectedBytes = 0;
                                    } else {
                                        socket.Send(new byte[] { Constants.PASSWORD_REQUEST });
                                        state = STATE_EXPECT_MESSAGELEN;
                                        messageState = STATE_EXPECT_PASSWORD;
                                        expectedBytes = 1;
                                    }
                                } else error = true;
                                break;

                            case STATE_EXPECT_MESSAGELEN:
                                buffer = new byte[expectedBytes];
                                socket.Receive(buffer, expectedBytes, SocketFlags.None);

                                expectedBytes = buffer[0];
                                state = messageState;
                                break;

                            case STATE_EXPECT_PASSWORD:
                                if (expectedBytes <= 0) {
                                    socket.Send(new byte[] { Constants.PASSWORD_WRONG });
                                    error = true;
                                }

                                buffer = new byte[expectedBytes];
                                socket.Receive(buffer, expectedBytes, SocketFlags.None);

                                if (NetworkHelper.GetString(buffer) == GG2server.password) {
                                    state = STATE_CLIENT_AUTHENTICATED;
                                    expectedBytes = 0;
                                } else {
                                    socket.Send(new byte[] { Constants.PASSWORD_WRONG });
                                    error = true;
                                }
                                break;

                            case STATE_CLIENT_AUTHENTICATED:
                                List<byte> b2 = new List<byte>();
                                b2.Add(Constants.HELLO);
                                b2.Add((byte)GG2server.serverName.Length);
                                b2.AddRange(NetworkHelper.GetBytes(GG2server.serverName));
                                b2.Add((byte)GG2server.Server.CurrentMap.Length);
                                b2.AddRange(NetworkHelper.GetBytes(GG2server.Server.CurrentMap));
                                b2.Add((byte)GG2server.Server.MapMD5.Length);
                                b2.AddRange(NetworkHelper.GetBytes(GG2server.Server.MapMD5));

                                // Server sent plugins
                                b2.Add(GG2server.pluginsRequired ? (byte)1 : (byte)0);
                                b2.Add((byte)GG2server.pluginList.Length);
                                b2.AddRange(NetworkHelper.GetBytes(GG2server.pluginList));
                                
                                state = STATE_EXPECT_COMMAND;
                                expectedBytes = 1;
                                socket.Send(b2.ToArray<byte>());
                                break;

                            case STATE_EXPECT_COMMAND:
                                byte command = socket.Read_ubyte();
                                switch (command) {
                                    case Constants.PING:
                                        state = STATE_EXPECT_COMMAND;
                                        expectedBytes = 1;
                                        break;

                                    case Constants.PLAYER_JOIN:
                                        state = STATE_EXPECT_MESSAGELEN;
                                        messageState = STATE_EXPECT_NAME;
                                        expectedBytes = 1;
                                        break;

                                    case Constants.DOWNLOAD_MAP:
                                        // TODO
                                        socket.Send(new byte[] { Constants.INCOMPATIBLE_PROTOCOL });
                                        error = true;
                                        break;
                                }
                                break;

                            case STATE_EXPECT_NAME:
                                if (Player.Players.Count >= GG2server.playerLimit) {
                                    socket.Send(new byte[] { Constants.SERVER_FULL });
                                    error = true;
                                } else {
                                    buffer = socket.Read(expectedBytes);

                                    String name = NetworkHelper.GetString(buffer).Replace('#', ' ');
                                    if (name.Length > 20) name = name.Substring(0, 20);
                                    Player player = new Player(socket, name);
                                    LogHelper.Log("Player " + name + " joined the game", LogLevel.info);
                                    error = true;
                                }
                                break;
                        }
                    }
                    Thread.Sleep(10);
                }
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.warning);
                socket.Close();
            }
        }
    }
}
