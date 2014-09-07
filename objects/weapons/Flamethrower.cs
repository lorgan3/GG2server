
namespace GG2server.objects.weapons {
    class Flamethrower : Weapon {
        public Flamethrower(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 15 / 30 * 1000;
            refireTime = 1 / 30 * 1000;
            reloadBuffer = 7 / 30 * 1000;
        }
    }
}
