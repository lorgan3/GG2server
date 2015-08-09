using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GG2server.data {
    [Serializable]
    public class Config {
        public List<KeyValue> config;

        // For serialisation
        private Config() {
            config = new List<KeyValue>();
        }

        /// <summary>
        /// Reads a config value.
        /// </summary>
        /// <param name="section">The section it's saved under.</param>
        /// <param name="key">The key.</param>
        /// <param name="def">Default value if this key wasn't defined before.</param>
        /// <returns>The data read from the config file.</returns>
        public object ReadConfig(string section, string key, object def) {
            List<KeyValue> list = (List<KeyValue>)this.config.Find(kv => kv.Key == section).Value;
            if (list != null) {
                object obj = list.Find(kv => kv.Key == key).Value;


                LogHelper.Log(String.Format("\tRead {0}:{1} from config: '{2}'", section, key, GeneralHelper.ToString(obj)), LogLevel.debug);
                if (obj != null) return obj;
            }

            LogHelper.Log(String.Format("\tRead {0}:{1} from config: '{2}'", section, key, GeneralHelper.ToString(def)), LogLevel.debug);
            WriteConfig(section, key, def);
            return def;
        }

        /// <summary>
        /// Writes a config value.
        /// </summary>
        /// <param name="section">The section it has to be saved under.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The data that should be written to the config file.</param>
        public void WriteConfig(string section, string key, object value) {
            List<KeyValue> list = (List<KeyValue>)this.config.Find(kv => kv.Key == section).Value;
            if (list == null) {
                list = new List<KeyValue>();
                this.config.Add(new KeyValue(section, list));
            }
            KeyValue old = list.Find(kv => kv.Key == key);
            if (old.Key == null) {
                old.Key = key;
                old.Value = value;
                list.Add(old);
            } else old.Value = value;
        }
           
        /// <summary>
        /// Read the config file or create a new one.
        /// </summary>
        /// <returns>A config object containing the configs</returns>
        public static Config ConfigFactory() {
            if (File.Exists("gg2.xml")) {
                try {
                    Type[] extra = { typeof(string[]) };
                    XmlSerializer s = new XmlSerializer(typeof(Config), extra);
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
                Type[] extra = { typeof(string[]) };
                XmlSerializer s = new XmlSerializer(typeof(Config), extra);
                using (FileStream fs = new FileStream("gg2.xml", FileMode.Create, FileAccess.Write)) {
                    using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true, NewLineOnAttributes = false })) {
                        s.Serialize(xw, this);
                    }
                }
            } catch (Exception ex) {
                LogHelper.Log(ex.ToString(), LogLevel.warning);
            }
        }
    }
}
