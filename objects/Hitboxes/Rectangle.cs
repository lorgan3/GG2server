namespace GG2server.objects.Hitboxes {
    public struct Rectangle {
        public float x1;
        public float y1;

        public float x2;
        public float y2;

        /// <summary>
        /// Defines a rectangle.
        /// </summary>
        /// <param name="x1">top left x.</param>
        /// <param name="y1">top left y.</param>
        /// <param name="x2">bottom right x.</param>
        /// <param name="y2">bottom right y.</param>
        public Rectangle(float x1, float y1, float x2, float y2) {
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
    }
}
