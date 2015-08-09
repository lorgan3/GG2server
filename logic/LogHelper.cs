using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.IO;

namespace GG2server.logic {
    public static class LogHelper {
        private static List<string> logs = new List<string>();
        private static LogLevel displayLevel = LogLevel.debug;

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message you want to display.</param>
        /// <param name="level">The importance of the message.</param>
        public static void Log(string message, LogLevel level) {
            if (level >= displayLevel) {
                Log2Color(level);
                message = String.Format("[{0}] {1}: {2}", DateTime.Now.ToShortTimeString(), level, message);
                Console.WriteLine(message);
                Console.ResetColor();

                logs.Add(message);
            }
        }

        /// <summary>
        /// Saves the logs to a file.
        /// </summary>
        /// <param name="filename">The file to save to.</param>
        /// <param name="append">Whether the file should be overwritten or not.</param>
        public static void SaveLogs(string filename, bool append) {
            FileMode mode = append ? FileMode.Append : FileMode.Create;
            using (FileStream fs = new FileStream(filename, mode)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.WriteLine(String.Join("\n", logs));
                }
            }
        }

        /// <summary>
        /// Clears the logs, save them to a file first!
        /// </summary>
        /// <returns>The amount of messages that were removed.</returns>
        public static int ClearLogs() {
            int amount = logs.Count;
            logs.Clear();
            return amount;
        }

        /// <summary>
        /// Sets the console color based on the loglevel.
        /// </summary>
        /// <param name="level">The loglevel.</param>
        public static void Log2Color(LogLevel level) {
            switch(level) {
                case LogLevel.debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.title:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    // White by default
                    break;
            }
        }

        /// <summary>
        /// Changes which messages are displayed. Only messages >= the level are displayed.
        /// </summary>
        public static LogLevel Level {
            get {
                return displayLevel;
            }
            set {
                if (value != displayLevel) logs.Add("** loglevel changed to " + value + " **");
                displayLevel = value;
            }
        }
    }
}
