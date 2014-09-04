using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic.data {
    public static class Extensions {
        public static byte Read_ubyte(this Socket socket) {
            return socket.Read(1)[0];
        }

        public static ushort Read_ushort(this Socket socket) {
            return NetworkHelper.GetShort(socket.Read(2));
        }

        public static byte[] Read(this Socket socket, int amount) {
            byte[] buffer = new byte[amount];
            if (amount == 0) return buffer;

            socket.Receive(buffer, amount, SocketFlags.None);
            return buffer;
        }
    }
}
