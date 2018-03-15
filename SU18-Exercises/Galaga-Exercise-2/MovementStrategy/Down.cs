using DIKUArcade.Entities;
using Galaga_Exercise_2.GalagaEntities;

namespace Galaga_Exercise_2.MovementStrategy {
    public class Down : IMovementStrategy {
        private float speed = -0.0003f;
        
        public void MoveEnemy(Enemy enemy) {
            enemy.Shape.Position.Y += speed;
        }
        
        public void MoveEnemies(EntityContainer<Enemy> enemies) {
            enemies.Iterate(MoveEnemy);
        }
    }
}