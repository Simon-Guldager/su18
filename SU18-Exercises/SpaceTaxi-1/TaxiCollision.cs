using DIKUArcade.Entities;

namespace SpaceTaxi_1 {
    public class TaxiCollision {
        public static bool Collision(Entity actor, Entity entity) {
            var px1 = actor.Shape.Position.X; 
            var py1 = actor.Shape.Position.Y;
            var px2 = actor.Shape.Position.X + actor.Shape.Extent.X;
            var py2 = actor.Shape.Position.Y + actor.Shape.Extent.Y;
            
            var ex1 = entity.Shape.Position.X;
            var ey1 = entity.Shape.Position.Y;
            var ex2 = entity.Shape.Position.X + entity.Shape.Extent.X / 2f;
            var ey2 = entity.Shape.Position.Y + entity.Shape.Extent.Y;
            return ((px1 <= ex1 && px2 >= ex2 && py1 <= ey2 && py2 >= ey2)
                    || (px1 <= ex1 && px2 >= ex2 && py2 >= ey1 && py1 <= ey2)
                    || (px2 >= ex1 && px1 <= ex2 && py2 >= ey1 && py1 <= ey2));
        }
    }
}