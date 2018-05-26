using DIKUArcade.Entities;

namespace SpaceTaxi_3 {
    public class Physics {
        public static bool Collision(Shape shape, Shape actor) {
            var px1 = actor.Position.X; 
            var py1 = actor.Position.Y;
            var px2 = actor.Position.X + actor.Extent.X;
            var py2 = actor.Position.Y + actor.Extent.Y;
            
            var ex1 = shape.Position.X;
            var ey1 = shape.Position.Y;
            
            // divide by 2 to make some navigaton easier, can be really teadious without
            var ex2 = shape.Position.X + shape.Extent.X;
            var ey2 = shape.Position.Y + shape.Extent.Y;
            return ((px1 <= ex1 && px2 >= ex2 && py1 <= ey2 && py2 >= ey2)
                    || (px1 <= ex1 && px2 >= ex2 && py2 >= ey1 && py1 <= ey2)
                    || (px2 >= ex1 && px1 <= ex2 && py2 >= ey1 && py1 <= ey2));
        }
    }
}