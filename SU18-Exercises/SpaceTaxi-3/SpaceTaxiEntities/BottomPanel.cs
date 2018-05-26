using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_3.SpaceTaxiEntities {
    public class BottomPanel {
        private EntityContainer container;

        private string row1 = "1222222221111333333333333331111222222221";
        private string row2 = "1444444441111133333333333311111444444441";
        
        public BottomPanel() {
            container = new EntityContainer();

            for (int i = 0; i < 40; i++) {
                AddEnitityFromChar(row1[i], i, 1);
            }
            for (int i = 0; i < 40; i++) {
                AddEnitityFromChar(row2[i], i, 0);
            }
        }

        public void Render() {
            container.RenderEntities();
        }

        private void AddEnitityFromChar(char c, float x, float y) {
            switch (c) {
                case '1':                    
                    container.AddStationaryEntity(
                        new StationaryShape(new Vec2F(x * Constants.WIDTH, y * Constants.HEIGHT), 
                            new Vec2F(Constants.WIDTH, Constants.BOTTOM / 2)), 
                        new Image(Path.Combine("Assets", "Images", "green-square.png")));
                    break;
                case '2':
                    container.AddStationaryEntity(
                        new StationaryShape(new Vec2F(x * Constants.WIDTH, y * Constants.HEIGHT), 
                            new Vec2F(Constants.WIDTH, Constants.BOTTOM / 2)), 
                        new Image(Path.Combine("Assets", "Images", "studio-square.png")));
                    break;    
                case '4':
                    container.AddStationaryEntity(
                        new StationaryShape(new Vec2F(x * Constants.WIDTH, y * Constants.HEIGHT), 
                            new Vec2F(Constants.WIDTH, Constants.BOTTOM / 2)), 
                        new Image(Path.Combine("Assets", "Images", "white-square.png")));
                    break;              
            }    
        }
    }
}