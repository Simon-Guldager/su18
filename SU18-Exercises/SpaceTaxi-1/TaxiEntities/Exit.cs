using System;
using DIKUArcade.Entities;

namespace SpaceTaxi_1.TaxiEntities {
    public class Exit : Prop {
        public Exit() {
            PropSprites = new EntityContainer();   
        }
        
        public override PropType GetPropType() {
            return PropType.Exit;
        }

    }
}