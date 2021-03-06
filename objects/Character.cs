﻿using GG2server.logic;
using GG2server.logic.data;
using GG2server.objects.Hitboxes;
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
        public float x=1000+GG2server.Server.random.Next(400);
        public float y;
        private float hp;
        private float maxHP;
        private Player player;
        private Weapon weapon;
        private Rectangle hitbox;
        private Point anchor;

        public float vspeed;
        public float hspeed;

        private float runPower;
        private byte tauntLength;

        public float aimdirection;
        public byte pressedKeys;
        public byte releasedKeys;
        public byte keyState;
        private byte oldKeyState;
        private short aimDistance;
        public ushort tmp;

        public Character(Player player) {
            this.player = player;
            vspeed = 10f;
            hspeed = 0f;

            hitbox = new Rectangle(0, 0, 12, 33);
            anchor = new Point(6, 10);

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
            buffer.Add(keyState);
            buffer.AddRange(NetworkHelper.GetBytes((ushort)((aimdirection * 65536) / 360.0), true));
            buffer.Add((byte)(aimDistance/2));

            if (type == UpdateType.quick || type == UpdateType.full) {
                buffer.AddRange(NetworkHelper.GetBytes((ushort)(x*5), true));
                buffer.AddRange(NetworkHelper.GetBytes((ushort)(y*5), true));
                buffer.Add((byte)(hspeed * 8.5));
                buffer.Add((byte)(vspeed * 8.5));
                buffer.Add((byte)Math.Ceiling(hp));
                buffer.Add(weapon.ammoCount);
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
                buffer.AddRange(NetworkHelper.GetBytes((short)0, true)); // Intel grab time
                buffer.Add(0);  // Intel
                buffer.AddRange(NetworkHelper.GetBytes((short)0, true));  // Intel recharge

                // Weapon
                weapon.Serialize(type, buffer);
            }
        }

        public Player Player {
            get {
                return this.player;
            }
        }

        public byte KeyState {
            set {
                oldKeyState = keyState;
                keyState = value;
                pressedKeys = (byte)(keyState & ~oldKeyState);
                releasedKeys = (byte)~pressedKeys;
            }
        }

        public ushort AimDirection {
            set {
                aimdirection = (float)((value * 360.0) / 65536);
            }
        }

        public byte AimDistance {
            set {
                aimDistance = (short)(value * 2);
            }
        }

        public Weapon Weapon {
            get {
                return this.weapon;
            }
        }

        public Rectangle Hitbox {
            get {
                return this.hitbox;
            }
        }

        public Point Anchor {
            get {
                return this.anchor;
            }
        }
    }
}
