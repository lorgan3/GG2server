using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Needle : Projectile {
        public Needle(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 8;

            Despawn(40 / 30 * 1000);
        }
    }
}
