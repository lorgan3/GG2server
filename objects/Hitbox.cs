using GG2server.logic.data;
using GG2server.objects.Hitboxes;
using System;

namespace GG2server.objects {
    public static class Hitbox {

        /// <summary>
        /// Checks if a rectangle collides with the map.
        /// </summary>
        /// <param name="self">The rectangle.</param>
        /// <param name="direction">The direction that should be checked.</param>
        /// <returns></returns>
        public static bool Collide(Rectangle self, Directions direction) {
            // TODO
            return false;
        }

        /// <summary>
        /// Check if 2 rectangles collide.
        /// </summary>
        /// <param name="self">Rectangle 1.</param>
        /// <param name="other">Rectangle 2.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Rectangle self, Rectangle other) {
            return !(self.x1 > other.x2 || self.x2 < other.x1 || self.y1 > other.y2 || self.y2 < other.y1);
        }

        /// <summary>
        /// Check if 2 circles collide.
        /// </summary>
        /// <param name="self">Circle 1.</param>
        /// <param name="other">Circle 2.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Circle self, Circle other) {
            return Math.Sqrt((self.x1 - other.x1) * (self.x1 - other.x1) + (self.y1 - other.y1) * (self.y1 - other.y1)) < (self.radius + other.radius);
        }

        /// <summary>
        /// Check if 2 points collide.
        /// </summary>
        /// <param name="self">Point 1.</param>
        /// <param name="other">Point 2.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Point self, Point other) {
            return (self.x1 == other.x1 && self.y1 == other.y1);
        }

        /// <summary>
        /// Check if 2 lines collide.
        /// </summary>
        /// <param name="self">Line 1.</param>
        /// <param name="other">Line 2.</param>
        /// <returns></returns>
        public static bool Collide(Line self, Line other) {
            return Line.DoLinesIntersect(self, other);
        }

        /// <summary>
        /// Check if a rectangle and a circle collide.
        /// </summary>
        /// <param name="self">Rectangle.</param>
        /// <param name="other">Circle.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Rectangle self, Circle other) {
            float closestX = (other.x1 < self.x1) ? self.x1 : ((other.x1 > self.x2) ? self.x2 : other.x1);
            float closestY = (other.y1 < self.y1) ? self.y1 : ((other.y1 > self.y2) ? self.y2 : other.y1);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = other.x1 - closestX;
            float distanceY = other.x1 - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (other.radius * other.radius);
        }

        /// <summary>
        /// Check if a rectangle and a point collide.
        /// </summary>
        /// <param name="self">Rectangle.</param>
        /// <param name="other">Point.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Rectangle self, Point other) {
            return (self.x1 <= other.x1 && self.x2 >= other.x1) && (self.y1 <= other.y1 && self.y2 >= other.y1);
        }

        /// <summary>
        /// Checks if a rectangle and a line collide.
        /// </summary>
        /// <param name="self">Rectangle.</param>
        /// <param name="other">Line.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Rectangle self, Line other) {
            // TODO
            return false;
        }

        /// <summary>
        /// Check if a circle and a point collide.
        /// </summary>
        /// <param name="self">Circle.</param>
        /// <param name="other">Point.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Circle self, Point other) {
            return Math.Sqrt((self.x1 - other.x1) * (self.x1 - other.x1) + (self.y1 - other.y1) * (self.y1 - other.y1)) <= self.radius;
        }

        /// <summary>
        /// Checks if a cricle and a line collide.
        /// </summary>
        /// <param name="self">Cirle.</param>
        /// <param name="other">Line.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Circle self, Line other) {
            // TODO
            return false;
        }

        /// <summary>
        /// Checks if a point and a line collide.
        /// </summary>
        /// <param name="self">Point.</param>
        /// <param name="other">Line.</param>
        /// <returns>True if they collide.</returns>
        public static bool Collide(Point self, Line other) {
            // TODO
            return false;
        }
    }
}
