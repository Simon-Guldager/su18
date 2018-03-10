using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using Galaga_Exercise_2.GalagaEntities;

namespace Galaga_Exercise_2.Squadrons {
    public class Squadron1 : ISquadron {
        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        private Enemy enemy;
        
        public Squadron1(int maxEnemies) {
            MaxEnemies = maxEnemies;
        }
        
        public void CreateEnemies(List<Image> enemyStrides) {
            for (int i = 1; i < 9; i++) {
                //var shape = new StationaryShape(new Vec2F(i * 0.1f, 0.9f), new Vec2F(0.1f, 0.1f));
                enemy = new Enemy(new StationaryShape(new Vec2F(i * 0.1f, 0.9f), new Vec2F(0.1f, 0.1f)), 
                                        new ImageStride(80, enemyStrides));
                var lel = enemy.Position;
                Enemies.AddStationaryEntity(enemy);
            }
        }
    }
}