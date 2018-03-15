using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using Galaga_Exercise_2.GalagaEntities;

namespace Galaga_Exercise_2.Squadrons {
    public class Squadron4 {
        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        
        public Squadron4() {
            MaxEnemies = 22;
            Enemies = new EntityContainer<Enemy>();
        }
        
        public void CreateEnemies(List<Image> enemyStrides) {
            for (int j = 7; j < 9; j++) {
                for (int i = 1; i < 9; i++) {
                    var shape = new StationaryShape(new Vec2F(i * 0.1f, j * 0.1f), 
                        new Vec2F(0.1f, 0.1f));
                    Enemies.AddDynamicEntity(new Enemy(shape,
                        new ImageStride(80, enemyStrides)));
                }   
            }
            
            for (int i = 2; i < 8; i++) {
                var shape = new StationaryShape(new Vec2F(i * 0.1f, 0.9f), 
                                                new Vec2F(0.1f, 0.1f));
                Enemies.AddDynamicEntity(new Enemy(shape,
                                         new ImageStride(80, enemyStrides)));
            }  
        }
    }
}