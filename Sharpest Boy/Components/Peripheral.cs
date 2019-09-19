
namespace SharpestBoy.Components {
    /// <summary>
    /// A Component that takes up MMIO registers and also needs to be updated using the CPU timer. (ex. Most of components like Display, Audio, Timer etc)
    /// </summary>
    public abstract class Peripheral : MappedComponent, IPeripheral {

        public int Clocks { get; set; } = 4;
        public abstract void Update(int Clocks);

    }
}
