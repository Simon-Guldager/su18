using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace SpaceTaxi_1.TaxiEntities {
    public enum PropType {
        Exit,
        Platform
    }
    
    public abstract class Prop {
        public EntityContainer PropSprites;

        public abstract PropType GetPropType();
        
        public virtual void RenderProp() {
            PropSprites.RenderEntities();    
        }
        
        /// <summary>
        /// Itterates all entities in a given prop, and finds the leftmost entity, usefull
        /// when placing customers.
        /// </summary>
        public Vec2F GetPosition() {
            var pos = new Vec2F(1f, 1f);
            
            PropSprites.Iterate(delegate(Entity entity) {
                var x1 = entity.Shape.Position.X;
                var y1 = entity.Shape.Position.Y;
                
                if (x1 < pos.X) {
                    pos.X = x1;
                }
                if (y1 < pos.Y) {
                    pos.Y = y1;
                }
            });
            return pos;
        }
    }
}