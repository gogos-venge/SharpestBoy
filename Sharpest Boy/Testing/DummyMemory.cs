using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpestBoy.Components;

namespace SharpestBoy.Testing {
    class DummyMemory : MemoryManagementUnit {
        Random r = new Random();

        public override void Initialize() {
            
        }

        public override byte DirectRead(ushort address) {
            return 0;
            //throw new NotImplementedException();
        }

        public override void DirectWrite(byte b, ushort address) {
            //throw new NotImplementedException();
        }

        public override byte Read(ushort address) {
            if(RouteMMIOReads(out byte b, address)) {
                return b;
            }
            return (byte)r.Next(255);
        }

        public override void Write(byte b, ushort address) {
            if (!RouteMMIOWrites(b, address)) {
                Console.WriteLine("Writing {0:X2} at ${1:X4}", b, address);
            } else {
                Console.WriteLine("Writing {0:X2} to a MMIO ${1:X4}", b, address);
            }
            
        }
    }
}
