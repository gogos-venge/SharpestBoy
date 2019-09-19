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
using System.Threading.Tasks;
using SharpestBoy.Components;

namespace SharpestBoy.DMG {
    /// <summary>
    /// Echo RAM "is an artifact of the way the RAM is connected to the bus" (reddit user: stone_henge)
    /// and will repeat the region of 0xC000-0xDDFF to a positive offset of 0x2000 (0xE000)
    /// </summary>
    class EchoRAM : MappedComponent {

        public override void Initialize() {
            AddMemoryMappedIORange(0xC000, 0xDDFF);
        }

        public override bool MMIORead(out byte value, int readAddress) {
            value = 0;
            return false;
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            GetBoard().GetMemoryManagementUnit().DirectWrite(value, (ushort)(writeAddress + 0x2000));
            return false;
        }
    }
}
