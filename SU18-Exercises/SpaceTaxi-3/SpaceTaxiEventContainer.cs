using DIKUArcade.EventBus;
using DIKUArcade.Timers;

namespace SpaceTaxi_3 {
    public class SpaceTaxiEventContainer {
        private static TimedEventContainer container;

        public static void SetContainerSize(int size) {
            SpaceTaxiEventContainer.container = new TimedEventContainer(size);    
        }
        
        public static TimedEventContainer GetContainer() {
            return SpaceTaxiEventContainer.container ?? (
                       SpaceTaxiEventContainer.container = new TimedEventContainer(1));
        }
    }
}