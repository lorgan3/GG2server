
using GG2server.logic;
using GG2server.logic.data;
using System.Collections.Generic;
using System.Net.Sockets;
namespace GG2server.objects {
    class Player : IEntity {
        private static List<Player> players = new List<Player>();
        private Socket socket;
        
        private string name;
        private Team team;
        private Class player_class;
        private Character character;

        // vars for receiving client commands
        public byte commandReceiveState;
        public ushort commandReceiveExpectedBytes;
        public byte commandReceiveCommand;

        public Player(Socket socket, string name) {
            this.socket = socket;
            this.name = name;

            ServerJoinUpdate();

            var buffer = GG2server.Server.Sendbuffer;
            buffer.Add(Constants.PLAYER_JOIN);
            buffer.Add((byte)name.Length);
            buffer.AddRange(NetworkHelper.GetBytes(name));

            players.Add(this);
        }

        public void Serialize() {
        }

        public void Deserialize() {
        }

        private void ServerJoinUpdate() {
            var buffer = new List<byte>();
            buffer.Add(Constants.JOIN_UPDATE);
            buffer.Add((byte)(players.Count));
            buffer.Add(0);  // Current map area

            buffer.Add(Constants.CHANGE_MAP);
            buffer.Add((byte)GG2server.Server.CurrentMap.Length);
            buffer.AddRange(NetworkHelper.GetBytes(GG2server.Server.CurrentMap));
            buffer.Add((byte)GG2server.Server.MapMD5.Length);
            buffer.AddRange(NetworkHelper.GetBytes(GG2server.Server.MapMD5));

            foreach (Player player in players) {
                buffer.Add(Constants.PLAYER_JOIN);
                buffer.Add((byte)player.name.Length);
                buffer.AddRange(NetworkHelper.GetBytes(player.name));

                // omit class and team for now
            }

            // Skip serialization for now

            socket.Send(buffer.ToArray());
        }

        private void killCharacter() {
            if (character != null) {
                character.Die(Constants.FINISHED_OFF_GIB, null, null);
                character = null;
            }
        }

        public static void doPlayerStep() {
            foreach (Player player in players) {
                // TODO
            }
        }

        ~Player() {
            players.Remove(this);
            socket.Close();
            LogHelper.Log("Player " + name + " has left.", LogLevel.info);
        }

        public Socket Socket {
            get {
                return this.socket;
            }
        }

        public string Name {
            get {
                return this.name;
            }
            set {
                if (name != value) {
                    LogHelper.Log(name + " is now known as " + value, LogLevel.info);
                    name = value.Replace("#", " ");
                    if (name.Length > 20) name = name.Substring(0, 20);
                    
                    var buffer = GG2server.Server.Sendbuffer;
                    buffer.Add(Constants.PLAYER_CHANGENAME);
                    buffer.Add((byte)players.IndexOf(this));
                    buffer.Add((byte)name.Length);
                    buffer.AddRange(NetworkHelper.GetBytes(name));
                }
            }
        }

        public Team Team {
            get {
                return this.team;
            }
            set {
                if (this.team != value) {
                    team = value;
                    killCharacter();

                    var buffer = GG2server.Server.Sendbuffer;
                    buffer.Add(Constants.PLAYER_CHANGETEAM);
                    buffer.Add((byte)players.IndexOf(this));
                    buffer.Add((byte)team);
                }
            }
        }

        public Class Class {
            get {
                return this.player_class;
            }
            set {
                if (this.player_class != value) {
                    player_class = value;
                    killCharacter();

                    var buffer = GG2server.Server.Sendbuffer;
                    buffer.Add(Constants.PLAYER_CHANGECLASS);
                    buffer.Add((byte)players.IndexOf(this));
                    buffer.Add((byte)player_class);
                }
            }
        }

        public Character Character {
            get {
                return this.character;
            }
            set {
                this.character = value;
            }
        }

        public static List<Player> Players {
            get {
                return players;
            }
        }
    }
}
