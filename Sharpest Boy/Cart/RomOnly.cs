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
