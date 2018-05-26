using System;

namespace SpaceTaxi_3.SpaceTaxiStates {
    public enum GameStateType {
        GameRunning,
        GamePaused,
        MainMenu,
        SelectLevel,
        GameLost,
        GameWon
    }

    public class StateTransformer {
        public static GameStateType TransformStringToState(string state) {
            switch (state) {
                case "GAME_RUNNING":
                    return GameStateType.GameRunning;
                case "GAME_PAUSED":
                    return GameStateType.GamePaused;
                case "MAIN_MENU":
                    return GameStateType.MainMenu;
                case "GAME_LOST":
                    return GameStateType.GameLost;
                case "GAME_WON":
                    return GameStateType.GameWon;
                case "SELECT_LEVEL":
                    return GameStateType.SelectLevel;
                default:
                    throw new ArgumentException();
            }
        }

        public static string TransformStateToString(GameStateType state) {
            switch (state) {
                case GameStateType.GameRunning:
                    return "GAME_RUNNING";
                case GameStateType.GamePaused:
                    return "GAME_PAUSED";
                case GameStateType.MainMenu:
                    return "MAIN_MENU";
                case GameStateType.GameLost:
                    return "GAME_LOST";
                case GameStateType.GameWon:
                    return "GAME_WON";
                case GameStateType.SelectLevel:
                    return "SELECT_LEVEL";
                default:
                    throw new ArgumentException();
            }
        }
    }
}

