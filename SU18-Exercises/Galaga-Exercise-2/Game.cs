using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.EventBus;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using Galaga_Exercise_2.GalagaEntities;
using Galaga_Exercise_2.Squadrons;

namespace Galaga_Exercise_2 {
    public class Game : IGameEventProcessor<object>{
        private GameEventBus<object> eventBus;
        private Window win;
        private Image backGround;
        private Player player;
        private GameTimer gameTimer;
        
        private List<Image> enemyStrides;
        private EntityContainer<Enemy> enemies;
        
        private Image shotStride;
        private EntityContainer playerShots;
        
        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private int explosionLength = 500;
        
        public Game() {
            gameTimer = new GameTimer(60, 60);
            win = new Window("Galaca", 500, AspectRatio.R1X1);
            backGround = new Image(Path.Combine("Assets", "Images", "SpaceBackground.png"));
            player = new Player();
    
            enemies = new EntityContainer<Enemy>();
            enemyStrides = ImageStride.CreateStrides(4,
                Path.Combine("Assets", "Images", "BlueMonster.png"));
            //AddEnemies();
            var sqr = new Squadron1(9);
            sqr.CreateEnemies(enemyStrides);
            //enemies = sqr.Enemies;
            
            playerShots = new EntityContainer();
            shotStride = new Image(Path.Combine("Assets", "Images", "BulletRed2.png"));
            
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(4);
            
            eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.InputEvent, 
                GameEventType.WindowEvent, 
                GameEventType.PlayerEvent 
            }); 
            
            win.RegisterEventBus(eventBus);
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);   
        }
        
        public void GameLoop() {
            while (win.IsRunning()) { 
                win.PollEvents();
                gameTimer.MeasureTime();

                while (gameTimer.ShouldUpdate()) {
                    eventBus.ProcessEvents();
                    ItterateShots();
                    player.Move();
                }
                
                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    backGround.Render(new StationaryShape(
                        new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)));
                    playerShots.RenderEntities();
                    enemies.RenderEntities();
                    explosions.RenderAnimations();
                    player.Entity.RenderEntity(); 
                    win.SwapBuffers();
                }
                
                if (gameTimer.ShouldReset()) {
                    win.Title = "Galaga | UPS: " + gameTimer.CapturedUpdates +
                                ", FPS: " + gameTimer.CapturedFrames;
                }
            }
        }
        
        public void AddExplosion(float posX, float posY, float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
        }
        
        //public void AddEnemies() {
        //       for (int i = 1; i < 9; i++) {
        //        var shape = new DynamicShape(new Vec2F(i * 0.1f, 0.9f), new Vec2F(0.1f, 0.1f));
        //        enemies.AddDynamicEntity(shape, new ImageStride(80, enemyStrides));
        //    }
        //}

        public void ItterateShots() {
            playerShots.Iterate(delegate(Entity shot) {
                if (shot.Shape.Position.Y > 1.0) {
                    shot.DeleteEntity();
                }
                
                enemies.Iterate(delegate(Enemy enemy) {
                    var collide = CollisionDetection.Aabb((
                        DynamicShape) shot.Shape, (DynamicShape) enemy.Shape);
                    if (collide.Collision) {
                        shot.DeleteEntity();
                        enemy.DeleteEntity();
                        AddExplosion(enemy.Shape.Position.X, enemy.Shape.Position.Y, 0.1f, 0.1f);
                    }
                });
                
                shot.Shape.Move();
            });
        }
        
        public void KeyPress(string key) {
            switch(key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
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
            }
        }

        public void KeyRelease(string key) {
            switch(key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
            case "KEY_RIGHT":
                player.MoveLeft();
                break;
            case "KEY_LEFT":
                player.MoveRight();
                break;
            }
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                }
            } else if (eventType == GameEventType.InputEvent) {
                switch (gameEvent.Parameter1) {
                case "KEY_PRESS":
                    KeyPress(gameEvent.Message);
                    break;
                case "KEY_RELEASE":
                    KeyRelease(gameEvent.Message);
                    break;
                }
            }
        }
    }
}