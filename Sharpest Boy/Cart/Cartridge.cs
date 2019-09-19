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
using System.IO;
using SharpestBoy.Components;
using SharpestBoy.Exceptions;

namespace SharpestBoy.Cart {
    class Cartridge : MappedComponent {

        private Header Header;
        private Mapper Mapper;
        private String RomPath;

        public Cartridge(FileStream RomFile) {
            AddMemoryMappedIORange(0, 0x7FFF);
            AddMemoryMappedIORange(0xA000, 0xBFFF);

            Header = new Header(RomFile);
            switch (Header.CartridgeType) {
                case 0:
                    Mapper = new RomOnly(RomFile, Header);
                    break;
                case 1:
                case 2:
                case 3:
                    Mapper = new MBC1(RomFile, Header);
                    break;
                case 0x11:
                case 0x12:
                case 0x13:
                    Mapper = new MBC3(RomFile, Header);
                    break;
                case 0x1B:
                    Mapper = new MBC5(RomFile, Header);
                    break;
                default:
                    throw new UnknownCartridgeType(String.Format("Cartridge type {0} is not supported", Header.CartridgeTypeFriendly));
            }

            RomPath = RomFile.Name;
        }
        
        public override void Initialize() {
            
        }

        public override bool MMIORead(out byte value, int readAddress) {
            return Mapper.Read(out value, readAddress);
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            return Mapper.Write(value, writeAddress);
        }

        public Header GetHeader() {
            return Header;
        }

        public Mapper GetMapper() {
            return Mapper;
        }

        public String GetRomPath() {
            return RomPath;
        }

        public override string ToString() {
            return String.Format("Path:\t\t{0}\nType:\t\t{1}\n{2}\n{3}",
                RomPath, Mapper.GetType().Name, Mapper.ToString(), Header.ToString());
        }
    }
}
