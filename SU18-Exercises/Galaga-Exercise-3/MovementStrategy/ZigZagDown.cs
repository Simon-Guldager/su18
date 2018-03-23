using System;
using DIKUArcade.Entities;
using DIKUArcade.Math;
using Galaga_Exercise_3.GalagaEntities;

namespace Galaga_Exercise_3.MovementStrategy {
    public class ZigZagDown : IMovementStrategy {
        const float PI = (float)Math.PI;
        private float speed = -0.0003f;
        
        public void MoveEnemy(Enemy enemy) {
            Vec2F start = enemy.Position;

            enemy.Shape.Position.Y += speed;
            float fraction = (2 * PI * (start.Y - enemy.Shape.Position.Y)) / 0.045f;
            enemy.Shape.Position.X = start.X + 0.05f * (float) Math.Sin(fraction);     
        }
        
        public void MoveEnemies(EntityContainer<Enemy> enemies) {
            enemies.Iterate(MoveEnemy);
        }
    }
}