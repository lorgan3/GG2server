
namespace GG2server.objects.weapons {
    class Rifle : Weapon {
        internal int chargeTime;

        public Rifle(Character owner)
            : base(owner) {
            ammoCount = 0;
            maxAmmo = ammoCount;
            reloadTime = 40 / 30 * 1000;
            refireTime = 40 / 30 * 1000;
            reloadBuffer = 0;
            chargeTime = 105 / 30 * 1000;
        }
    }
}
