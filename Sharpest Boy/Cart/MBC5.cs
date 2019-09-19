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
using System.IO;

namespace SharpestBoy.Cart {
    class MBC5 : Mapper {


        bool Register0 = false; //RAM protection
        byte Register1 = 0; //ROM bank code
        byte Register2 = 0; //Upper ROM bank code
        bool Register3 = false; //ROM/RAM change

        int ROMOffset = 0;
        int RAMOffset = 0;

        public MBC5(FileStream RomFile, Header Header) : base(RomFile, Header) { }


        public override bool Read(out byte value, int readAddress) {
            if (readAddress >= 0xA000 + RAMBankSize) {
                value = 0xFF;
                return true;
            }
            if (readAddress >= 0xA000) {
                //Writing data to RAM
                if (Register0) {
                    int address = (readAddress & 0x1FFF) | RAMOffset;
                    value = RAM[address];
                }
                else {
                    value = 0xFF;
                }
            }
            else if (readAddress >= 0x4000) {
                value = ROM[readAddress + ROMOffset];
            }
            else {
                int Mode1Offset = 0;
                if (Register3) {
                    int Bank = Register2 << 5;
                    if (Bank % 0x20 != 0) {
                        Bank++;
                    }
                    Bank &= (ROMTotalBanks - 1);
                    Mode1Offset = Bank * ROMBankSize;
                }
                value = ROM[readAddress + Mode1Offset];
            }
            return true;
        }

        public override bool Write(byte value, int writeAddress) {
            if (writeAddress >= 0xA000 + RAMBankSize) {
                return true;
            }
            if (writeAddress >= 0xA000) {
                //Writing data to RAM
                if (Register0) {
                    int address = (writeAddress & 0x1FFF) | RAMOffset;
                    RAM[address] = value;
                }
            }
            else if (writeAddress >= 0x6000) {
                //Register 3: ROM/RAM change
                Register3 = (value & 0x1) == 1;

            }
            else if (writeAddress >= 0x4000) {
                //Register 2: Upper ROM bank code when using 8 Mbits or more of ROM (and register 3 is 0)
                Register2 = value;
                Remap();

            }
            else if (writeAddress >= 0x2000) {
                //Register 1: ROM bank code
                Register1 = value;
                Remap();

            }
            else {
                //Register 0: RAMCS gate data (serves as write-protection for RAM)
                Register0 = (value & 0x0F) == 0x0A;

            }

            return true;
        }

        private void Remap() {
            if (Register3) {
                int Bank = Register2 & (RAMTotalBanks - 1);
                RAMOffset = Bank << 0xD;
            }
            else {
                RAMOffset = 0;
            }

            int BankValue = (Register2 << 8) | Register1;
            int AdjustedRomBank = BankValue & (ROMTotalBanks - 1);
            AdjustedRomBank = AdjustedRomBank == 0 ? 1 : AdjustedRomBank;
            ROMOffset = (AdjustedRomBank - 1) * ROMBankSize;
        }

    }
}
