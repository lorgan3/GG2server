using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GG2server.data {
    class FileReader : IEnumerable<string> {
        private List<string> lines;

        public FileReader(string path) {
            try {
                lines = File.ReadAllLines(path).ToList<string>();
            } catch (FileNotFoundException) {
                LogHelper.Log("File '" + path + "' does not exist.", LogLevel.warning);
                lines = new List<string>();
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.error);
            }
        }

        public IEnumerator<string> GetEnumerator() {
            return new LineEnumerator(lines);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new LineEnumerator(lines);
        }

        public int Lines {
            get {
                return lines.Count;
            }
        }

        internal class LineEnumerator : IEnumerator<string> {
            private List<string> lines;
            private int current;

            internal LineEnumerator(List<string> lines) {
                this.lines = lines;
                this.current = -1;
            }

            public string Current {
                get {
                    if (current >= lines.Count) {
                        if (current <= 0) return "";
                        current = -1;
                    }
                    return lines[current];
                }
            }

            public void Dispose() {
                // do nothing
            }

            object IEnumerator.Current {
                get {
                    return Current;
                }
            }

            public bool MoveNext() {
                current++;
                return (current < lines.Count);
            }

            public void Reset() {
                current = -1;
            }
        }
    }
}
