using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.EventBus;
using DIKUArcade.Physics;
using DIKUArcade.State;
using Galaga_Exercise_3.GalagaEntities;
using Galaga_Exercise_3.MovementStrategy;
using Galaga_Exercise_3.Squadrons;

namespace Galaga_Exercise_3.GalagaStates {
    public class GameRunning : IGameState {
        private static GameRunning instance;
        
        private Entity backGroundImage;
        private Player player;

        private List<Image> enemyStrides;
        private EntityContainer<Enemy> enemies;
        private IMovementStrategy movementStrategy;

        private Image shotStride;
        private EntityContainer playerShots;

        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private int explosionLength = 500;

        public GameRunning() {
            InitializeGameState();
        }
        
        public void AddExplosion(float posX, float posY, float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
        }

        public void ItterateEnemies() {
            if (enemies.CountEntities() == 0) {
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent,
                        this,
                        "CHANGE_STATE",
                        "GAME_WON", ""));
            }
            enemies.Iterate(delegate(Enemy enemy) {
                if (enemy.Position.Y > 1f) {
                    GalagaBus.GetBus().RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.GameStateEvent,
                            this,
                            "CHANGE_STATE",
                            "GAME_LOST", ""));
                }
            });
        }

        private void gameOver() {
            GalagaBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "MAIN_MENU", ""));
        }
        
        public void ItterateShots() {
            playerShots.Iterate(delegate(Entity shot) {
                if (shot.Shape.Position.Y > 1.0) {
                    shot.DeleteEntity();
                }
                
                enemies.Iterate(delegate(Enemy enemy) {
                    var collide = CollisionDetection.Aabb((DynamicShape) shot.Shape, 
                                      enemy.Shape);
                    if (collide.Collision) {
                        shot.DeleteEntity();
                        enemy.DeleteEntity();
                        AddExplosion(enemy.Shape.Position.X, enemy.Shape.Position.Y, 0.1f, 0.1f);
                    }
                });
                
                shot.Shape.Move();
            });
        }

        public void UpdateGameLogic() {
            ItterateShots();
            ItterateEnemies();
            player.Move();
            movementStrategy.MoveEnemies(enemies);
        }

        public void RenderState() {
            backGroundImage.RenderEntity();
            playerShots.RenderEntities();
            enemies.RenderEntities();
            explosions.RenderAnimations();
            player.Entity.RenderEntity(); 
        }

        public void GameLoop() { }
        
        public void InitializeGameState() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));

            player = new Player();

            enemies = new EntityContainer<Enemy>();
            enemyStrides = ImageStride.CreateStrides(4,
                Path.Combine("Assets", "Images", "BlueMonster.png"));

            var sqr = new Squadron2();
            sqr.CreateEnemies(enemyStrides);
            enemies = sqr.Enemies;
            movementStrategy = new ZigZagDown();

            playerShots = new EntityContainer();
            shotStride = new Image(Path.Combine("Assets", "Images", "BulletRed2.png"));

            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(4);
        }
        
        public void KeyPress(string key) {
            switch(key) {
                case "KEY_SPACE":
                    playerShots.AddDynamicEntity(
                        new DynamicShape(player.Entity.Shape.Position + new Vec2F(0.046f, 0.09f), 
                            new Vec2F(0.008f, 0.027f), new Vec2F(0.0f, 0.01f)),
                        shotStride);
                    break;
                case "KEY_RIGHT":
                    player.MoveRight();
                    break;
                case "KEY_LEFT":
                    player.MoveLeft();
                    break;
                case "KEY_ESCAPE":
                    GalagaBus.GetBus().RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.GameStateEvent,
                            this,
                            "CHANGE_STATE",
                            "GAME_PAUSED", ""));
                    break;
                }
        }

        public void KeyRelease(string key) {
            switch(key) {
                case "KEY_RIGHT":
                    player.MoveLeft();
                    break;
                case "KEY_LEFT":
                    player.MoveRight();
                    break;
                }
        }
        
        public void HandleKeyEvent(string keyValue, string keyAction) {
            switch (keyAction) {
            case "KEY_PRESS":
                KeyPress(keyValue);
                break;
            case "KEY_RELEASE":
                KeyRelease(keyValue);
                break;
            }
        }
        
        public static GameRunning NewInstance() {
            return instance = new GameRunning();
        }
        
        public static GameRunning GetInstance() {
            return instance ?? (instance =
                       new GameRunning());
        }
    }
}