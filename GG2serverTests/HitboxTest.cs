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
            Rectangle rect2 = new Rectangle(10, 10, 30, 10);
            Rectangle rect3 = new Rectangle(80, 80, 100, 100);

            Assert.IsTrue(Hitbox.Collide(rect1, rect2), "Rectangles should collide.");
            Assert.IsFalse(Hitbox.Collide(rect1, rect3), "Rectangles shouldn't collide.");
        }
    }
}
