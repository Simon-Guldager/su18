using System;
using System.Reflection;
using NUnit.Framework;
using DIKUArcade;
using DIKUArcade.EventBus;
using SpaceTaxi_1;
using SpaceTaxi_1.TaxiEntities;

namespace SpaceTaxiTests {
    [TestFixture()]
    public class TestPlayer {
        private Player player;
        
        [SetUp()]
        public void Setup()
        {
            // win is closed when all tests are done
            var win = new Window("Space Taxi Game v0.1", 800, AspectRatio.R1X1);
            player = new Player();
        }
        
        [Test()]
        public void TestMoveRight() {
            var e =  new GameEvent<object>();
            e.Message = "BOOSTER_TO_RIGHT";
            
            player.ProcessEvent(GameEventType.PlayerEvent, e);
            player.Move();
            Assert.Greater(player.Entity.Shape.AsDynamicShape().Direction.X, 0f);
        }
        
        [Test()]
        public void TestMoveLeft() {
            var e =  new GameEvent<object>();
            e.Message = "BOOSTER_TO_LEFT";
            
            player.ProcessEvent(GameEventType.PlayerEvent, e);
            player.Move();
            Assert.Less(player.Entity.Shape.AsDynamicShape().Direction.X, 0f);
        }
        
        [Test()]
        public void TestMoveUp() {
            var e =  new GameEvent<object>();
            e.Message = "BOOSTER_UPWARDS";
            
            player.ProcessEvent(GameEventType.PlayerEvent, e);
            player.Move();
            Assert.Greater(player.Entity.Shape.AsDynamicShape().Direction.Y, 0f);
        }
    }
}