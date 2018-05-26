using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi_3.SpaceTaxiEntities;
using SpaceTaxi_3.SpaceTaxiStates;

namespace SpaceTaxi_3
{
    public class Game : IGameEventProcessor<object> {
        private Window win;
        private GameEventBus<object> eventBus;
        private GameTimer gameTimer;
        private StateMachine stateMachine;

        public Game() {
            // window
            win = new Window("Space Taxi Game v0.1", 800, AspectRatio.R1X1);
            
            // event bus
            eventBus = SpaceTaxiBus.GetBus();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window, e.g. CloseWindow()
                GameEventType.PlayerEvent, // commands issued to the player object, e.g. move, destroy, receive health, etc.
                GameEventType.GameStateEvent, // command for switching between states
                GameEventType.TimedEvent
            });
            win.RegisterEventBus(eventBus);
                        
            // game timer
            gameTimer = new GameTimer(60); // 60 UPS, no FPS limit
            
            stateMachine = new StateMachine();
            
            eventBus.Subscribe(GameEventType.WindowEvent, this);
        }

        public void GameLoop() {
            while (win.IsRunning()) {
                gameTimer.MeasureTime();

                win.PollEvents();
                
                eventBus.ProcessEventsSequentially();
                
                stateMachine.ActiveState.UpdateGameLogic();
                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    
                    stateMachine.ActiveState.RenderState();
        
                    win.SwapBuffers();
                }
                if (gameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps from the timer
                    win.Title = "Space Taxi | UPS: " + gameTimer.CapturedUpdates + ", FPS: " +
                                 gameTimer.CapturedFrames;
                }
            }
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                    case "CLOSE_WINDOW":
                        win.CloseWindow();
                        break;
                    case "SAVE_SCREENSHOT":
                        Console.WriteLine("Saving screenshot");
                        win.SaveScreenShot();
                        break;
                }
            } 
        }
    }
}
