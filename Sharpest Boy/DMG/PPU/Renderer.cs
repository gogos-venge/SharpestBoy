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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace SharpestBoy.DMG.PPU {
    class Renderer {

        /// <summary>
        /// Gets the pixel color from two tile bytes based on the given pallete and the LCD pointer current position
        /// </summary>
        /// <param name="b1">First byte in memory</param>
        /// <param name="b2">Second byte in memory</param>
        /// <param name="pos">Pixel position (0-7)</param>
        /// <param name="pal">The pallete to get the colors from</param>
        /// <returns>0-3 with 3 being the darkest</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPixel(byte b1, byte b2, int pos, int[] pal) {
            pos = 7 - pos;
            int shade = (((b2 >> pos) & 1) << 1 | ((b1 >> pos) & 1));
            return pal[shade];
        }
    }
}
