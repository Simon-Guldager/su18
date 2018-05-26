using System;
using System.Drawing;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi_3.SpaceTaxiEntities {
    public class Platform : GameObject {

        public Player Player;
        public int PlatformNumber { get; private set; }
    
        private Text text;
        
        public override void RenderObject() {
            base.RenderObject();
            text.RenderText();
        }

        public void SetPlatformNumber(int n) {
            PlatformNumber = n;

            var pos = BoundingBox.Position.Copy();
            text = new Text(n.ToString(), new Vec2F(pos.X + BoundingBox.Extent.X / 2f - 0.01f, pos.Y), 
                new Vec2F(Constants.HEIGHT, Constants.HEIGHT));
            text.SetFontSize(350);
        }
    }
}