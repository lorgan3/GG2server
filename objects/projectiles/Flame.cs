using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Flame : Projectile {
        internal float burnIncrease = 1.35f;
        internal int durationIncrease = 30;

        public Flame(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 3.14f;

            Despawn(15 / 30 * 1000);
        }
    }
}
