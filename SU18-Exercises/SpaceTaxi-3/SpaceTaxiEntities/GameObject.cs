using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using DIKUArcade.Math;

namespace SpaceTaxi_3.SpaceTaxiEntities {
    public abstract class GameObject {
        
        public EntityContainer GameObjectEntities = new EntityContainer();
        
        public StationaryShape BoundingBox { get; } = new StationaryShape(
            new Vec2F(1f, 1f), 
            new Vec2F(0.0f, 0.0f)                  
        );

        public virtual void RenderObject() {
            GameObjectEntities.RenderEntities();
        }
        
        public void AddEntity(StationaryShape shape, IBaseImage image) {
            // add to entities
            GameObjectEntities.AddStationaryEntity(shape, image);

            // update the Platform bounding box
            if (shape.Position.X < BoundingBox.Position.X) {
                BoundingBox.Position.X = shape.Position.X;
            }
            if (shape.Position.Y < BoundingBox.Position.Y) {
                BoundingBox.Position.Y = shape.Position.Y;
            }
            if (shape.Position.X + shape.Extent.X >
                BoundingBox.Position.X + BoundingBox.Extent.X) {
                BoundingBox.Extent.X =
                    (shape.Position.X + shape.Extent.X) - BoundingBox.Position.X;
            }
            if (shape.Position.Y + shape.Extent.Y >
                BoundingBox.Position.Y + BoundingBox.Extent.Y) {
                BoundingBox.Extent.Y =
                    (shape.Position.Y + shape.Extent.Y) - BoundingBox.Position.Y;
            }
        }
        
        public bool CheckCollision(DynamicShape actor) {
            return Physics.Collision(actor, BoundingBox);
        }
    }
}