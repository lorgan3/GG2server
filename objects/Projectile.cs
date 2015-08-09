using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GG2server.objects {
    public abstract class Projectile {
        private static List<Projectile> projectiles = new List<Projectile>();
        public float x;
        public float y;
        public float vspeed;
        public float hspeed;

        internal Player player;
        internal float hitDamage;
        internal CancellationTokenSource cts;

        public Projectile(float x, float y, Player player) {
            this.x = x;
            this.y = y;
            this.player = player;

            projectiles.Add(this);
        }

        public async virtual void Despawn(int timeout) {
            try {
                cts = new System.Threading.CancellationTokenSource();
                await Task.Delay(timeout, cts.Token);
                cts = null;

                projectiles.Remove(this);
            } catch (Exception) { };
        }

        public virtual void doPrjectileStep() {
            this.x += hspeed / 2;
            this.y += vspeed / 2;
        }

        public static List<Projectile> Projectiles {
            get {
                return projectiles;
            }
        }
    }
}
