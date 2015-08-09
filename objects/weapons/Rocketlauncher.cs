
namespace GG2server.objects.weapons {
    class Rocketlauncher : Weapon {
        public Rocketlauncher(Character owner)
            : base(owner) {
            ammoCount = 4;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 22 / 30;
            refireTime = 1000 * 30 / 30;
            reloadBuffer = 1000 * 30 / 30;
        }
    }
}
