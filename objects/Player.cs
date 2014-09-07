
using GG2server.logic;
using GG2server.logic.data;
using System.Collections.Generic;
using System.Net.Sockets;
namespace GG2server.objects {
    public class Player : IEntity {
        private static List<Player> players = new List<Player>();
        private Socket socket;
        
        private string name;
        private Team team = Team.spectator;
        private Class player_class;
        private Character character;
        private Dictionary<byte, byte> stats;

        // vars for receiving client commands
        public byte commandReceiveState;
        public ushort commandReceiveExpectedBytes;
        public byte commandReceiveCommand;

        public Player(Socket socket, string name) {
            this.socket = socket;
            this.name = name;

            stats = new Dictionary<byte, byte>();
            stats.Add(Constants.KILLS, 0);
            stats.Add(Constants.DEATHS, 0);
            stats.Add(Constants.CAPS, 0);
            stats.Add(Constants.ASSISTS, 0);
            stats.Add(Constants.DESTRUCTION, 0);
            stats.Add(Constants.STABS, 0);
            stats.Add(Constants.HEALING, 0);
            stats.Add(Constants.DEFENSES, 0);
            stats.Add(Constants.INVULNS, 0);
            stats.Add(Constants.BONUS, 0);
            stats.Add(Constants.DOMINATIONS, 0);
            stats.Add(Constants.REVENGE, 0);
            stats.Add(Constants.POINTS, 0);

            ServerJoinUpdate();

            var buffer = GG2server.Server.Sendbuffer;
            buffer.Add(Constants.PLAYER_JOIN);
            buffer.Add((byte)name.Length);
            buffer.AddRange(NetworkHelper.GetBytes(name));

            players.Add(this);
        }

        public void Serialize(UpdateType type, List<byte> buffer) {
            if (type == UpdateType.full) {
                buffer.Add(stats[Constants.KILLS]);
                buffer.Add(stats[Constants.DEATHS]);
                buffer.Add(stats[Constants.CAPS]);
                buffer.Add(stats[Constants.ASSISTS]);
                buffer.Add(stats[Constants.DESTRUCTION]);
                buffer.Add(stats[Constants.STABS]);
                buffer.Add(stats[Constants.HEALING]);
                buffer.Add(stats[Constants.DEFENSES]);
                buffer.Add(stats[Constants.INVULNS]);
                buffer.Add(stats[Constants.BONUS]);
                buffer.Add(stats[Constants.KILLS]);
                buffer.Add(0);  // queue jump
                buffer.AddRange(NetworkHelper.GetBytes((short)0));  // rewards
            }

            byte subobjects = 0;
            if (character != null) subobjects &= 1;
            buffer.Add(subobjects);

            if (character != null) character.Serialize(type, buffer);
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

        /// <summary>
        /// Removes a player from the server.
        /// </summary>
        public void Remove() {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;

            killCharacter();
            players.Remove(this);
        }

        public static void doPlayerStep() {
            foreach (Player player in players) {
                // TODO
            }
        }

        ~Player() {
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
