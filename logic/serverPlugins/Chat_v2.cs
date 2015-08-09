using GG2server.logic.data;
using GG2server.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic.serverPlugins {
    class Chat_v2 : ServerPlugin {
        public override void ReceivePluginPacket(Player player, byte[] buffer) {
            switch (buffer[0]) {
                case 0: // HELLO
                    player.hasChat = true;
                    break;
                case 1: // global chat
                    String global = NetworkHelper.GetString(NetworkHelper.CopyByteBlock(buffer, 2, buffer[1]));
                    LogHelper.Log(String.Format("{0}: {1}", player.Name, global), LogLevel.chat);

                    sendMsg(formatMessage(false, player.Team, player.Name, global), Team.any);
                    break;
                case 2: // team chat
                    String team = NetworkHelper.GetString(NetworkHelper.CopyByteBlock(buffer, 2, buffer[1]));
                    LogHelper.Log(String.Format("[TEAM] {0}: {1}", player.Name, team), LogLevel.chat);

                    sendMsg(formatMessage(true, player.Team, player.Name, team), player.Team);
                    break;
            }
        }

        private String formatMessage(bool isTeam, Team team, String name, String msg) {
            Char prefix;
            if (team == Team.blue) {
                prefix = 'B';
            } else if (team == Team.red) {
                prefix = 'R';
            } else {
                prefix = 'W';
            }

            if (isTeam) {
                return String.Format("{0}{1}: {2}", prefix, name, msg);
            } else {
                return String.Format("{0}{1}: #W{2}", prefix, name, msg);
            }
        }

        private void sendMsg(String msg, Team team) {
            if (msg.Length > 255) {
                msg = msg.Substring(0, 255);
            }
            List<byte> buffer = new List<byte>();
            buffer.Add(Constants.PLUGIN_PACKET);
            buffer.AddRange(NetworkHelper.GetBytes((ushort)(msg.Length+2), true));
            buffer.Add(packetId);
            buffer.Add((byte)msg.Length);
            buffer.AddRange(NetworkHelper.GetBytes(msg));
            byte[] sendBuffer = buffer.ToArray();

            foreach (Player player in Player.Players) {
                if (player.hasChat && player.Team == team || team == Team.any) {
                    player.Socket.Send(sendBuffer);
                }
            }
        }
    }
}
