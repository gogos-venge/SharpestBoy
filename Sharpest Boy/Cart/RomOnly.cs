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

using System.IO;

namespace SharpestBoy.Cart {
    class RomOnly : Mapper {

        public RomOnly(FileStream RomFile, Header Header) : base(RomFile, Header) { }

        public override bool Read(out byte value, int readAddress) {
            if(readAddress >= 0xA000) { //why not readAddress < 0xBFFF? Because the Cartridge is a mapped component AddMemoryMappedIORange(0xA000, 0xBFFF);
                //If there's no RAM module, the controller returns 0xFF
                value = 0xFF;
                return true;
            }
            value = ROM[readAddress];
            return true;
        }

        public override bool Write(byte value, int writeAddress) {
            //Nothing can be written
            return true;
        }
        
    }
}
