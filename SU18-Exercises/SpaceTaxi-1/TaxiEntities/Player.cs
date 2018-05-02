using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;

namespace SpaceTaxi_1.TaxiEntities
{
    public class Player : IGameEventProcessor<object> {
        public Platform Platform;
        public bool InFlight;
        public Entity Entity { get; private set; }
        
        private readonly DynamicShape shape;
        private readonly Image taxiBoosterOffImageLeft;
        private readonly Image taxiBoosterOffImageRight;
        private Orientation taxiOrientation;

        private float accelerationDown = 0f;
        private float accelerationLeftRight = 0f;
        private float gravity = -0.0001f;

        public Player() {   
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.0625f, 0.045f));
            taxiBoosterOffImageLeft = new Image(
                Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
            taxiBoosterOffImageRight = new Image(
                Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));

            Entity = new Entity(shape, taxiBoosterOffImageLeft);
            taxiOrientation = Orientation.Right;

            InFlight = false;
        }

        public void Move() {
            if (InFlight) {
                shape.Direction.Y += accelerationDown + gravity;
                shape.Direction.X += accelerationLeftRight; 
                shape.Move();
            }
        }

        public void Stall() {
            shape.Direction.Y = 0;
            shape.Direction.X = 0;
            InFlight = false;
        }

        public void RenderPlayer() {
            Entity.Image = taxiOrientation == Orientation.Left
                ? taxiBoosterOffImageLeft
                : taxiBoosterOffImageRight;
            Entity.RenderEntity();
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                if (!InFlight) {
                    InFlight = true;
                }
                switch (gameEvent.Message)
                {
                    case "BOOSTER_UPWARDS":
                        accelerationDown = 0.0003f;
                        break;
                    case "STOP_ACCELERATE_UP":
                        accelerationDown = 0f;
                        break;
                    case "BOOSTER_TO_LEFT":
                        taxiOrientation = Orientation.Left;
                        accelerationLeftRight = -0.00006f;
                        break;
                    case "STOP_ACCELERATE_LEFT":
                        accelerationLeftRight = 0f;
                        break;
                    case "BOOSTER_TO_RIGHT":
                        taxiOrientation = Orientation.Right;
                        accelerationLeftRight = 0.00006f;
                        break;
                    case "STOP_ACCELERATE_RIGHT":
                        accelerationLeftRight = 0f;
                        break;
                }
            }
        }
        
    }
}
