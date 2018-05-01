using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi_1.TaxiEntities;

namespace SpaceTaxi_1
{
    public class Game : IGameEventProcessor<object> {
        private Window win;
        private GameEventBus<object> eventBus;
        private GameTimer gameTimer;
        private LevelParser level;
        
        private readonly Entity backGroundImage;

        public Game() {
            // window
            win = new Window("Space Taxi Game v0.1", 800, AspectRatio.R1X1);
            
            // level class
            level = new LevelParser();  

            // event bus
            eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window, e.g. CloseWindow()
                GameEventType.PlayerEvent // commands issued to the player object, e.g. move, destroy, receive health, etc.
            });
            win.RegisterEventBus(eventBus);

            // game timer
            gameTimer = new GameTimer(60); // 60 UPS, no FPS limit

            // game assets
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png"))
            );
            
            // event delegation
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);
            eventBus.Subscribe(GameEventType.PlayerEvent, level.Player);  
            
        }

        public void GameLoop() {
            while (win.IsRunning()) {
                gameTimer.MeasureTime();

                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();
                    eventBus.ProcessEvents();
                    
                    level.Player.Move();
                    MoveCustomer();
                    ItterateProps();
                    ItterateLevelSprites();
                }

                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    
                    backGroundImage.RenderEntity();
                    level.Player.RenderPlayer();
                    level.Customer.RenderCustomer();
                    level.LevelSprites.RenderEntities();
                    RenderProps();

                    win.SwapBuffers();
                }

                if (gameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps from the timer
                    win.Title = "Space Taxi | UPS: " + gameTimer.CapturedUpdates + ", FPS: " +
                                 gameTimer.CapturedFrames;
                }
            }
        }

        public void KeyPress(string key) {
            switch (key) {
                case "KEY_ESCAPE":
                    win.CloseWindow();
                    break;
                case "KEY_F12":
                    Console.WriteLine("Saving screenshot");
                    win.SaveScreenShot();
                    break;
                case "KEY_UP":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "BOOSTER_UPWARDS", "", ""));
                    break;
                case "KEY_LEFT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "BOOSTER_TO_LEFT", "", ""));
                    break;
                case "KEY_RIGHT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "BOOSTER_TO_RIGHT", "", ""));
                    break;
            }
        }

        public void KeyRelease(string key) {
            switch (key) {
                case "KEY_LEFT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "STOP_ACCELERATE_LEFT", "", ""));
                    break;
                case "KEY_RIGHT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "STOP_ACCELERATE_RIGHT", "", ""));
                    break;
                case "KEY_UP":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.PlayerEvent, this, "STOP_ACCELERATE_UP", "", ""));
                    break;
            }
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                    case "CLOSE_WINDOW":
                        win.CloseWindow();
                        break;
                    default:
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

        private void MoveCustomer() {
            if (level.Customer.InFlight) {
                return;    
            }
            var shape = level.Customer.Entity.Shape;
            if (TaxiCollision.Collision(level.Player.Entity, level.Customer.Entity)) {
                if (level.Player.Entity.Shape.AsDynamicShape().Direction.Y != 0f) {
                    win.CloseWindow();   
                } else {
                    level.Customer.InFlight = true;
                }    
            }
            if (level.Player.Platform == level.Customer.Platform) {
                var pos = level.Player.Entity.Shape.Position.X;
                if (pos > shape.Position.X) {
                    shape.Position.X += 0.002f;
                } else {
                    shape.Position.X -= 0.002f;
                }     
            }       
        }
        
        private void RenderProps() {
            foreach (Prop p in level.Props) {
                p.RenderProp();
            }
        }

        private void ItterateProps() {           
            if (level.Player.InFlight) {
                level.Player.Platform = null;
            }
            foreach (Prop p in level.Props) {
                p.PropSprites.Iterate(delegate(Entity entity) {
                    if (TaxiCollision.Collision(level.Player.Entity, entity)) {
                        switch (p.GetPropType()) {
                            case PropType.Platform:
                                var shape = level.Player.Entity.Shape;
                                if (shape.AsDynamicShape().Direction.Y < -0.002f) {  
                                    win.CloseWindow(); 
                                }  
                                level.Player.Stall();
                                shape.Position.Y =
                                    entity.Shape.Position.Y + entity.Shape.Extent.Y + 0.001f;
                                level.Player.Platform = (Platform)p;
                                if (level.Customer.Destination == (Platform)p) {
                                    NextCustomer();    
                                }
                                break;
                            case PropType.Exit:
                                if (level.Customer.Destination == null) {
                                    NextCustomer();    
                                }
                                break;
                        }
                    }
                });
            }
        }

        private void ItterateLevelSprites() {
            level.LevelSprites.Iterate(delegate(Entity entity) {
                if (TaxiCollision.Collision(level.Player.Entity, entity)) {
                    win.CloseWindow(); 
                    level.Player.Stall();
                }
            });
        }

        private void NextCustomer() {
            if (!level.NextCustomer()) {
                win.CloseWindow();   
            }    
        }
    }
}
