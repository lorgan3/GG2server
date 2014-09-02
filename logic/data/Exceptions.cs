using System;

namespace GG2server.logic.data {
    class UuidException : Exception {
        public UuidException() {}
        public UuidException(string message) : base(message) {}
        public UuidException(string message, Exception inner) : base(message, inner) {}
    }

    class MapException : Exception {
        public MapException() { }
        public MapException(string message) : base(message) { }
        public MapException(string message, Exception inner) : base(message, inner) { }
    }
}
