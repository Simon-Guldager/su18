using System.Net.Mime;
using DIKUArcade.Graphics;
using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace Galaga_Exercise_2.GalagaEntities {
    public class Enemy : Entity {
        public Vec2F Position { get; }

        public Enemy(StationaryShape shape, IBaseImage image) : base(shape, image) {
            Position = shape.Position.Copy();
        }
    }
}