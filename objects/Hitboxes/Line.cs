using System;

namespace GG2server.objects.Hitboxes {
    public struct Line {
        public const double EPSILON = 0.000001;

        public float x1;
        public float y1;

        public float x2;
        public float y2;

        /// <summary>
        /// Defines a Line segmentt.
        /// </summary>
        /// <param name="x1">top left x.</param>
        /// <param name="y1">top left y.</param>
        /// <param name="x2">bottom right x.</param>
        /// <param name="y2">bottom right y.</param>
        public Line(float x1, float y1, float x2, float y2) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        /// <summary>
        /// Move to this point.
        /// </summary>
        /// <param name="x">point x.</param>
        /// <param name="y">point y.</param>
        public void move(float x, float y) {
            float diffX = x2 - y1;
            float diffY = y2 - y1;

            this.x1 = x;
            this.y1 = y;
            this.x2 = x + diffX;
            this.y2 = y + diffY;
        }        

        public static double CrossProduct(float x1, float y1, float x2, float y2) {
            return x1 * y2 - x2 * y1;
        }

        public static bool DoBoundingBoxesIntersect(Line line1, Line line2) {
            return line1.x1 <= line2.x2 && line1.x2 >= line2.x1 && line1.y1 <= line2.y2 && line1.y2 >= line2.y1;
        }

        public static bool IsPointOnLine(Line line1, float x1, float y1) {
            Line tmp = new Line(0, 0, line1.x2 - line1.x1, line1.y2 - line1.y1);
            x1 -= line1.x1;
            y1 -= line1.y1;

            double r = CrossProduct(tmp.x2, tmp.y2, x1, y1);
            return Math.Abs(r) < EPSILON;
        }

        public static bool IsPointRightOfLine(Line line1, float x1, float y1) {
            Line tmp = new Line(0, 0, line1.x2 - line1.x1, line1.y2 - line1.y1);
            x1 -= line1.x1;
            y1 -= line1.y1;

            return CrossProduct(tmp.x2, tmp.y2, x1, y1) < 0;
        }

        public static bool LineTouchesOrCrossesLine(Line line1, Line line2) {
            return IsPointOnLine(line1, line2.x1, line2.y1) 
                || IsPointOnLine(line1, line2.x2, line2.y2) 
                || (IsPointRightOfLine(line1, line2.x1, line2.y1) ^ IsPointRightOfLine(line1, line2.x2, line2.y2));
        }

        public static bool DoLinesIntersect(Line line1, Line line2) {
            return DoBoundingBoxesIntersect(line1, line2) && LineTouchesOrCrossesLine(line1, line2) && LineTouchesOrCrossesLine(line2, line1);
        }
    }
}
