
namespace SharpestBoy.Components {
    public abstract class PPU : Peripheral{

        /// <summary>
        /// Returns a pixel map in the raw 2bpp format. Sets the Signal end of frame to false
        /// </summary>
        abstract public int[] Draw();

        /// <summary>
        /// Used to synchronize with the host pc frame rate
        /// </summary>
        /// <returns>True when the screen finished drawing a frame.</returns>
        abstract public bool CheckEndOfFrame();
    }
}
