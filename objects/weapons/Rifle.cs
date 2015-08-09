
namespace GG2server.objects.weapons {
    class Rifle : Weapon {
        internal int chargeTime;

        public Rifle(Character owner)
            : base(owner) {
            ammoCount = 0;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 40 / 30;
            refireTime = 1000 * 40 / 30;
            reloadBuffer = 0;
            chargeTime = 1000 * 105 / 30 ;
        }
    }
}
