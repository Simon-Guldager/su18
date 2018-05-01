using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace SpaceTaxi_1.TaxiEntities {
    public class Platform : Prop {
        public Player Player;

        public override PropType GetPropType() {
            return PropType.Platform;
        }

        public Platform() {
            PropSprites = new EntityContainer();        
        }
    }
}