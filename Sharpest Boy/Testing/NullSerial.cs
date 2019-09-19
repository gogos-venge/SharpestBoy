using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpestBoy.Components;

namespace SharpestBoy.DMG {
    class NullSerial : Peripheral {

        const ushort _SB = 0xFF01;
        const ushort _SC = 0xFF02;

        byte SC = 0;
        byte SB = 0;

        bool TransferSignal = false;
        int Clock = 0;
        MemoryManagementUnit Memory;

        public NullSerial() {
            AddMemoryMappedIORange(0xFF01, 0xFF02);
        }

        public override void Initialize() {
            Memory = GetBoard().GetMemoryManagementUnit();
        }

        public override bool MMIORead(out byte value, int readAddress) {
            value = 0;
            switch (readAddress) {
                case _SC:
                    value = (byte)(SC | 0x7C);
                    break;
                case _SB:
                    value = SB;
                    break;
            }
            return true;
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            switch (writeAddress) {
                case _SC:
                    TransferSignal = (value & 0x81) == 0x81;
                    SC = value;
                    break;
                case _SB:
                    SB = value;
                    break;
            }
            return true;
        }

        public override void Update(int Clocks) {
            if (TransferSignal) {
                Clock -= Clocks;
                if (Clock <= 0) {
                    SC &= 0x7F;
                    SB = 0xFF;
                    Memory.DirectWrite((byte)(Memory.DirectRead(0xFF0F) | 0x8), 0xFF0F);
                    TransferSignal = false;
                    Clock = 4096;
                }
            }
        }
    }
}
