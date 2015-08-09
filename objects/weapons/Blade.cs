
namespace GG2server.objects.weapons {
    class Blade : Weapon {
        public Blade(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 55 / 30;
            refireTime = 1000 * 4 / 30;
            reloadBuffer = 1000 * 0 / 30;
        }
    }
}
