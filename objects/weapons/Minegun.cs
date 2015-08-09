
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
            reloadTime = 1000 * 15 / 30;
            refireTime = 1000 * 26 / 30;
            reloadBuffer = 1000 * 26 / 30;
        }

        public new void Serialize(UpdateType type, List<byte> buffer) {
            base.Serialize(type, buffer);
            buffer.Add(lobbed);
        }
    }
}
