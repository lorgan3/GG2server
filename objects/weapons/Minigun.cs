
using System;
using System.Threading.Tasks;

namespace GG2server.objects.weapons {
    class Minigun : Weapon {
        private readonly Task Task;

        public Minigun(Character owner)
            : base(owner) {
            ammoCount = 200;
            maxAmmo = ammoCount;
            reloadTime = 1000 * 1 / 30;
            refireTime = 1000 * 2 / 30;
            reloadBuffer = 1000 * 10 / 30;
        }

        public async override void Reload(int timeout) {
            try {
                cts = new System.Threading.CancellationTokenSource();
                await Task.Delay(timeout, cts.Token);
                cts = null;

                if (ammoCount < maxAmmo) ammoCount += 1;
                if (ammoCount < maxAmmo) Reload(reloadTime);
            } catch (Exception) { };
        }
    }
}
