using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic.data {
    [Serializable]
    public struct Map {
        private string md5;
        private Dictionary<string, Dictionary<string, string>> entities;
        private bool[,] walkmask;

        public Map(String md5, Dictionary<string, Dictionary<string, string>> entities, bool[,] walkmask) {
            this.md5 = md5;
            this.entities = entities;
            this.walkmask = walkmask;
        }

        public string Md5 {
            get {
                return this.md5;
            }
        }

        public Dictionary<string, Dictionary<string, string>> Entities {
            get {
                return this.entities;
            }
        }

        public bool[,] Walkmask {
            get {
                return this.walkmask;
            }
        }
    }
}
