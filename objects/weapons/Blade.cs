
namespace GG2server.objects.weapons {
    class Blade : Weapon {
        public Blade(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 55 / 30 * 1000;
            refireTime = 4 / 30 * 1000;
            reloadBuffer = 0 / 30 * 1000;
        }
    }
}
