using DIKUArcade.Graphics;

namespace SpaceTaxi_3.SpaceTaxiEntities {
    public class ExitObject : GameObject {
        public bool ExitClosed { get; private set; }

        public ExitObject() {
            ExitClosed = true;
        }

        public override void RenderObject() {
            if (ExitClosed) {
                base.RenderObject();    
            }
        }
        
        public void OpenExit() {
            ExitClosed = false;
        }
    }    
}