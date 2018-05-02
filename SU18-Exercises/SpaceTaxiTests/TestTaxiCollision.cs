using System;
using System.IO;
using DIKUArcade;
using NUnit.Framework;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi_1;

namespace SpaceTaxiTests {
    public class TestTaxiCollision {
        [TestFixture()]
        public class TestGame {
            private Entity e1;
            private Entity e2;
        
            [SetUp()]
            public void Setup()
            {
                // win is closed when all tests are done
                var win = new Window("Space Taxi Game v0.1", 800, AspectRatio.R1X1);
                
                e1 = new Entity(new DynamicShape(new Vec2F(), new Vec2F(0.1f, 0.1f)), 
                    new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png")));
                e2 = new Entity(new DynamicShape(new Vec2F(), new Vec2F(0.1f, 0.1f)), 
                    new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png")));
            }
            
            [Test()]
            public void NoCollision() {
                e1.Shape.Position = new Vec2F(0.1f, 0.1f);
                e2.Shape.Position = new Vec2F(0.3f, 0.3f);
                
                Assert.AreEqual(TaxiCollision.Collision(e1, e2), false);
            }
            
            [Test()]
            public void TopCollision() {
                e1.Shape.Position = new Vec2F(0.1f, 0.1f);
                e2.Shape.Position = new Vec2F(0.1f, 0.2f);
                
                Assert.AreEqual(TaxiCollision.Collision(e1, e2), true);
            }
            
            [Test()]
            public void BottomCollision() {
                e1.Shape.Position = new Vec2F(0.1f, 0.1f);
                e2.Shape.Position = new Vec2F(0.1f, 0.0f);
                
                Assert.AreEqual(TaxiCollision.Collision(e1, e2), true);
            }
            
            [Test()]
            public void RightCollision() {
                e1.Shape.Position = new Vec2F(0.1f, 0.1f);
                e2.Shape.Position = new Vec2F(0.2f, 0.1f);
                
                Assert.AreEqual(TaxiCollision.Collision(e1, e2), true);
            }
            
            [Test()]
            public void LeftCollision() {
                e1.Shape.Position = new Vec2F(0.1f, 0.1f);
                e2.Shape.Position = new Vec2F(0.05f, 0.1f);
                
                Assert.AreEqual(TaxiCollision.Collision(e1, e2), true);
            }
        }
    }
}