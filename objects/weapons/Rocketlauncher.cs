
namespace GG2server.objects.weapons {
    class Rocketlauncher : Weapon {
        public Rocketlauncher(Character owner)
            : base(owner) {
            ammoCount = 4;
            maxAmmo = ammoCount;
            reloadTime = 22 / 30 * 1000;
            refireTime = 30 / 30 * 1000;
            reloadBuffer = 30 / 30 * 1000;
        }
    }
}
