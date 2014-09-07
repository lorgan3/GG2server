
namespace GG2server.objects.weapons {
    class Minigun : Weapon {
        public Minigun(Character owner)
            : base(owner) {
            ammoCount = 200;
            maxAmmo = ammoCount;
            reloadTime = 0 / 30 * 1000;
            refireTime = 2 / 30 * 1000;
            reloadBuffer = 10 / 30 * 1000;
        }
    }
}
