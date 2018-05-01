using System.IO;
using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.EventBus;

namespace Galaga_Exercise_3 {
    
    public class Player : IGameEventProcessor<object> {
        private GameEventBus<object> eventBus;
        private float speed = 0.009f;
        
        public Entity Entity;
        
        public Player() {
            Entity = new Entity(
                new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "Player.png")));
            
            eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.PlayerEvent 
            }); 
            eventBus.Subscribe(GameEventType.PlayerEvent, this);   
        }    
          
        /// <summary>
        /// Moves the player entity in the current direction,
        /// if the player entity is inside the window.
        /// </summary>
        public void Move() {
            var ent = ((DynamicShape) Entity.Shape);
            var nextPos = Entity.Shape.Position.X + ent.Direction.X;
            if (nextPos > 0.0 && nextPos < 0.9) {
                ent.Move();             
            }
        }

        public void MoveLeft() {
            ((DynamicShape) Entity.Shape).Direction.X -= speed;
            Move();
        }

        public void MoveRight() {
            ((DynamicShape) Entity.Shape).Direction.X += speed;
            Move();
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
            }
        }
    }
}
