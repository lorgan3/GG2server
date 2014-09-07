
using GG2server.logic.data;
using System.Collections.Generic;
namespace GG2server.objects.weapons {
    class Minegun : Weapon {
        internal byte lobbed;

        public Minegun(Character owner)
            : base(owner) {
            lobbed = 0;
            ammoCount = 8;
            maxAmmo = ammoCount;
            reloadTime = 15 / 30 * 1000;
            refireTime = 26 / 30 * 1000;
            reloadBuffer = 26 / 30 * 1000;
        }

        public void Serialize(UpdateType type, List<byte> buffer) {
            base.Serialize(type, buffer);
            buffer.Add(lobbed);
        }
    }
}
