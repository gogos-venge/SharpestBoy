using SharpestBoy.Components;
using System;
using System.Linq;

namespace SharpestBoy.DMG {
    class MMU : MemoryManagementUnit {
        
        byte[] Memory;

        public override void Initialize() {
            Memory = Enumerable.Repeat<Byte>(0, 0x10000).ToArray();

            //Random values in memory
            Random r = new Random();
            for(int i = 0x8000; i < 0xFF00; i++) {
                Memory[i] = (byte)r.Next();
            }

            Write(0x1, 0xFF0F);
        }

        public override byte Read(ushort address) {
            if (RouteMMIOReads(out byte b, address)) {
                return b;
            }
            if (address == 0xFF0F) {
                return (byte)(Memory[address] | 0xE0);
            }
            if (address == 0xFFFF) {
                return (byte)(Memory[address] | 0xE0);
            }
            return Memory[address];
        }

        public override void Write(byte b, ushort address) {
            if (!RouteMMIOWrites(b, address)) {
                if (address >= 0x8000) {
                    Memory[address] = b;
                }
            }
        }

        public override byte DirectRead(ushort address) {
            return Memory[address];
        }

        public override void DirectWrite(byte b, ushort address) {
            Memory[address] = b;
        }
    }
}
