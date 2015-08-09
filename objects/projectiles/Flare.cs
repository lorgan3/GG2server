using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Flare : Projectile {
        internal float burnIncrease = 8f;
        internal int durationIncrease = 35;

        public Flare(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 30f;

            Despawn(40 / 30 * 1000);
        }
    }
}
