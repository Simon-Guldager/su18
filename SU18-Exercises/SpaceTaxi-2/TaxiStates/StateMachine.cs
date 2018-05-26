using DIKUArcade.EventBus;
using DIKUArcade.State;
using SpaceTaxi_2.GalagaStates;

namespace SpaceTaxi_2.TaxiStates {
    public class StateMachine : IGameEventProcessor<object> {      
        public IGameState ActiveState { get; private set; }

        public StateMachine() {
            TaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            TaxiBus.GetBus().Subscribe(GameEventType.InputEvent, this);

            ActiveState = MainMenu.GetInstance();
        }

        public void SwitchState(GameStateType stateType) {
            switch (stateType) {
                case GameStateType.GameRunning:
                    ActiveState = GameRunning.GetInstance();
                    break;
                case GameStateType.MainMenu:
                    ActiveState = MainMenu.GetInstance();
                    break;
                case GameStateType.SelectLevel:
                    ActiveState = SelectLevel.GetInstance();
                    break;
                case GameStateType.GamePaused:
                    ActiveState = GamePaused.GetInstance();
                    break;
            }
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.GameStateEvent) {
                switch (gameEvent.Message) {
                    case "CHANGE_STATE":
                        var state = StateTransformer.TransformStringToState(gameEvent.Parameter1);
                        SwitchState(state);
                        break;
                }    
            } else if (eventType == GameEventType.InputEvent) {
                ActiveState.HandleKeyEvent(gameEvent.Message, gameEvent.Parameter1);
            } 
        }
    }
}