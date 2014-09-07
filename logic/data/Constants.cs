
namespace GG2server.logic.data {
    static class Constants {
        public const string LOBBY_SERVER_HOST = "ganggarrison.com";
        public const int LOBBY_SERVER_PORT = 29944;
        public const string PROTOCOL_UUID = "de7d74f8-455c-bc1b-3713-0519c44356dc";
        public const string GG2_LOBBY_UUID = "1ccf16b1-436d-856f-504d-cc1af306aaa7";
        public const string GAME_NAME_STRING = "c# gg2 server";
        public const string GAME_VERSION_STRING = "v0.0.1";
        public const string GAME_URL_STRING = "http://www.ganggarrison.com";
        public const int VERSION = 27400;

        public const int HELLO = 0;
        public const int PLAYER_JOIN = 1;
        public const int PLAYER_LEAVE = 2;
        public const int PLAYER_CHANGETEAM = 3;
        public const int PLAYER_CHANGECLASS = 4;
        public const int INPUTSTATE = 6;
        public const int CHANGE_MAP = 7;
        public const int PLAYER_DEATH = 10;
        public const int SERVER_FULL = 11;
        public const int CHAT_BUBBLE = 15;
        public const int BUILD_SENTRY = 16;
        public const int DESTROY_SENTRY = 17;
        public const int DROP_INTEL = 21;
        public const int OMNOMNOMNOM = 24;
        public const int PASSWORD_REQUEST = 25;
        public const int PASSWORD_WRONG = 27;
        public const int PLAYER_CHANGENAME = 31;
        public const int KICK = 37;
        public const int TOGGLE_ZOOM = 40;
        public const int INCOMPATIBLE_PROTOCOL = 42;
        public const int JOIN_UPDATE = 43;
        public const int DOWNLOAD_MAP = 44;
        public const int REWARD_UPDATE = 49;
        public const int REWARD_REQUEST = 50;
        public const int REWARD_CHALLENGE_CODE = 51;
        public const int REWARD_CHALLENGE_RESPONSE = 52;
        public const int WEAPON_FIRE = 54;
        public const int PLUGIN_PACKET = 55;
        public const int PING = 57;
        public const int CLIENT_SETTINGS = 58;

        public const int FINISHED_OFF_GIB = 19;

        public const sbyte commandBytesPrefixLength1 = -2;  // The length of the command is indicated by the first byte
        public const sbyte commandBytesPrefixLength2 = -3;  // The length of the command is indicated by the first two bytes

        public const byte KILLS = 0;
        public const byte DEATHS = 1;
        public const byte CAPS = 2;
        public const byte ASSISTS = 3;
        public const byte DESTRUCTION = 4;
        public const byte STABS = 5;
        public const byte HEALING = 6;
        public const byte DEFENSES = 7;
        public const byte INVULNS = 8;
        public const byte BONUS = 9;
        public const byte DOMINATIONS = 10;
        public const byte REVENGE = 11;
        public const byte POINTS = 12;
    }
}
