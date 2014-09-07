
namespace GG2server.objects.weapons {
    class Shotgun : Weapon {
        public Shotgun(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 15 / 30 * 1000;
            refireTime = 20 / 30 * 1000;
            reloadBuffer = 20 / 30 * 1000;
        }
    }
}
