﻿using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.State;
using DIKUArcade.Math;
using DIKUArcade.EventBus;

namespace Galaga_Exercise_3.GalagaStates {
    public class MainMenu : IGameState {
        private static MainMenu instance;

        private Entity backGroundImage;
        
        private Text[] menuButtons;
        private int activeMenuButton;
        private int maxMenuButtons;

        public MainMenu() {
            InitializeGameState();
        }

        public void UpdateGameLogic() { }
        
        public void RenderState() {
            backGroundImage.RenderEntity();
            
            foreach (Text but in menuButtons) {
                but.RenderText();
            }
        }
        
        public void GameLoop() { }
        
        public void InitializeGameState() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "TitleImage.png")));

            maxMenuButtons = 2;
            activeMenuButton = 0;
            
            menuButtons = new Text[maxMenuButtons];
            menuButtons = new[] {
                new Text("New Game", new Vec2F(0.1f, 0.0f), new Vec2F(0.8f, 0.4f)),
                new Text("      Quit", new Vec2F(0.1f, -0.1f), new Vec2F(0.8f, 0.4f)),
            };

            for (int i = 0; i < maxMenuButtons; i++) {
                menuButtons[i].SetColor(new Vec3F(1.0f, 0.0f, 0.0f));
                menuButtons[i].SetFontSize(70);
            }
            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f)); 
        }

        public void HandleKeyEvent(string keyValue, string keyAction) {
            if (keyAction == "KEY_PRESS") {    
                switch (keyValue) {
                    case "KEY_UP":
                        if (activeMenuButton > 0) {
                            menuButtons[activeMenuButton].SetColor(new Vec3F(1.0f, 0.0f, 0.0f));
                            activeMenuButton--;
                            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f));
                        }
                        break;
                    case "KEY_DOWN":
                        if (activeMenuButton < maxMenuButtons - 1) {
                            menuButtons[activeMenuButton].SetColor(new Vec3F(1.0f, 0.0f, 0.0f));
                            activeMenuButton++;
                            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f));
                        }
                        break;
                    case "KEY_ENTER":
                        if (activeMenuButton == 1) {
                            GalagaBus.GetBus().RegisterEvent(
                                GameEventFactory<object>.CreateGameEventForAllProcessors(
                                    GameEventType.WindowEvent,
                                    this,
                                    "CLOSE_WINDOW","","")); 
                        } else if (activeMenuButton == 0) {
                            GameRunning.NewInstance();
                            GalagaBus.GetBus().RegisterEvent(
                                GameEventFactory<object>.CreateGameEventForAllProcessors(
                                    GameEventType.GameStateEvent,
                                    this,
                                    "CHANGE_STATE",
                                    "GAME_RUNNING", ""));
                        }
                        break;
                }        
            }
        }
        
        public static MainMenu GetInstance() {
            return instance ?? (instance =
                       new MainMenu());
        }
    }
}