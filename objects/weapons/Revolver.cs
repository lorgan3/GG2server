
using System;
using System.Threading.Tasks;

namespace GG2server.objects.weapons {
    class Revolver : Weapon {
        public Revolver(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 45 / 30;
            refireTime = 1000 * 18 / 30;
            reloadBuffer = 1000 * 18 / 30;
        }

        public async override void Reload(int timeout) {
            try {
                cts = new System.Threading.CancellationTokenSource();
                await Task.Delay(timeout, cts.Token);
                cts = null;

                ammoCount = maxAmmo;
            } catch (Exception) { };
        }
    }
}
