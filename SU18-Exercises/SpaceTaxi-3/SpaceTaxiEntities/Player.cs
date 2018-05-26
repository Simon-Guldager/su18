using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_3.SpaceTaxiEntities
{
    public class Player : IGameEventProcessor<object> {
        
        public bool PlayerIsMoving;
        public DynamicShape Shape { get; private set; }

        private float accelerationY = 0;
        private float accelerationX = 0;
        
        private TaxiOrientation orientation;
        private TaxiBoosterState state;
        
        
        public Player() {
            orientation = TaxiOrientation.TaxiOrientedLeft;
            state = TaxiBoosterState.TaxiBoosterOff;
            
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, this);
            
            Shape = new DynamicShape(new Vec2F(), new Vec2F(0.0625f, 0.045f));

            PlayerIsMoving = true;
        }

        public void Move() {
            if (PlayerIsMoving) {
                Shape.Direction.Y += accelerationY + Constants.GRAVITY;
                Shape.Direction.X += accelerationX; 
                Shape.Move();
            }
        }

        public void StopMoving() {
            Shape.Direction.Y = 0;
            Shape.Direction.X = 0;
            PlayerIsMoving = false;
        }

        public void RenderPlayer() {
            var image = ImageParser.GetInstance().GetPlayerImage(orientation, state);
            image.Render(Shape);
        }

        private void StartAccelerationY() {
            PlayerIsMoving = true;
            accelerationY = Constants.PLAYER_ACCELERATION_CONST_Y;
            state = (accelerationX == 0)
                ? TaxiBoosterState.TaxiBoosterUp
                : TaxiBoosterState.TaxiBoosterUpAndHorizontal;
        }

        private void StopAccelerationY() {
            accelerationY = 0f;
            state = (accelerationX == 0)
                ? TaxiBoosterState.TaxiBoosterOff
                : TaxiBoosterState.TaxiBoosterHorizontal;
        }

        private void StartAccelerationX() {
            state = (accelerationY == 0)
                ? TaxiBoosterState.TaxiBoosterHorizontal
                : TaxiBoosterState.TaxiBoosterUpAndHorizontal;
        }
        
        private void StopAccelerationX() {
            accelerationX = 0f;
            state = (accelerationY == 0)
                ? TaxiBoosterState.TaxiBoosterOff
                : TaxiBoosterState.TaxiBoosterUp;    
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message)
                {
                    case "BOOSTER_UPWARDS":
                        StartAccelerationY();
                        break;
                    case "STOP_ACCELERATE_UP":
                        StopAccelerationY();
                        break;
                    case "BOOSTER_TO_LEFT":
                        accelerationX = -Constants.PLAYER_ACCELERATION_CONST_X;
                        orientation = TaxiOrientation.TaxiOrientedLeft;
                        StartAccelerationX();
                        break;
                    case "STOP_ACCELERATE_LEFT":
                        StopAccelerationX();
                        break;
                    case "BOOSTER_TO_RIGHT":
                        accelerationX = Constants.PLAYER_ACCELERATION_CONST_X;
                        orientation = TaxiOrientation.TaxiOrientedRight;
                        StartAccelerationX();
                        break;
                    case "STOP_ACCELERATE_RIGHT":
                        StopAccelerationX();   
                        break;
                }
            }
        }

        private ImageStride CreateStride(string fileName) {
            var strides = ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", fileName));
            return new ImageStride(40,strides);
        }
    }
}
