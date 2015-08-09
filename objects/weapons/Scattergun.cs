
using GG2server.objects.projectiles;
using System;
using System.Threading.Tasks;
namespace GG2server.objects.weapons {
    class Scattergun : Weapon {
        public Scattergun(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 15 * 1000 / 30;
            refireTime = 20 * 1000 / 30;
            reloadBuffer = 20 * 1000 / 30;
        }

        public override void FireEvent(ushort seed) {
            base.FireEvent(seed);

            for(int i = 0; i < 6; i++) {
                Shot.Create(owner.Player, owner.x, owner.y, 13, owner.aimdirection, 4, 15);
            }
        }
    }
}
