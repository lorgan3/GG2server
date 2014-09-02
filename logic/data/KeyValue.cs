using System;
using System.Collections.Generic;

namespace GG2server.logic.data {
    [Serializable]
    public struct KeyValue {
        private string key;
        private object value;

        public KeyValue(string key, object value) {
            this.key = key;
            this.value = value;
        }

        public string Key {
            get {
                return this.key;
            }
            set {
                this.key = value;
            }
        }

        public object Value {
            get {
                return this.value;
            }
            set {
                this.value = value;
            }
        }

        public override string ToString() {
            if (value == null) String.Format("'{0}': null", key);
            return String.Format("'{0}': '{1}'", key, value);
        }
    }
}
