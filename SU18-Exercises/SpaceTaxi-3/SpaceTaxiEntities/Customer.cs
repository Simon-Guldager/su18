using System;
using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace SpaceTaxi_3.SpaceTaxiEntities {
    public class Customer {
        
        public string Name { get; private set; }
        public DynamicShape Shape;
        public Platform Origin;
        public char Destination;

        public bool CustomerInTransport;
        public bool DestinationOnNewLevel;
        
        private SpaceTaxi_3.CustomerOrientation orientation;
        private SpaceTaxi_3.CustomerState state;

        public bool DestinationReached;
        
        public int SpawnTimer;
        public int DesapwnTimer;

        public bool ToBeDeleted;
        
        public Customer(string name) {
            DestinationReached = false;
            CustomerInTransport = false;
            ToBeDeleted = false;
            
            orientation = CustomerOrientation.CustomerOrientedRight;
            state = CustomerState.CustomerStateStanding;
            
            Shape = new DynamicShape(new Vec2F(), new Vec2F(0.06f, 0.04f));
        }

        public void RenderCustomer() {
            if (!CustomerInTransport) {
                var image = ImageParser.GetInstance().GetCustomerImage(orientation, state);
                image.Render(Shape);    
            }
        }

        public void Update() {
            if (ToBeDeleted) {
                return;    
            }
            
            var shape = Origin.Player != null && !DestinationReached
                ? Origin.Player.Shape.AsStationaryShape()
                : Origin.BoundingBox;
            
            if (Shape.Position.X < shape.Position.Copy().X) {
                StartMoveRight();     
            } else if (Shape.Position.X > shape.Position.Copy().X + shape.Extent.Copy().X - Constants.WIDTH) {  
                StartMoveLeft();   
            }    

            Shape.Move();
        }

        public void ResetPosition() { 
            CustomerInTransport = false;
            Shape.Position.X = Origin.BoundingBox.Position.Copy().X;
            Shape.Position.Y = Origin.BoundingBox.Position.Copy().Y + 1 / 26f;
            StartMoveRight();
        }
        
        public void StartMoveLeft() {
            CheckToBeDeleted();
            
            state = CustomerState.CustomerStateWalking;
            orientation = CustomerOrientation.CustomerOrientedLeft;
            Shape.Direction.X = -Constants.CUSTOMER_VELOCITY_X;
        }

        public void StartMoveRight() {
            CheckToBeDeleted();
            
            state = CustomerState.CustomerStateWalking;
            orientation = CustomerOrientation.CustomerOrientedRight;
            Shape.Direction.X = Constants.CUSTOMER_VELOCITY_X;
        }
        
        public void StopMoving() {
            state = CustomerState.CustomerStateStanding;
        }

        private void CheckToBeDeleted() {
            if (DestinationReached) {
                ToBeDeleted = true;
                StopMoving();
            }    
        }
    }
}