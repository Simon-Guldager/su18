﻿using DIKUArcade.Entities;

namespace SpaceTaxi_2 {
    public class TaxiCollision {
        public static bool Collision(Entity actor, Entity entity) {
            var px1 = actor.Shape.Position.X; 
            var py1 = actor.Shape.Position.Y;
            var px2 = actor.Shape.Position.X + actor.Shape.Extent.X;
            var py2 = actor.Shape.Position.Y + actor.Shape.Extent.Y;
            
            var ex1 = entity.Shape.Position.X;
            var ey1 = entity.Shape.Position.Y;
            
            // divide by 2 to make some navigaton easier, can be really teadious without
            var ex2 = entity.Shape.Position.X + entity.Shape.Extent.X / 2;
            var ey2 = entity.Shape.Position.Y + entity.Shape.Extent.Y;
            return ((px1 <= ex1 && px2 >= ex2 && py1 <= ey2 && py2 >= ey2)
                    || (px1 <= ex1 && px2 >= ex2 && py2 >= ey1 && py1 <= ey2)
                    || (px2 >= ex1 && px1 <= ex2 && py2 >= ey1 && py1 <= ey2));
        }
    }
}