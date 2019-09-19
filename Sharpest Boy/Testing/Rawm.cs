using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpestBoy.Components;
using System.IO;

namespace SharpestBoy.Testing {
    class Rawm : MemoryManagementUnit {

        byte[] Memory;

        public override void Initialize() {
            Memory = Enumerable.Repeat<Byte>(0x00, 0x10000).ToArray();
            Write(0x1, 0xFF0F);
            Write(0x91, 0xFF40);
            Write(0x81, 0xFF41);
        }

        public override byte Read(ushort address) {
            if (RouteMMIOReads(out byte b, address)) {
                return b;
            }
            if(address == 0xFF0F) {
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
