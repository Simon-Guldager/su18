using System;
using DIKUArcade.Entities;

namespace SpaceTaxi_1.TaxiEntities {
    public class Exit : Prop {
        public override PropType GetPropType() {
            return PropType.Exit;
        }

        public Exit() {
            PropSprites = new EntityContainer();   
        }
    }
}