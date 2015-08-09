using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.projectiles {
    class Shot : Projectile {

        public Shot(float x, float y, Player owner)
            : base(x, y, owner) {
            this.hitDamage = 8;

            Despawn(40 / 30 * 1000);
        }

        public static Shot Create(Player owner, float x, float y, float speed, float direction) {
            Shot shot = new Shot(x, y, owner);
            shot.hspeed = (float)(speed * Math.Sin(direction));
            shot.vspeed = (float)(speed * Math.Cos(direction));
            return shot;
        }

        public static Shot Create(Player owner, float x, float y, float speed, float direction, int speedVariance, int directionVariance) {
            return Create(owner, x, y, speed - speedVariance / 2 + (int)(GG2server.random.NextDouble() * speedVariance), direction - directionVariance / 2 + (int)(GG2server.random.NextDouble() * directionVariance));
        }

         ~Shot() {
            LogHelper.Log("rip", LogLevel.Debug);
        }
    }
}
