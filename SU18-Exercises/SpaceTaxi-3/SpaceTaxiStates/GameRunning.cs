using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.State;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi_3.SpaceTaxiEntities;
using SpaceTaxi_3.SpaceTaxiLevels;


namespace SpaceTaxi_3.SpaceTaxiStates {
    public class GameRunning : IGameState {
        private static GameRunning instance;

        private GameEventBus<object> eventBus;
        
        private Entity backGroundImage;
        private BottomPanel bottomPanel;

        private int points;

        public GameRunning() {
            InitializeGameState();
        }

        public static void Restart() {
            instance = new GameRunning();
        }
        
        public static GameRunning GetInstance() {
            return instance ?? (instance = new GameRunning());
        }
        
        public void UpdateGameLogic() {
            SpaceTaxiEventContainer.GetContainer().ProcessTimedEvents();

            LevelContainer.GetInstance().ActiveLevel.UpdateLevel();
        }

        public void RenderState() {
            backGroundImage.RenderEntity();
            
            LevelContainer.GetInstance().ActiveLevel.RenderLevel();
            bottomPanel.Render();
        }
        
        public void GameLoop() { }

        public void InitializeGameState() {
            bottomPanel = new BottomPanel();
            
            eventBus = SpaceTaxiBus.GetBus();
            
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f + Constants.BOTTOM), new Vec2F(1.0f, 1.0f - Constants.BOTTOM)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png"))
            );
        }
        
        public void KeyPress(string key) {
            switch (key) {
                case "KEY_ESCAPE":
                    StaticTimer.PauseTimer();
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.GameStateEvent, this, "CHANGE_STATE", "GAME_PAUSED", ""));  
                    break;
                case "KEY_F12":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.WindowEvent, this, "SAVE_SCREENSHOT", "", ""));
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

        public void HandleTimedEvent(string msg, string par1, string par2) {
            switch (msg) {
                case "SPAWN_CUSTOMER":
                    LevelContainer.GetInstance().ActiveLevel.SpawnCustomer();
                    break;
                case "DESPAWN_CUSTOMER":
                    LevelContainer.GetInstance().ActiveLevel.DespawnCustomer();
                    break;
                case "DELETE_OLD_CUSTOMER":
                    LevelContainer.GetInstance().ActiveLevel.DeleteOldCustomer();
                    break;
            }
        }
    }
}