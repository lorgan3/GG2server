﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic.data {
    [Serializable]
    public struct Map {
        private string md5;
        private Dictionary<string, object> entities;
        private string walkmaskString;
        private int width;
        private int height;

        [NonSerialized]
        private bool[,] walkmask;

        public Map(String md5, Dictionary<string, object> entities, int width, int height, string walkmaskString) {
            this.md5 = md5;
            this.entities = entities;

            this.width = width;
            this.height = height;
            this.walkmaskString = walkmaskString;

            this.walkmask = Map.Parse(width, height, walkmaskString);
        }

        private static bool[,] Parse(int width, int height, string walkmaskString) {
            bool[,] mask = new bool[width, height];
            int index = 0;
            byte curByte = (byte)(walkmaskString[index] - 32);
            int byte_fill = 0;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    mask[j, i] = (curByte & (byte)1) == 1;

                    curByte >>= 1;
                    byte_fill++;
                    if (byte_fill == 6) {
                        byte_fill = 0;
                        index++;
                        curByte = (byte)(walkmaskString[index] - 32);
                    }
                }
            }

            return mask;
        }

        public string Md5 {
            get {
                return this.md5;
            }
        }

        public Dictionary<string, object> Entities {
            get {
                return this.entities;
            }
        }

        public int Width {
            get {
                return this.width;
            }
        }

        public int Height {
            get {
                return this.height;
            }
        }

        public bool[,] Walkmask {
            get {
                return this.walkmask;
            }
        }
    }
}
