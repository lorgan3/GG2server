using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GG2server.objects {
    public abstract class Weapon : IEntity {
        internal byte ammoCount;
        internal byte maxAmmo;
        internal int reloadTime;
        internal int refireTime;
        internal int reloadBuffer;
        internal byte readyToShoot;
        internal DateTime startTime;
        internal Character owner;

        public Weapon(Character owner) {
            this.owner = owner;
            startTime = DateTime.Now;
            readyToShoot = 0;
            Refire(refireTime);
        }

        public async void Refire(int timeout) {
            await Task.Delay(timeout);
            readyToShoot = 1;
        }

        public async void Reload(int timeout) {
            await Task.Delay(timeout);
        }

        public void Primary() {
            if (readyToShoot == 1) {
                readyToShoot = 0;
                FireEvent(0);
                Refire(refireTime);
                Reload(reloadBuffer + reloadTime);
            }
        }

        public void Secondary() {
        }

        public void FireEvent(ushort seed) {
            var buffer = GG2server.Server.Sendbuffer;
            buffer.Add(Constants.WEAPON_FIRE);
            buffer.Add((byte)Player.Players.IndexOf(owner.Player));
            buffer.AddRange(NetworkHelper.GetBytes((ushort)owner.x * 5));
            buffer.AddRange(NetworkHelper.GetBytes((ushort)owner.y * 5));
            buffer.Add((byte)(owner.vspeed * 8.5));
            buffer.Add((byte)(owner.hspeed * 8.5));
            buffer.AddRange(NetworkHelper.GetBytes(seed));
        }

        public void Serialize(UpdateType type, List<byte> buffer) {
            buffer.Add(readyToShoot);
            buffer.Add((byte)(Math.Max(0, (DateTime.Now - startTime).Milliseconds - refireTime) / 1000 * 30));
        }
    }
}
