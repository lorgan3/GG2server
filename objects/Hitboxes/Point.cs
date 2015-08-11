using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects.Hitboxes {
    public struct Point {
        public float x1;
        public float y1;

        /// <summary>
        /// Defines a point.
        /// </summary>
        /// <param name="x1">point x.</param>
        /// <param name="y1">point y.</param>
        public Point(float x1, float y1) {
            this.x1 = x1;
            this.y1 = y1;
        }

        /// <summary>
        /// Move to this point.
        /// </summary>
        /// <param name="x">point x.</param>
        /// <param name="y">point y.</param>
        public void move(float x, float y) {
            this.x1 = x;
            this.y1 = y;
        }
    }
}
