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
                    UpdateCustomer();
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
        
        /// <summary>
        /// Spawn the next customer, if there is none, the game is over
        /// </summary>
        private void NextCustomer() {
            if (!level.NextCustomer()) {
                win.CloseWindow();   
            }    
        }
        
        /// <summary>
        /// Moves the customer towards the taxi if they are on the same platform.
        /// Then checks for a collision between the taxi and the player.
        /// Note that the customer is never moved when inside the taxi, instead
        /// the customer is simply made invisible.
        /// </summary>
        private void UpdateCustomer() {
            // if the customer is inside the taxi, none of this applies
            if (level.Customer.InFlight) {
                return;    
            }
            // shorthand for public fields.
            var shapeC = level.Customer.Entity.Shape;
            var shapeP = level.Player.Entity;
            
            // if the taxi collides with the customer, make sure the taxi is static
            if (TaxiCollision.Collision(shapeP, level.Customer.Entity)) {
                if (shapeP.Shape.AsDynamicShape().Direction.Y != 0f) {
                    win.CloseWindow();   
                } else {
                    level.Customer.InFlight = true;
                }    
            } 
            // if the taxi is on the platform of a customer, move the customer towards the taxi
            if (level.Player.Platform == level.Customer.Platform) {
                var pos = shapeP.Shape.Position.X;
                if (pos > shapeC.Position.X) {
                    shapeC.Position.X += 0.002f;
                } else {
                    shapeC.Position.X -= 0.002f;
                }     
            }       
        }
        
        /// <summary>
        /// Itterates over the different props and calls relevant methods.
        /// </summary>
        private void ItterateProps() {  
            // since the player object keeps a platform, make sure it is null when airborn
            if (level.Player.InFlight) {
                level.Player.Platform = null;
            }
            foreach (Prop p in level.Props) {
                p.PropSprites.Iterate(delegate(Entity entity) {
                    if (TaxiCollision.Collision(level.Player.Entity, entity)) {
                        switch (p.GetPropType()) {
                            // if the player collides with a platform 
                            case PropType.Platform:
                                var shape = level.Player.Entity.Shape;
                                
                                // the taxi crashes if it lands at too much speed
                                if (shape.AsDynamicShape().Direction.Y < -0.002f) {  
                                    win.CloseWindow(); 
                                }  
                                // othwerwise the taxi is stopped, and moved a tiny bit
                                // above the platform to prevent further collsions
                                level.Player.Stall();
                                shape.Position.Y =
                                    entity.Shape.Position.Y + entity.Shape.Extent.Y + 0.001f;
                                level.Player.Platform = (Platform)p;
                                
                                if (level.Customer.Destination == (Platform)p) {
                                    NextCustomer();    
                                }
                                break;
                            case PropType.Exit:
                                // if the current customer is airborn and the exit is reached
                                // go to the next level
                                if (level.Customer.InFlight && level.Customer.Destination == null) {
                                    NextCustomer();   
                                } else {
                                    win.CloseWindow();
                                }
                                break;
                        }
                    }
                });
            }
        }
        
        /// <summary>
        /// Checks if the taxi has collided with the environment, whereby the game is over.
        /// </summary>
        private void ItterateLevelSprites() {
            level.LevelSprites.Iterate(delegate(Entity entity) {
                if (TaxiCollision.Collision(level.Player.Entity, entity)) {
                    win.CloseWindow(); 
                    level.Player.Stall();
                }
            });
        }
        
        private void RenderProps() {
            foreach (Prop p in level.Props) {
                p.RenderProp();
            }
        }
    }
}
