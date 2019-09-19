using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharpestBoy.Cart {
    class Header {

        public String Title { get; set; }
        public int CGBFlag { get; set; }
        public byte[] NewLicenseCode { get; set; }
        public int SGBFlag { get; set; }
        public int CartridgeType { get; set; }
        public int ROMSize { get; set; }
        public int RAMSize { get; set; }
        public int DestinationCode { get; set; }
        public int OldLicenseCode { get; set; }
        public int ROMVersion { get; set; }
        public int HeaderChecksum { get; set; }
        public byte[] GlobalChecksum { get; set; }
        public String CartridgeTypeFriendly { get; set; }

        public Header(FileStream RomFile) {
            //Ugly code due to managed mem. This whole thing could be a simple struct
            char c;

            //Title
            Title = String.Empty;
            RomFile.Seek(0x134, SeekOrigin.Begin);
            while ((c = (char)RomFile.ReadByte()) != 0) {
                Title += c;
            }

            //CGBFlag
            RomFile.Seek(0x143, SeekOrigin.Begin);
            CGBFlag = RomFile.ReadByte();

            //New License Code
            NewLicenseCode = new byte[2];
            RomFile.Read(NewLicenseCode, 0, 2);

            //SGBFlag
            SGBFlag = RomFile.ReadByte();

            //Cartridge Type
            CartridgeType = RomFile.ReadByte();

            //Rom Size
            ROMSize = RomFile.ReadByte();

            //Ram Size
            RAMSize = RomFile.ReadByte();

            //Destination Code
            DestinationCode = RomFile.ReadByte();

            //Old License Code
            OldLicenseCode = RomFile.ReadByte();

            //Mask Rom Version Number
            ROMVersion = RomFile.ReadByte();

            //Header Cheksum
            HeaderChecksum = RomFile.ReadByte();

            //Global Checksum
            GlobalChecksum = new byte[2];
            RomFile.Read(GlobalChecksum, 0, 2);

            //Friendly Cartridge Type
            CartridgeTypeFriendly = GetCartridgeTypeFriendly(CartridgeType);
        }

        public String GetCartridgeTypeFriendly(int mbcByte) {
            switch (mbcByte) {
                case 0: return "ROM ONLY";
                case 1: return "MBC1";
                case 2: return "MBC1+RAM";
                case 3: return "MBC1+RAM+BATTERY";
                case 5: return "MBC2";
                case 6: return "MBC2+BATTERY";
                case 8: return "ROM+RAM";
                case 9: return "ROM+RAM+BATTERY";
                case 0xB: return "MMM01";
                case 0xC: return "MMM01+RAM";
                case 0xD: return "MMM01+RAM+BATTERY";
                case 0xF: return "MBC3+TIMER+BATTERY";
                case 0x10: return "MBC3+TIMER+RAM+BATTERY";
                case 0x11: return "MBC3";
                case 0x12: return "MBC3+RAM";
                case 0x13: return "MBC3+RAM+BATTERY";
                case 0x15: return "MBC4";
                case 0x16: return "MBC4+RAM";
                case 0x17: return "MBC4+RAM+BATTERY";
                case 0x19: return "MBC5";
                case 0x1A: return "MBC5+RAM";
                case 0x1B: return "MBC5+RAM+BATTERY";
                case 0x1C: return "MBC5+RUMBLE";
                case 0x1D: return "MBC5+RUMBLE+RAM";
                case 0x1E: return "MBC5+RUMBLE+RAM+BATTERY";
                case 0xFC: return "POCKET CAMERA";
                case 0xFD: return "BANDAI TAMA5";
                case 0xFE: return "HuC3";
                case 0xFF: return "HuC1+RAM+BATTERY";
            }
            return "Unknown";
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo pi in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)) {
                object value = pi.GetValue(this, null);
                if (value is int || value is bool || value is byte || value is ushort)
                    sb.Append(string.Format("{0}\t{1:X}\n", pi.Name, value));
            }
            return sb.ToString();
        }
    }
}
