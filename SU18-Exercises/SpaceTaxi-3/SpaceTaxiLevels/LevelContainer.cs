using System;
using System.IO;
using System.Collections.Generic;
using DIKUArcade.EventBus;

namespace SpaceTaxi_3.SpaceTaxiLevels {

    /// <summary>
    /// This class contains a list of SpaceTaxiLevel objects, as well
    /// as information as to which one is currently active
    /// </summary>
    public class LevelContainer {

        public SpaceTaxiLevel[] Levels { get; private set; }
        public SpaceTaxiLevel ActiveLevel { get; private set; }
        
        private static LevelContainer instance;

        private int activeLevelNumber;
        
        public LevelContainer() {
            Restart();     
        }

        public static LevelContainer GetInstance() {
            return instance ?? (instance = new LevelContainer());
        }
        
        public void Restart() {
            activeLevelNumber = 0;
            LoadLevels();
            GetLevel(activeLevelNumber);
        }

        public void NextLevel() {
            GetLevel(activeLevelNumber + 1);
        }
        
        public void GetLevel(int n) {
            activeLevelNumber = n;
            if (n < Levels.Length) {
                Console.WriteLine("Loading " + Levels[n].Name);
                
                // the current customer is tranfered betweeen levels
                var customer = ActiveLevel.Customer;
                
                // deactivate the old level
                ActiveLevel.DeactivateGameLevel();
                
                // activate the new level    
                ActiveLevel = Levels[n];;
                ActiveLevel.ResetLevel();
                ActiveLevel.Customer = customer;
                ActiveLevel.ActivateGameLevel();
            } else {
                Console.WriteLine("U won");    
            }   
        }

        private void LoadLevels() {
            
            // find base path of Levels directory
            var dir = new DirectoryInfo(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));
            while (dir.Name != "bin") {
                dir = dir.Parent;
            }
            dir = dir.Parent;
            
            var path = Path.Combine(dir.FullName.ToString(), "Levels");
            var files = new DirectoryInfo(path).GetFiles();
            
            // get level from file with LevelParser
            Levels = new SpaceTaxiLevel[files.Length];
            for (var i = 0; i < files.Length; i++) {
                Levels[i] = LevelParser.GetLevelFromFile(Path.Combine(path, files[i].Name));     
            }
            ActiveLevel = Levels[0];
        }
    }
}