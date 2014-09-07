using GG2server.logic;
using GG2server.logic.data;
using GG2server.objects.weapons;
//using Physics2DDotNet;
//using Physics2DDotNet.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GG2server.objects {
    public class Character : IEntity {
        public float x;
        public float y;
        private float hp;
        private float maxHP;
        private Player player;
        private Weapon weapon;

        public float vspeed;
        public float hspeed;

        private float runPower;
        private byte tauntLength;

        public Character(Player player) {
            this.player = player;

            switch (player.Class) {
                case Class.scout:
                    runPower = 1.4f;
                    maxHP = 100;
                    tauntLength = 9;
                    weapon = new Scattergun(this);
                    break;
                case Class.soldier:
                    runPower = 0.9f;
                    maxHP = 160;
                    tauntLength = 11;
                    weapon = new Rocketlauncher(this);
                    break;
                case Class.sniper:
                    runPower = 0.9f;
                    maxHP = 120;
                    tauntLength = 13;
                    weapon = new Rifle(this);
                    break;
                case Class.demoman:
                    runPower = 1f;
                    maxHP = 140;
                    tauntLength = 11;
                    weapon = new Minegun(this);
                    break;
                case Class.medic:
                    runPower = 1.09f;
                    maxHP = 120;
                    tauntLength = 11;
                    weapon = new Medigun(this);
                    break;
                case Class.engineer:
                    runPower = 1f;
                    maxHP = 120;
                    tauntLength = 13;
                    weapon = new Shotgun(this);
                    break;
                case Class.heavy:
                    runPower = 0.8f;
                    maxHP = 200;
                    tauntLength = 12;
                    weapon = new Minigun(this);
                    break;
                case Class.spy:
                    runPower = 1.08f;
                    maxHP = 100;
                    tauntLength = 11;
                    weapon = new Revolver(this);
                    break;
                case Class.pyro:
                    runPower = 1.1f;
                    maxHP = 120;
                    tauntLength = 10;
                    weapon = new Flamethrower(this);
                    break;
                case Class.quote:
                    runPower = 1.07f;
                    maxHP = 140;
                    tauntLength = 17;
                    weapon = new Blade(this);
                    break;
            }

            hp = maxHP;
        }

        public void Die(int damageSource, Player damageDealer, Player assistant) {
            var buffer = GG2server.Server.Sendbuffer;
            buffer.Add(Constants.PLAYER_DEATH);
            buffer.Add((byte)Player.Players.IndexOf(player));
            buffer.Add((byte)(damageDealer == null ? 255 : Player.Players.IndexOf(damageDealer)));
            buffer.Add((byte)(assistant == null ? 255 : Player.Players.IndexOf(assistant)));
            buffer.Add((byte)damageSource);

            player.Character = null;
            this.player = null;
        }
        
        public void Serialize(UpdateType type, List<byte> buffer) {
            buffer.Add(0);  // Keystate
            buffer.AddRange(NetworkHelper.GetBytes((short)0));  // Aimdirection
            buffer.Add(0);  // Aimdistance

            if (type == UpdateType.quick || type == UpdateType.full) {
                buffer.AddRange(NetworkHelper.GetBytes((ushort)x*5));
                buffer.AddRange(NetworkHelper.GetBytes((ushort)y*5));
                buffer.Add((byte)(vspeed * 8.5));
                buffer.Add((byte)(hspeed * 8.5));
                buffer.Add((byte)Math.Ceiling(hp));
                buffer.Add(0);  // Ammocount
                buffer.Add(0);  // cloak & movestate
            }

            if (type == UpdateType.full) {
                buffer.Add(0);  // AnimationOffset
                switch (player.Class) {
                    case Class.spy:
                        buffer.Add(1 * 255);    // Cloak
                        break;
                    case Class.medic:
                        buffer.Add(0 * 255 / 2000); // Uber
                        break;
                    case Class.engineer:
                        buffer.Add(100);    // Nuts & bolts
                        break;
                    case Class.sniper:
                        buffer.Add(0);  // Charge
                        break;
                    default:
                        buffer.Add(0);
                        break;
                }
                buffer.AddRange(NetworkHelper.GetBytes((short)0)); // Intel grab time
                buffer.Add(0);  // Intel
                buffer.AddRange(NetworkHelper.GetBytes((short)0));  // Intel recharge

                // Weapon
                weapon.Serialize(type, buffer);
            }
        }

        public Player Player {
            get {
                return this.player;
            }
        }
    }
}
