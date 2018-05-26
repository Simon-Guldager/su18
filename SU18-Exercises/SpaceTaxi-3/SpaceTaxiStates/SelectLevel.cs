using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.State;
using DIKUArcade.Math;
using DIKUArcade.EventBus;
using SpaceTaxi_3.SpaceTaxiLevels;

namespace SpaceTaxi_3.SpaceTaxiStates {
    public class SelectLevel : IGameState {
        private static SelectLevel instance;

        private Entity backGroundImage;

        private Text titel;
        private Text[] menuButtons;
        private int activeMenuButton;
        private int maxMenuButtons;

        public SelectLevel() {
            InitializeGameState();
        }
        
        public static SelectLevel GetInstance() {
            return SelectLevel.instance ?? (SelectLevel.instance = new SelectLevel());
        }
        
        public void UpdateGameLogic() { }
        
        public void RenderState() {
            backGroundImage.RenderEntity();
            
            titel.RenderText();
            foreach (Text but in menuButtons) {
                but.RenderText();
            }
        }
        
        public void GameLoop() { }
        
        public void InitializeGameState() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));
            
            titel = new Text("Select Level", new Vec2F(0f, 0.4f), new Vec2F(1f, 0.4f));
            titel.SetColor(new Vec3F(1f, 1f, 0f));
            titel.SetFontSize(65);

            maxMenuButtons = LevelContainer.GetInstance().Levels.Length;
            
            menuButtons = new Text[maxMenuButtons + 1];
            activeMenuButton = 2;

            for (int i = 0; i < maxMenuButtons; i++) {
                var levelName = LevelContainer.GetInstance().Levels[i].Name;
                var text = new Text(levelName, 
                    new Vec2F(0.3f, i * 0.1f + 0.2f), new Vec2F(0.8f, 0.4f)); 
                text.SetColor(new Vec3F(1.0f, 1.0f, 1.0f));
                text.SetFontSize(20);
                menuButtons[maxMenuButtons - 1 - i] = text;
            }
            
            menuButtons[maxMenuButtons] =
                new Text("Return", new Vec2F(0.3f, -0.2f), new Vec2F(0.8f, 0.4f));
            menuButtons[maxMenuButtons].SetColor(new Vec3F(0.0f, 1.0f, 0.0f)); 
        }

        public void HandleKeyEvent(string keyValue, string keyAction) {
            if (keyAction == "KEY_PRESS") {    
                switch (keyValue) {
                    case "KEY_UP":
                        if (activeMenuButton > 0) {
                            menuButtons[activeMenuButton].SetColor(new Vec3F(1.0f, 1.0f, 1.0f));
                            activeMenuButton--;
                            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f));
                        }
                        break;
                    case "KEY_DOWN":
                        if (activeMenuButton < maxMenuButtons) {
                            menuButtons[activeMenuButton].SetColor(new Vec3F(1.0f, 1.0f, 1.0f));
                            activeMenuButton++;
                            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f));
                        }
                        break;
                    case "KEY_ENTER":
                        switch (activeMenuButton) {
                            case 2:
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.GameStateEvent,
                                        this, "CHANGE_STATE", "MAIN_MENU", ""));
                                break;
                            default:
                                LevelContainer.GetInstance().GetLevel(maxMenuButtons - activeMenuButton - 1);
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.GameStateEvent,
                                        this, "CHANGE_STATE", "GAME_RUNNING", ""));
                                break;
                        }
                        break;
                }        
            }
        }
    }
}