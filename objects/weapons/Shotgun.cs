
using System;
using System.Threading.Tasks;

namespace GG2server.objects.weapons {
    class Shotgun : Weapon {
        public Shotgun(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 15 / 30;
            refireTime = 1000 * 20 / 30;
            reloadBuffer = 1000 * 20 / 30;
        }
    }
}
