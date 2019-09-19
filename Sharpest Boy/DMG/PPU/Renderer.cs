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
