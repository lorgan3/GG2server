using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Rocket : Projectile {
        protected int explosionDamage = 30;
        protected int blastRadius = 65;
        protected int knockback = 8;
        internal int distanceToTravel = 800;
        internal List<Player> playersPassed;

        internal bool fade;

        public Rocket(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 25;
            explosionDamage = 30;
            blastRadius = 65;
            knockback = 8;
            fade = false;
            distanceToTravel = 800;
            playersPassed = new List<Player>();

            Despawn(55 / 30 * 1000);
        }
    }
}
