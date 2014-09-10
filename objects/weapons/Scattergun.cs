
using System;
using System.Threading.Tasks;
namespace GG2server.objects.weapons {
    class Scattergun : Weapon {
        public Scattergun(Character owner)
            : base(owner) {
            ammoCount = 6;
            maxAmmo = ammoCount;
            reloadTime = 15 * 1000 / 30;
            refireTime = 20 * 1000 / 30;
            reloadBuffer = 20 * 1000 / 30;
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
