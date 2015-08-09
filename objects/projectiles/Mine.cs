using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Mine : Projectile {
        internal int explosionDamage = 45;
        internal bool stickied = false;
        internal int blastRadius = 40;
        internal Player reflector = null;
        internal int knockback = 10;

        public Mine(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 50f;
        }
    }
}
