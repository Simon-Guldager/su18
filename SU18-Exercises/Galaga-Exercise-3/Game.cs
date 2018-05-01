using System.Collections.Generic;
using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Timers;
using Galaga_Exercise_3.GalagaStates;

namespace Galaga_Exercise_3 {
    public class Game : IGameEventProcessor<object>{
        private GameTimer gameTimer;
        private Window win;
        
        private GameEventBus<object> eventBus;
        private StateMachine stateMachine;
        
        public Game() {
            gameTimer = new GameTimer();
            win = new Window("Galaca", 500, AspectRatio.R1X1);
            
            // eventBus for various GameEvents
            eventBus = GalagaBus.GetBus();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.GameStateEvent,
                GameEventType.InputEvent, 
                GameEventType.WindowEvent, 
                GameEventType.PlayerEvent 
            }); 
            
            // The eventBus is linked to the window
            win.RegisterEventBus(eventBus);
            eventBus.Subscribe(GameEventType.WindowEvent, this);   
            
            stateMachine = new StateMachine();
        }

        public void GameLoop() {
            while (win.IsRunning()) { 
                gameTimer.MeasureTime();
                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();
                    eventBus.ProcessEvents();
                    stateMachine.ActiveState.UpdateGameLogic();
                }
                
                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    stateMachine.ActiveState.RenderState(); 
                    win.SwapBuffers();
                }    
                
                if (gameTimer.ShouldReset()) {
                    win.Title = "Galaga | UPS: " + gameTimer.CapturedUpdates +
                                ", FPS: " + gameTimer.CapturedFrames;
                }
            }
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                    case "CLOSE_WINDOW":
                        win.CloseWindow();
                        break;
                    }
            } 
        }
    }
}