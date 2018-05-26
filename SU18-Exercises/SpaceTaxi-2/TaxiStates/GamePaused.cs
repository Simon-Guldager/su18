using DIKUArcade.State;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.EventBus;
using SpaceTaxi_2.TaxiStates;

namespace SpaceTaxi_2.GalagaStates {
    public class GamePaused : IGameState {
        private static GamePaused instance;
        
        private Text[] menuButtons;
        private int activeMenuButton;
        private int maxMenuButtons;

        public GamePaused() {
            InitializeGameState();
        }
        
        public void UpdateGameLogic() {
        }

        public void RenderState() {
            GameRunning.GetInstance().RenderState();
            
            foreach (Text but in menuButtons) {
                but.RenderText();
            }
        }

        public void GameLoop() { }
        
        public void InitializeGameState() { 
            maxMenuButtons = 2;
            activeMenuButton = 0;
            
            menuButtons = new Text[maxMenuButtons];
            menuButtons = new[] {
                new Text("Continue", new Vec2F(0.1f, 0.0f), new Vec2F(0.8f, 0.4f)),
                new Text("Main Menu", new Vec2F(0.1f, -0.1f), new Vec2F(0.8f, 0.4f)),
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
                        TaxiBus.GetBus().RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent,
                                this,
                                "CHANGE_STATE",
                                "MAIN_MENU", ""));
                    } else if (activeMenuButton == 0) {
                        TaxiBus.GetBus().RegisterEvent(
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
        
        public static GamePaused GetInstance() {
            return instance ?? (instance =
                       new GamePaused());
        }
    }
}