using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GG2server.data {
    [Serializable]
    public class Config {
        private List<KeyValue> config;

        // For serialisation
        private Config() {
            config = new List<KeyValue>();
            config.Add(new KeyValue("Settings", new List<KeyValue>()));
            config.Add(new KeyValue("Server", new List<KeyValue>()));
        }
           
        /// <summary>
        /// Read the config file or create a new one.
        /// </summary>
        /// <returns>A config object containing the configs</returns>
        public static Config ConfigFactory() {
            if (File.Exists("gg2.xml")) {
                try {
                    XmlSerializer s = new XmlSerializer(typeof(Config));
                    using (FileStream fs = new FileStream("gg2.xml", FileMode.Open, FileAccess.Read)) {
                        return (Config)s.Deserialize(fs);
                    }
                } catch (Exception ex) {
                    LogHelper.Log(ex.ToString(), LogLevel.warning);
                }
            }

            return new Config();
        }

        ~Config() {
            try {
                XmlSerializer s = new XmlSerializer(typeof(Config));
                using (FileStream fs = new FileStream("gg2.xml", FileMode.Create, FileAccess.Write)) {
                    s.Serialize(fs, this);
                }
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.warning);
            }
        }

        public List<KeyValue> Settings {
            get {
                return (List<KeyValue>)this.config.Find(kv => kv.Key == "Settings").Value;
            }
            set {
                this.config.Remove(this.config.Find(kv => kv.Key == "Settings"));
                this.config.Add(new KeyValue("Settings", value));
            }
        }

        public List<KeyValue> Server {
            get {
                return (List<KeyValue>)this.config.Find(kv => kv.Key == "Server").Value;
            }
            set {
                this.config.Remove(this.config.Find(kv => kv.Key == "Server"));
                this.config.Add(new KeyValue("Server", value));
            }
        }
    }
}
