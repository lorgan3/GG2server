
namespace GG2server.logic.data {
    public enum LogLevel : byte { debug, info, warning, error, title };
    public enum Team : byte { red, blue, spectator };
    public enum Class : byte { scout, soldier, sniper, demoman, medic, engineer, heavy, spy, pyro, quote };
    public enum UpdateType : byte { quick=9, full=8, caps=28, inputstate=6 };
}
