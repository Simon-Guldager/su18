using System;
using System.Collections.Generic;
using System.Linq;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi_3.SpaceTaxiEntities;
using SpaceTaxi_3.SpaceTaxiStates;

namespace SpaceTaxi_3.SpaceTaxiLevels {
    public class SpaceTaxiLevel {
        
        // uninitialized data members
        public string Name { get; private set; } = "";
        public Player Player { get; set; } = null;
        public Customer Customer = null;
        
        public EntityContainer Obstacles { get; private set; } = new EntityContainer();
        public Dictionary<char, Platform> Platforms { get; private set; } = new Dictionary<char, Platform>();
        public ExitObject Exit { get; private set; } = new ExitObject();
        public List<Customer> Customers { get; private set; } = new List<Customer>();
        
        private Vec2F PlayerStartPosition { get; set; }
        
        private int customerNumber = 0;

        private Customer oldCustomer = null;
        
        public SpaceTaxiLevel() { } // this class shall be treated as data-only
        
        public void ResetLevel() {
            customerNumber = 0;
            Customer = null;
            InitializeCustomers();
            Player.StopMoving();
            Player.PlayerIsMoving = true;
            Player.Shape.Position = PlayerStartPosition;
        }
        
        public void ActivateGameLevel() {
            if (Customer != null) {
                if (Customer.Destination == '^') {
                    Console.WriteLine("Any Platform Please");
                } else {
                    Console.WriteLine(Platforms[Customer.Destination].PlatformNumber);          
                }  
            }
            
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, Player);
        }

        public void DeactivateGameLevel() {
            SpaceTaxiBus.GetBus().Unsubscribe(GameEventType.PlayerEvent, Player);
        }
        
        public void SetLevelName(string name) {
            if (name != "") {
                Name = name;       
            }
        }
        
        public void AddPlayer(Vec2F pos) {
            if (Player == null) {
                PlayerStartPosition = pos.Copy();
                Player = new Player();
                Player.Shape.Position = pos;
            }
        }
    
        public void UpdateLevel() {
            
            Player.Move();
            
            UpdateCustomer();   
            
            CheckPlatformCollision();
            CheckObstacleCollision();
            CheckExitCollision();

            if (oldCustomer != null) {
                oldCustomer.Update();
                
                if (oldCustomer.ToBeDeleted && oldCustomer.DestinationReached) {
                    oldCustomer.DestinationReached = false;
                    SpaceTaxiEventContainer.GetContainer().AddTimedEvent(
                        TimeSpanType.Seconds, 2, "DELETE_OLD_CUSTOMER", "", "");
                }
            }
        }

        public void RenderLevel() {
            Platforms.ToList().ForEach(platform => platform.Value.RenderObject());
            Obstacles.RenderEntities();
            Exit.RenderObject();
            
            Player.RenderPlayer();

            if (Customer != null) {
                Customer.RenderCustomer();     
            }

            if (oldCustomer != null) {
                oldCustomer.RenderCustomer();    
            }
        }

        public void CheckObstacleCollision() {
            foreach (Entity obstacle in Obstacles) {
                if (Physics.Collision(Player.Shape, obstacle.Shape)) {
                    ResetLevel();
                }    
            }    
        }
        
        public void CheckExitCollision() {
            if (Exit.CheckCollision(Player.Shape)) {
                if (Exit.ExitClosed) {
                    ResetLevel();    
                } else {
                    LevelContainer.GetInstance().NextLevel();    
                }
            }  
        }

        public void CheckPlatformCollision() {
            if (!Player.PlayerIsMoving || Player.Shape.Direction.Y > 0) {
                return;    
            }
            
            foreach (var pair in Platforms) {
                pair.Value.Player = null;
                if (pair.Value.CheckCollision(Player.Shape)) {  
                    if (Player.Shape.Direction.Y < -0.003f) {
                        ResetLevel(); 
                    }
                    
                    Player.Shape.Position.Y += 0.0001f;
                    Player.StopMoving();
                    
                    Player.Shape.Position.Y =
                        Player.Shape.Position.Y - 0.005f;
                    
                    pair.Value.Player = Player;

                    if (Customer == null) {
                         return;   
                    } 
                    if (Customer.Destination == pair.Key || Customer.Destination == '^') {
                        Customer.Shape.Position = Player.Shape.Position.Copy();
                        Customer.DestinationReached = true;
                        Customer.CustomerInTransport = false;
                    }    
                    if (Customer.DestinationReached) {
                        Customer.Origin = Platforms[Customer.Destination];
                        oldCustomer = Customer;
                        oldCustomer.CustomerInTransport = false;
                        Customer = null;
                    }
                }
            }   
        }

        public void DespawnCustomer() {
            Customer = null;
            customerNumber++;      
        }

        public void SpawnCustomer() {
            Customer = Customers[customerNumber];    
            Customer.ResetPosition();    
        }

        public void DeleteOldCustomer() {
            oldCustomer = null;
        }
        
        private void InitializeCustomers() {      
             
            SpaceTaxiEventContainer.SetContainerSize(Customers.Count * 2 + 1);
            
            SpaceTaxiEventContainer.GetContainer().ResetContainer();
            SpaceTaxiEventContainer.GetContainer().AttachEventBus(SpaceTaxiBus.GetBus());
            
            var container = SpaceTaxiEventContainer.GetContainer();
            foreach (Customer c in Customers) {
                container.AddTimedEvent(TimeSpanType.Seconds, c.SpawnTimer, "SPAWN_CUSTOMER", "", "");        
                container.AddTimedEvent(TimeSpanType.Seconds, c.DesapwnTimer, "DESPAWN_CUSTOMER", "", "");    
            }
        }
        
        private void UpdateCustomer() {
            
            // if the customer is inside the taxi, none of this applies
            if (Customer == null || Customer.CustomerInTransport) {
                return;    
            } 
            Customer.Update();
            
            if (Physics.Collision(Player.Shape, Customer.Shape)) {
                if (Player.PlayerIsMoving) {
                    ResetLevel();            
                } else {  
                    Customer.CustomerInTransport = true;
                    if (Customer.DestinationOnNewLevel) {
                        Exit.OpenExit();  
                        Console.WriteLine("Up Please");
                    } else {
                        Console.WriteLine(Platforms[Customer.Destination].PlatformNumber); 
                    }
                }
            }     
        }
    }
}