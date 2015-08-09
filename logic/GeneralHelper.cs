using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.logic {
    public class GeneralHelper {
        /// <summary>
        /// Properly prints objects that are arrays.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns></returns>
        public static String ToString(object obj) {
            if (obj.GetType().IsArray) {
                return String.Format("[{0}]", String.Join(", ", ((IEnumerable)obj).Cast<object>().Select(x => x.ToString()).ToArray()));
            } else {
                return obj.ToString();
            }
        }
    }
}
