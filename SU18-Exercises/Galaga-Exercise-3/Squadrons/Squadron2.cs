using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using Galaga_Exercise_3.GalagaEntities;

namespace Galaga_Exercise_3.Squadrons {
    public class Squadron2 {
        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        
        public Squadron2() {
            MaxEnemies = 16;
            Enemies = new EntityContainer<Enemy>();
        }
        
        public void CreateEnemies(List<Image> enemyStrides) {
            for (int j = 8; j < 10; j++) {
                for (int i = 1; i < 9; i++) {
                    var shape = new StationaryShape(new Vec2F(i * 0.1f, j * 0.1f), 
                                                    new Vec2F(0.1f, 0.1f));
                    Enemies.AddDynamicEntity(new Enemy(shape,
                                             new ImageStride(80, enemyStrides)));
                }   
            }
        }
    }
}