
namespace GG2server.objects.weapons {
    class Revolver : Weapon {
        public Revolver(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 45 / 30 * 1000;
            refireTime = 18 / 30 * 1000;
            reloadBuffer = 18 / 30 * 1000;
        }
    }
}
