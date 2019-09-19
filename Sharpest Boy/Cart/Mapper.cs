using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpestBoy.Cart {
    abstract class Mapper {
        protected byte[] ROM;
        protected byte[] RAM;

        protected int ROMSize;
        protected int ROMBankSize;
        protected int ROMTotalBanks;

        protected int RAMSize;
        protected int RAMBankSize;
        protected int RAMTotalBanks;

        protected Mapper(FileStream RomFile, Header Header) {
            //Always 0x4000
            ROMBankSize = 0x4000;

            switch (Header.ROMSize) {
                case 0:
                    //Corrupted Rom. Assuming 2
                    ROMTotalBanks = 2;
                    break;
                case 1:
                    ROMTotalBanks = 4;
                    break;
                case 2:
                    ROMTotalBanks = 8;
                    break;
                case 3:
                    ROMTotalBanks = 16;
                    break;
                case 4:
                    ROMTotalBanks = 32;
                    break;
                case 5:
                    ROMTotalBanks = 64;
                    break;
                case 6:
                    ROMTotalBanks = 128;
                    break;
                case 7:
                    ROMTotalBanks = 256;
                    break;
                case 8:
                    ROMTotalBanks = 512;
                    break;
                case 0x52:
                    ROMTotalBanks = 72;
                    break;
                case 0x53:
                    ROMTotalBanks = 80;
                    break;
                case 0x54:
                    ROMTotalBanks = 96;
                    break;
            }

            ROMSize = ROMTotalBanks * ROMBankSize;

            int FinalRomSize = (int)Math.Max(ROMSize, RomFile.Length);
            ROM = LoadRom(RomFile, FinalRomSize);
            
            switch (Header.RAMSize) {
                case 1:
                    RAMSize = 0x800;
                    RAMBankSize = 0x800;
                    RAMTotalBanks = 1;
                    break;
                case 2:
                    RAMSize = 0x2000;
                    RAMBankSize = 0x2000;
                    RAMTotalBanks = 1;
                    break;
                case 3:
                    RAMSize = 0x8000;
                    RAMBankSize = 0x2000;
                    RAMTotalBanks = 4;
                    break;
                case 4:
                    RAMSize = 0x20000;
                    RAMBankSize = 0x2000;
                    RAMTotalBanks = 16;
                    break;
                case 5:
                    RAMSize = 0x10000;
                    RAMBankSize = 0x2000;
                    RAMTotalBanks = 8;
                    break;
            }
            RAM = Enumerable.Repeat<Byte>(0x00, RAMSize).ToArray();
        }

        abstract public bool Read(out byte value, int readAddress);

        abstract public bool Write(byte value, int writeAddress);

        private byte[] LoadRom(FileStream RomFile, int RomSize) {
            byte[] tROM = Enumerable.Repeat<Byte>(0xFF, RomSize).ToArray();
            RomFile.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[0x100];
            int index = 0, length;
            while ((length = RomFile.Read(buffer, 0, buffer.Length)) != 0) {
                Array.Copy(buffer, 0, tROM, index, length);
                index += length;
            }
            return tROM;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
                object value = fi.GetValue(this);
                if (value is int || value is bool || value is byte || value is ushort)
                    sb.Append(System.String.Format("{0}\t{1:X}\n", fi.Name, value));
            }
            return sb.ToString();
        }
    }
}
