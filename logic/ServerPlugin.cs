using GG2server.logic.data;
using GG2server.logic.serverPlugins;
using GG2server.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic {
    public abstract class ServerPlugin {
        private static Dictionary<AvailablePlugins, string> serverPlugins = new Dictionary<AvailablePlugins, string> { { AvailablePlugins.chat_v2, "chat_v2@23a35be03bcca9da69d3c9590c3def6d" } };
        private static List<ServerPlugin> packetIds = new List<ServerPlugin>();

        internal byte packetId;

        public ServerPlugin() {
            packetId = (byte)packetIds.Count;
            packetIds.Add(this);
        }

        /// <summary>
        /// Plugin implementations should override this function to receive their plugindata.
        /// </summary>
        /// <param name="player">The player that sent the packet.</param>
        /// <param name="buffer">The packet data.</param>
        public virtual void ReceivePluginPacket(Player player, byte[] buffer) {
        }

        /// <summary>
        /// Create server sent plugins so they can respond to plugin packets.
        /// </summary>
        /// <param name="pluginList">The plugins.</param>
        public static void parsePlugins(string pluginList) {
            String[] plugins = pluginList.Split(',');
            foreach(String plugin in plugins) {
                switch (getEnum(plugin)) {
                    case AvailablePlugins.chat_v2:
                        new Chat_v2();
                        break;
                    default:
                        LogHelper.Log(String.Format("Got an unimplemented server sent plugin: {0}", plugin), LogLevel.error);
                        break;
                }
           }
        }

        public static void ReceivePluginPacket(byte packetId, Player player, byte[] buffer) {
            packetIds[packetId].ReceivePluginPacket(player, buffer);
        }

        public static string GetRealName(AvailablePlugins plugin) {
            return serverPlugins[plugin];
        }

        public static AvailablePlugins getEnum(string plugin) {
            return serverPlugins.FirstOrDefault(x => x.Value == plugin).Key;
        }
    }
}
