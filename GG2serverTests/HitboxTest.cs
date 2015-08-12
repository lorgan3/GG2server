using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GG2server.objects.Hitboxes;
using GG2server.objects;

namespace GG2serverTests {
    [TestClass]
    public class HitboxTest {
        [TestMethod]
        public void CollidingRectanglesTest() {
            Rectangle rect1 = new Rectangle(20, 20, 60, 60);
            Rectangle rect2 = new Rectangle(10, 10, 30, 30);
            Rectangle rect3 = new Rectangle(80, 80, 100, 100);

            Assert.IsTrue(Hitbox.Collide(rect1, rect2), "Rectangles should collide.");
            Assert.IsFalse(Hitbox.Collide(rect1, rect3), "Rectangles shouldn't collide.");
        }

        [TestMethod]
        public void CollidingCirclesTest() {
            Circle circle1 = new Circle(20, 20, 5);
            Circle circle2 = new Circle(50, 30, 30);
            Circle circle3 = new Circle(50, 30, 20);

            Assert.IsTrue(Hitbox.Collide(circle1, circle2), "Circles should collide.");
            Assert.IsFalse(Hitbox.Collide(circle1, circle3), "Circles shouldn't collide.");
        }

        [TestMethod]
        public void CollidingPointsTest() {
            Point point1 = new Point(20, 20);
            Point point2 = new Point(20, 20);
            Point point3 = new Point(20, 21);

            Assert.IsTrue(Hitbox.Collide(point1, point2), "Points should collide.");
            Assert.IsFalse(Hitbox.Collide(point1, point3), "Points shouldn't collide.");
        }

        [TestMethod]
        public void CollidingLinesTest() {
            Line line1 = new Line(20, 20, 60, 60);
            Line line2 = new Line(10, 10, 30, 30);
            Line line3 = new Line(80, 80, 100, 100);

            Assert.IsTrue(Hitbox.Collide(line1, line2), "Lines should collide.");
            Assert.IsFalse(Hitbox.Collide(line1, line3), "Lines shouldn't collide.");
        }

        [TestMethod]
        public void CollidingRectangleCircleTest() {
            Rectangle rect1 = new Rectangle(20, 20, 60, 60);
            Circle circle1 = new Circle(30, 30, 8);
            Circle circle2 = new Circle(10, 10, 5);

            Assert.IsTrue(Hitbox.Collide(rect1, circle1), "Rectangle and circle should collide.");
            Assert.IsFalse(Hitbox.Collide(rect1, circle2), "Rectangle and circle shouldn't collide.");
        }

        [TestMethod]
        public void CollidingRectanglePointTest() {
            Rectangle rect1 = new Rectangle(20, 20, 60, 60);
            Point point1 = new Point(40, 40);
            Point point2 = new Point(15, 20);

            Assert.IsTrue(Hitbox.Collide(rect1, point1), "Rectangle and point should collide.");
            Assert.IsFalse(Hitbox.Collide(rect1, point2), "Rectangle and point shouldn't collide.");
        }

        [TestMethod]
        public void CollidingCirclePointTest() {
            Circle circle1 = new Circle(30, 30, 20);
            Point point1 = new Point(40, 40);
            Point point2 = new Point(10, 15);

            Assert.IsTrue(Hitbox.Collide(circle1, point1), "Circle and point should collide.");
            Assert.IsFalse(Hitbox.Collide(circle1, point2), "Circle and point shouldn't collide.");
        }

        [TestMethod]
        public void MoveRectangleTest() {
            Rectangle rect1 = new Rectangle(30, 30, 60, 60);
            rect1.move(0, 10);

            Assert.AreEqual(rect1.x1, 0);
            Assert.AreEqual(rect1.y1, 10);
            Assert.AreEqual(rect1.x2, 30);
            Assert.AreEqual(rect1.y2, 40);
        }

        [TestMethod]
        public void MoveCircleTest() {
            Circle circle1 = new Circle(30, 30, 10);
            circle1.move(0, 10);

            Assert.AreEqual(circle1.x1, 0);
            Assert.AreEqual(circle1.y1, 10);
            Assert.AreEqual(circle1.radius, 10);
        }

        [TestMethod]
        public void MovePointTest() {
            Point point1 = new Point(30, 20);
            point1.move(0, 10);

            Assert.AreEqual(point1.x1, 0);
            Assert.AreEqual(point1.y1, 10);
        }
    }
}
