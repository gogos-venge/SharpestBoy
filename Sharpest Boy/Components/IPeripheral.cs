
namespace SharpestBoy.Components {
    /// <summary>
    /// Allows a component to become updatable.
    /// </summary>
    public interface IPeripheral {

        /// <summary>
        /// Advances the state of a Component in time.
        /// </summary>
        /// <param name="clocks">The CPU clocks to advance</param>
        void Update(int Clocks);
    }
}
