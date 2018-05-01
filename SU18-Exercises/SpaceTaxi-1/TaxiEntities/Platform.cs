using System;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_1.TaxiEntities {
    public class Platform : Prop {
        public Player Player;
        
        public Platform() {
            PropSprites = new EntityContainer();        
        }
        
        public override PropType GetPropType() {
            return PropType.Platform;
        }
    }
}