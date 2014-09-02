
namespace GG2server.objects {
    class Player {
        private static byte length = 0;

        public Player() {
            length += 1;
        }

        ~Player() {
            length -= 1;
        }

        public static byte Length {
            get {
                return length;
            }
        }
    }
}
