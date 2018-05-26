using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi_3.SpaceTaxiEntities;

namespace SpaceTaxi_3.SpaceTaxiLevels {
    public class LevelParser {
        public static SpaceTaxiLevel GetLevelFromFile(string filepath) {
            
            Dictionary<char, Image> keyLegend = new Dictionary<char, Image>();
            SpaceTaxiLevel level = new SpaceTaxiLevel();
            
            var text = File.ReadAllLines(filepath);

            foreach (var line in text.Skip(24)) {
                if (line != "") {
                    var s = line.Split(' ');
                    
                    // the first word in the sentence determines the type
                    switch (s[0]) {
                        
                        case "Name:":
                            
                            // set map name
                            level.SetLevelName(line.Replace("Name:", ""));
                            break;
                        
                        case "Platforms:": 
                            
                            // add the platform to the dictionary with the symbol as the key
                            foreach (var ss in s.Skip(1)) {
                                var plat = new Platform();
                                level.Platforms.Add(ss[0], plat);
                            }
                            break;
                        
                        case "Customer:":
                            
                            // add customer with origin and destionation to dict with key
                            var customer = new Customer(s[1]);
                            var spawnTimer = Int32.Parse(s[2]);
                            var despawnTimer = Int32.Parse(s[5]) + spawnTimer;

                            customer.SpawnTimer = spawnTimer;
                            customer.DesapwnTimer = despawnTimer;
                            customer.Origin = level.Platforms[s[3][0]];
                            
                            var dest = s[4];
                            if (dest[0] == '^') {
                                customer.DestinationOnNewLevel = true;

                                if (dest.Length > 1) {
                                    customer.Destination = dest[1];       
                                } else {
                                    customer.Destination = '^';
                                }
                            } else {
                                customer.Destination = dest[0];
                            }

                            level.Customers.Add(customer);
                            break;
                        
                        default:
                            keyLegend.Add(line[0], 
                                new Image(Path.Combine("Assets", "Images", line.Substring(3))));
                            break;
                    }
                }
            }   
            
            // translate from ascii to game level, each char represents an entity
            var textArr = text.ToArray();
            for (int i = 0; i < 23; i++) {
                for (int j = 0; j < 40; j++) {
                    char symbol = textArr[22-i][j];

                    // ^ is the ascii symbol for the exit
                    if (symbol == '^') {
                        var exitStrides = ImageStride.CreateStrides(4,
                            Path.Combine("Assets", "Images", "aspargus-passage.png"));
                        level.Exit.AddEntity(new StationaryShape(
                            new Vec2F(Constants.WIDTH * j, i * Constants.HEIGHT + Constants.BOTTOM), 
                            new Vec2F(Constants.WIDTH, Constants.HEIGHT)),
                            new ImageStride(30, exitStrides));
                    
                    // > is the ascii symbol for the player, set player position
                    } else if (symbol == '>') {
                        level.AddPlayer(new Vec2F(Constants.WIDTH * j - Constants.WIDTH, 
                            i * Constants.HEIGHT - Constants.HEIGHT / 2));
                    
                    } else if (symbol != ' ') {
                        
                        // add the char as an Entity to the appropriate container
                        if (level.Platforms.ContainsKey(symbol)) {
                            level.Platforms[symbol].AddEntity(
                                new StationaryShape(
                                    new Vec2F(Constants.WIDTH * j, i * Constants.HEIGHT + Constants.BOTTOM), 
                                    new Vec2F(Constants.WIDTH, Constants.HEIGHT)),
                                keyLegend[symbol]);
                        } else {
                            level.Obstacles.AddStationaryEntity(
                                new StationaryShape(
                                    new Vec2F(Constants.WIDTH * j, i * Constants.HEIGHT + Constants.BOTTOM), 
                                    new Vec2F(Constants.WIDTH, Constants.HEIGHT)),
                                keyLegend[symbol]);
                        }
                    }
                } 
            }
            
            // assign each platform a number, which will be rendered on the screen
            var n = 1;
            foreach (Platform p in level.Platforms.Values) {
                p.SetPlatformNumber(n);
                n++;
            }
            
            return level;
        } 
    }
}