using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_1.TaxiEntities {
    public class Customer {
        public Entity Entity;
        public Platform Platform { get; private set; }
        public Platform Destination;
        public bool InFlight;
        
        private readonly Shape shape;
        private readonly Image customerStandRight;
        
        public Customer(Platform dest) {
            InFlight = false;
            
            // the customer only has one image on current version
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.03f, 0.06f));
            customerStandRight = new Image(
                Path.Combine("Assets", "Images", "CustomerStandRight.png"));
            Entity = new Entity(shape, customerStandRight);
            
            Platform = dest;
        }
        
        public void RenderCustomer() {
            // only render the customer if she's on the ground
            if (!InFlight) {
                Entity.RenderEntity();     
            }     
        }
    }
}