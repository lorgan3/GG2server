using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Bubble : Projectile {
        public Bubble(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 0.75f;

            Despawn(155 / 30 * 1000);
        }
    }
}
