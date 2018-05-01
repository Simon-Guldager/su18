using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi_1.TaxiEntities;

namespace SpaceTaxi_1 {
    public class LevelParser {
        public Player Player;
        public Customer Customer;
        public List<Prop> Props;
        public EntityContainer LevelSprites;

        private int currentLevelNumber;
        private int numberOfLevels;
        private List<Customer> customers;    
        
        public LevelParser() {
            Player = new Player();
            currentLevelNumber = 0;
            
            NextLevel();
        }
        
        public bool NextCustomer() {
            if (customers.Count > 0) {
                Customer = customers[0];
                var pos = Customer.Platform.GetPosition();
                pos.Y += 1 / 23f;
                Customer.Entity.Shape.Position = pos;
                customers = customers.Skip(1).ToList();
            } else if (currentLevelNumber < numberOfLevels) {
                NextLevel();
            } else {
                return false;
            }
            return true;
        }
        
        private void NextLevel() {
            if (currentLevelNumber <= numberOfLevels) {
                Player.InFlight = false;
                Player.Entity.Shape.AsDynamicShape().Direction = new Vec2F();
                
                customers = new List<Customer>();
                Props = new List<Prop>();
                LevelSprites = new EntityContainer();
                
                GetLevel(currentLevelNumber);  
                NextCustomer();
                currentLevelNumber++;
            }   
        }
        
        private string GetTextMap(int levelNumber) {
            // find base path
            var dir = new DirectoryInfo(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));
            while (dir.Name != "bin") {
                dir = dir.Parent;
            }
            dir = dir.Parent;

            // get text map
            var path = Path.Combine(dir.FullName.ToString(), "Levels");
            var files = new DirectoryInfo(path).GetFiles();
            numberOfLevels = files.Length;
            return Path.Combine(path, files[levelNumber].Name);
        }

        private void GetLevel(int n) {
            var file = GetTextMap(n);
            var text = File.ReadAllLines(file);
            
            var keyLegend = new Dictionary<char, Image>();
            var platformKeys = new Dictionary<char, Platform>();
            
            SetKeys(text.Skip(24), platformKeys, keyLegend);
            AsciiToLevel(text.Take(23), platformKeys, keyLegend);
        }

        private void SetKeys(IEnumerable<string> text,
            Dictionary<char, Platform> platformKeys, Dictionary<char, Image> keyLegend) {
            // read from metadata
            foreach (var line in text) {
                if (line != "") {
                    var s = line.Split(' ');
                    switch (s[0]) {
                        case "Name:":
                            break;
                        case "Platforms:":
                            foreach (var ss in s.Skip(1)) {
                                var plat = new Platform();
                                Props.Add(plat);
                                platformKeys.Add(ss[0], plat);
                            }
                            break;
                        case "Customer:":
                            var origin = platformKeys[s[3][0]];
                            var dest = s[4][0];
                            var customer = new Customer(origin);
                            var pos = origin.GetPosition();
                            customer.Entity.Shape.Position = pos;
                            if (dest != '^') {
                                customer.Destination = platformKeys[dest];
                            }   
                            customers.Add(customer);
                            break;
                        default:
                            keyLegend.Add(line[0], 
                                new Image(Path.Combine("Assets", "Images", line.Substring(3))));
                            break;
                    }
                }
            }   
        }
        
        private void AsciiToLevel(IEnumerable<string> text,
            Dictionary<char, Platform> platformKeys, Dictionary<char, Image> keyLegend) {
            // map is 23 x 40, calculate size of each levelSprite
            float height = 1f / 23f;
            float width = 1f / 40f;
            
            // translate from ascii to game level, each char represents an entity
            var textArr = text.ToArray();
            for (int i = 0; i < 23; i++) {
                for (int j = 0; j < 40; j++) {
                    char _char = textArr[22-i][j];
                    
                    // ^ is the ascii symbol for the exit
                    if (_char == '^') {
                        var exitStrides = ImageStride.CreateStrides(4,
                            Path.Combine("Assets", "Images", "aspargus-passage.png"));
                        var shape = new StationaryShape(
                            new Vec2F(width * j, i * height), new Vec2F(width, height)); 
                        var e = new Exit();
                        e.PropSprites.AddStationaryEntity(
                            shape, new ImageStride(30, exitStrides));
                        Props.Add(e);
                    // > is the ascii symbol for the player, set player position
                    } else if (_char == '>') {
                        Player.Entity.Shape.Position = 
                            new Vec2F(width * j - width, i * height - height / 2);  
                    } else if (_char != ' ') {
                        // add the char as an Entity to the appropriate container
                        EntityContainer container;
                        if (platformKeys.ContainsKey(_char)) {
                            container = platformKeys[_char].PropSprites;
                        } else {
                            container = LevelSprites;
                        }
                        container.AddStationaryEntity(
                            new StationaryShape(
                                new Vec2F(width * j, i * height), 
                                new Vec2F(width, height)),
                            keyLegend[_char]);
                    }
                } 
            }
        }
    }
}