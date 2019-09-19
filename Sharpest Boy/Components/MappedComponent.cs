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

using System.Collections.Generic;
using SharpestBoy.Exceptions;

namespace SharpestBoy.Components {
    /// <summary>
    /// A Component that takes up MMIO registers but doesn't need to be (or can't be) updated. (ex. Cartridge, Echo RAM)
    /// </summary>
    public abstract class MappedComponent : Component {

        public List<Board.MMIO> Ranges = new List<Board.MMIO>();

        /// <summary>
        /// Reads an MMIO that belongs to a Peripheral. MMU will reroute the call to the device
        /// when the readAddress is within the range of LowAddress to HiAddress and instead return an internal value
        /// of the peripheral rather than a typical saved value that otherwise belongs to RAM.
        /// </summary>
        /// <param name="value">The value the Peripheral MUST return. For example in DMG STAT has its 7th bit always 1. WARNING: Return 0 if not handled!</param>
        /// <param name="readAddress">The address that belongs to the Peripheral MMIO register</param>
        /// <returns>If the register is read from the Peripheral returns true (handled)</returns>
        abstract public bool MMIORead(out byte value, int readAddress);

        /// <summary>
        /// Writes an MMIO Register that belongs to a Peripheral. MMU will reroute the call to the device
        /// when the writeAddress is within the range of LowAddress to HiAddress and write a value to the peripheral
        /// rather than saving it to RAM.
        /// </summary>
        /// <param name="value">The value you want to write to the peripheral MMIO register</param>
        /// <param name="writeAddress">The address that belongs to the Peripheral MMIO register</param>
        /// <returns>If the register is written to the Peripheral returns true (handled)</returns>
        abstract public bool MMIOWrite(byte value, int writeAddress);

        /// <summary>
        /// Maps the peripheral to the Memory Management Unit (MMIO) within the given range
        /// </summary>
        /// <param name="lowAddress">The lowest end of the address the Memory controller will reroute (inclusive)</param>
        /// <param name="hiAddress">The highest end of the address the Memory controller will reroute (inclusive)</param>
        /// <exception cref="InvalidMappedRangeException">Thrown when High Address is smaller than Low Address</exception>
        public void AddMemoryMappedIORange(int lowAddress, int hiAddress) {
            if (lowAddress > hiAddress) throw new InvalidMappedRangeException("Low address must be less or equal than high address");
            Ranges.Add(new Board.MMIO(lowAddress, hiAddress, this));
        }

        public override string ToString() {
            return "Address Range:\t"+ string.Join(",", Ranges);
        }

    }
}
