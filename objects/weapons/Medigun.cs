
using GG2server.logic.data;
using System.Collections.Generic;
namespace GG2server.objects.weapons {
    class Medigun : Weapon {
        internal byte healTarget;
        internal float uberCharge;
        internal byte ubering;
        internal byte uberReady;

        public Medigun(Character owner)
            : base(owner) {
            ammoCount = 40;
            maxAmmo = ammoCount;
            reloadTime = 3 / 30 * 1000;
            refireTime = 55 / 30 * 1000;
            reloadBuffer = 0 / 30 * 1000;

            healTarget = 255;
            uberCharge = 0;
            ubering = 0;
            uberReady = 0;
        }

        public new void Serialize(UpdateType type, List<byte> buffer) {
            buffer.Add(healTarget);
        }
    }
}
