using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.State;
using DIKUArcade.Math;
using DIKUArcade.EventBus;

namespace SpaceTaxi_2.TaxiStates {
    public class MainMenu : IGameState {
        private static MainMenu instance;

        private Entity backGroundImage;

        private Text titel;
        private List<Text> menuButtons;
        private int activeMenuButton;
        private int maxMenuButtons;

        public MainMenu() {
            InitializeGameState();
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

            maxMenuButtons = 3;
            activeMenuButton = 0;
            
            titel = new Text("Space\n  Taxi", new Vec2F(0.05f, 0.6f), new Vec2F(0.85f, 0.4f));
            titel.SetColor(new Vec3F(1f, 1f, 0f));
            titel.SetFontSize(120);
            
            menuButtons = new List<Text> {
                new Text("  New Game", new Vec2F(0.15f, 0.0f), new Vec2F(0.8f, 0.4f)),
                new Text("Choose Level", new Vec2F(0.15f, -0.1f), new Vec2F(0.8f, 0.4f)),
                new Text("        Quit", new Vec2F(0.15f, -0.2f), new Vec2F(0.8f, 0.4f)),
            };

            for (int i = 0; i < maxMenuButtons; i++) {
                menuButtons[i].SetColor(new Vec3F(1.0f, 1.0f, 1.0f));
                menuButtons[i].SetFontSize(50);
            }
            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f)); 
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
                        if (activeMenuButton < maxMenuButtons - 1) {
                            menuButtons[activeMenuButton].SetColor(new Vec3F(1.0f, 1.0f, 1.0f));
                            activeMenuButton++;
                            menuButtons[activeMenuButton].SetColor(new Vec3F(0.0f, 1.0f, 0.0f));
                        }
                        break;
                    case "KEY_ENTER":
                        switch (activeMenuButton) {
                            case 1:
                                TaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.GameStateEvent,
                                        this,
                                        "CHANGE_STATE",
                                        "SELECT_LEVEL", ""));
                                break;
                            case 2:
                                TaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.WindowEvent,
                                        this,
                                        "CLOSE_WINDOW","","")); 
                                break;
                            default:
                                //GameRunning.NewInstance();
                                GameRunning.NewInstance();
                                TaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.GameStateEvent,
                                        this,
                                        "CHANGE_STATE",
                                        "GAME_RUNNING", ""));
                                break;
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