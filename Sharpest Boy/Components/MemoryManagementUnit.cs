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
    /// <summary>
    /// A Memory Management Unit with an 8bit Data Bus and a 16bit Address Bus
    /// </summary>
    public abstract class MemoryManagementUnit : Component {
        
        /// <summary>
        /// Reads a byte from the specified address. The actual read value reflects the behaviour of the emulated MMU.
        /// </summary>
        /// <param name="address">The address to read the byte from</param>
        /// <returns></returns>
        abstract public byte Read(ushort address);

        /// <summary>
        /// Writes a byte to the specified address. The actual written value reflects the behaviour of the emulated MMU.
        /// </summary>
        /// <param name="b">The byte to be written</param>
        /// <param name="address">The address to write the byte</param>
        abstract public void Write(byte b, ushort address);

        /// <summary>
        /// Reads a byte from the specified address. This bypasses any MMIO register or memory behaviour and returns the raw value.
        /// Use it with extreme caution, only when performance is priority. (eg. when handling interrupts)
        /// </summary>
        /// <param name="address">The address to read the byte from</param>
        /// <returns></returns>
        abstract public byte DirectRead(ushort address);

        /// <summary>
        /// Writes a byte to the specified address. This bypasses any MMIO register or memory behaviour and writes the raw value.
        /// Use it with extreme caution, only when performance is priority. (eg. when handling interrupts)
        /// </summary>
        /// <param name="b"></param>
        /// <param name="address"></param>
        abstract public void DirectWrite(byte b, ushort address);

        /// <summary>
        /// Reroutes the MMIO reads from the attached mapped components / peripherals if any. This is not mandatory for all MMUs
        /// but most of the implementations usually feature some Mapped Devices (MMIO). Call it first when implementing the Read() method.
        /// </summary>
        /// <param name="b">The returned value from the peripheral. WARNING: Returns 0 if not handled!</param>
        /// <param name="address">The specific address of this peripheral MMIO</param>
        /// <returns>True if the device handled the read internally</returns>
        public bool RouteMMIOReads(out byte b, ushort address) {
            foreach (Board.MMIO mmio in GetBoard().GetMappedRanges()) {
                if (address >= mmio.Lo && address <= mmio.Hi) {
                    return mmio.C.MMIORead(out b, address);
                }
            }
            b = 0;
            return false;
        }

        /// <summary>
        /// Reroutes the MMIO writes to the attached mapped components / peripherals if any. This is not mandatory for all MMUs
        /// but most of the implementations usually feature some Mapped Devices (MMIO). Call it first when implementing the Write() method.
        /// </summary>
        /// <param name="b">The value to route</param>
        /// <param name="address">The specific address of this peripheral MMIO</param>
        /// <returns>True if the device handled the write internally</returns>
        public bool RouteMMIOWrites(byte b, ushort address) {
            foreach(Board.MMIO mmio in GetBoard().GetMappedRanges()) {
                if(address >= mmio.Lo && address <= mmio.Hi) {
                    return mmio.C.MMIOWrite(b, address);
                }
            }
            return false;
        }

    }
}