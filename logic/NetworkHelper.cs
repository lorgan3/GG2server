using GG2server.logic.data;
using Open.Nat;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GG2server.logic {
    static class NetworkHelper {

        /// <summary>
        /// Parse an UUID.
        /// </summary>
        /// <param name="uuid">The UUID to be parsed.</param>
        /// <returns>A 16-byte array containing the parsed UUID</returns>
        public static byte[] ParseUuid(string uuid) {
            byte[] parsed = new byte[16];

            uuid = uuid.Replace("-", "");
            if (uuid.Length != 32) throw new UuidException("The uuid '" + uuid + "' has an incorrect length");
            string posValue = "0123456789abcdef";
            
            for (byte i = 0; i < 16; i += 1) {
                char currentNibble = uuid[i * 2];
                if (!posValue.Contains(currentNibble)) throw new UuidException("The uuid '" + uuid + "' contains invalid characters");
                byte numericByte = (byte)(posValue.IndexOf(currentNibble) * 16);

                currentNibble = currentNibble = uuid[i * 2 + 1];
                if (!posValue.Contains(currentNibble)) throw new UuidException("The uuid '" + uuid + "' contains invalid characters");

                numericByte += (byte)(posValue.IndexOf(currentNibble));
                parsed[i] = numericByte;
            }

            return parsed;
        }

        /// <summary>
        /// Converts a value to a big endian byte array.
        /// </summary>
        public static byte[] GetBytes(short x, bool isLittleEndian) {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(ushort x, bool isLittleEndian) {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(int x, bool isLittleEndian) {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(float x, bool isLittleEndian) {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(uint x, bool isLittleEndian) {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(string x) {
            return Encoding.ASCII.GetBytes(x);
        }
        public static string GetString(byte[] buffer) {
            return Encoding.ASCII.GetString(buffer);
        }

        /// <summary>
        /// Get a long from a buffer with 4 bytes
        /// </summary>
        public static long GetLong(byte[] buffer, bool isLittleEndian) {
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static ushort GetShort(byte[] buffer, bool isLittleEndian) {
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// Return a buffer that contains the encoded key and value.
        /// </summary>
        public static byte[] KeyValueToBytes(string key, string value) {
            byte[] keyBytes = GetBytes(key);
            byte[] valueBytes = GetBytes(value);

            byte[] bytes = new byte[3 + keyBytes.Length + valueBytes.Length];
            bytes[0] = (byte)keyBytes.Length;
            keyBytes.CopyTo(bytes, 1);
            GetBytes((short)valueBytes.Length, false).CopyTo(bytes, 1 + keyBytes.Length);
            valueBytes.CopyTo(bytes, 3 + keyBytes.Length);

            return bytes;
        }
        public static byte[] KeyValueToBytes(string key, byte[] valueBytes) {
            byte[] keyBytes = GetBytes(key);

            byte[] bytes = new byte[3 + keyBytes.Length + valueBytes.Length];
            bytes[0] = (byte)keyBytes.Length;
            keyBytes.CopyTo(bytes, 1);
            GetBytes((short)valueBytes.Length, false).CopyTo(bytes, 1 + keyBytes.Length);
            valueBytes.CopyTo(bytes, 3 + keyBytes.Length);

            return bytes;
        }

        /// <summary>
        /// Copies a block of bytes from a source buffer.
        /// </summary>
        /// <param name="buffer">The source buffer.</param>
        /// <param name="offset">Index in the source buffer.</param>
        /// <param name="length">How many bytes should be copied.</param>
        /// <returns></returns>
        public static byte[] CopyByteBlock(byte[] buffer, int offset, int length) {
            byte[] b = new byte[length];
            Buffer.BlockCopy(buffer, offset, b, 0, length);
            return b;
        }
        public static byte[] CopyByteBlock(Stream stream, int offset, int length) {
            byte[] b = new byte[length];
            stream.Position = 0;
            stream.Read(b, offset, length);
            return b;
        }

        /// <summary>
        /// Opens a port using upnp.
        /// </summary>
        /// <param name="port">The port you want to open.</param>
        public static async void RegisterServerport(int port) {
            try {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, port, port, "C# Gang Garrison 2 server"));
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.warning);
            }
        }

        /// <summary>
        /// Closes upnp port.
        /// </summary>
        /// <param name="port">The port you want to close.</param>
        public static async void UnregServerport(int port) {
            try {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                await device.DeletePortMapAsync(new Mapping(Protocol.Tcp, port, port));
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.warning);
            }
        }
    }
}
