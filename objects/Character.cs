using GG2server.logic.data;
//using Physics2DDotNet;
//using Physics2DDotNet.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GG2server.objects {
    class Character : IEntity {
        private float x;
        private float y;
        private float hp;
        private float maxHP;
        private Player player;

        public float vspeed;
        public float hspeed;

        public Character(Player player) {
            this.player = player;
        }

        public void Die(int damageSource, Player damageDealer, Player assistant) {
            var buffer = GG2server.Server.Sendbuffer;
            buffer.Add(Constants.PLAYER_DEATH);
            buffer.Add((byte)Player.Players.IndexOf(player));
            buffer.Add((byte)(damageDealer == null ? 255 : Player.Players.IndexOf(damageDealer)));
            buffer.Add((byte)(assistant == null ? 255 : Player.Players.IndexOf(assistant)));
            buffer.Add((byte)damageSource);

            player.Character = null;
            this.player = null;
        }
        
        public void Serialize() {
        }

        public void Deserialize() {
        }
    }
}
