/**
This file is part of SharpestBoy.
SharpestBoy is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpestBoy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with SharpestBoy.  If not, see <https://www.gnu.org/licenses/>.
*/

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
