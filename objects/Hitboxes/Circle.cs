namespace GG2server.objects.Hitboxes {
    public struct Circle {
        public float x1;
        public float y1;
        public float radius;

        /// <summary>
        /// Defines a circle.
        /// </summary>
        /// <param name="x1">center point x.</param>
        /// <param name="y1">center point y.</param>
        /// <param name="radius">circle radius.</param>
        public Circle(float x1, float y1, float radius) {
            this.x1 = x1;
            this.y1 = y1;
            this.radius = radius;
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
