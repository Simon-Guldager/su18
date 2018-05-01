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
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.03f, 0.06f));
            customerStandRight = new Image(
                Path.Combine("Assets", "Images", "CustomerStandRight.png"));
            Entity = new Entity(shape, customerStandRight);
            
            Platform = dest;

        }
        
        public void RenderCustomer() {
            if (!InFlight) {
                Entity.RenderEntity();     
            }     
        }
    }
}