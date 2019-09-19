using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SharpestBoy.DMG.CPU;

namespace SharpestBoy.DMG.CPU.Units {
    [Serializable]
    public class Registers : Unit{

        /// <summary>
        /// Union like approach for the combined registers.
        /// Ex. From AF, left is A and right is F. Both is AF
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct RegPair {
            [FieldOffset(1)] public byte Left;
            [FieldOffset(0)] public byte Right;
            [FieldOffset(0)] public ushort Both;
        }

        private RegPair AF;
        private RegPair BC;
        private RegPair DE;
        private RegPair HL;

        public Registers(DMGCPU dmgcpu) : base(dmgcpu) {
            //Program Counter begins at 0x100
            PC = 0x100;

            //Stack pointer starts from FFFE
            SP = 0xFFFE;

            AF = new RegPair { Both = 0x0100 };
            BC = new RegPair { Both = 0xFF13 };
            DE = new RegPair { Both = 0x00C1 };
            HL = new RegPair { Both = 0x8403 };

        }

        //Program Counter
        public ushort PC;
        //Stack Pointer
        public ushort SP;

        //Registers (split as pairs)
        public Byte A {
            get {
                return AF.Left;
            } set {
                AF.Left = value;
            }
        }

        public Byte F {
            get {
                return (byte)(AF.Right & 0xF0);
            }
            set {
                AF.Right = value;
            }
        }

        public Byte B {
            get {
                return BC.Left;
            }
            set {
                BC.Left = value;
            }
        }

        public Byte C {
            get {
                return BC.Right;
            }
            set {
                BC.Right = value;
            }
        }

        public Byte D {
            get {
                return DE.Left;
            }
            set {
                DE.Left = value;
            }
        }

        public Byte E {
            get {
                return DE.Right;
            }
            set {
                DE.Right = value;
            }
        }

        public Byte H {
            get {
                return HL.Left;
            }
            set {
                HL.Left = value;
            }
        }

        public Byte L {
            get {
                return HL.Right;
            }
            set {
                HL.Right = value;
            }
        }

        public void SetAF(ushort value) {
            AF.Both = value;
        }

        public void SetBC(ushort value) {
            BC.Both = value;
        }

        public void SetDE(ushort value) {
            DE.Both = value;
        }

        public void SetHL(ushort value) {
            HL.Both = value;
        }

        public void IncHL() {
            HL.Both++;
        }

        public void DecHL() {
            HL.Both--;
        }

        public ushort GetAF() => AF.Both;

        public ushort GetBC() => BC.Both;

        public ushort GetDE() => DE.Both;

        public ushort GetHL() => HL.Both;

        /// <summary>
        /// This bit is set when the result of a math operation is zero or two values match when using the CP instruction.
        /// </summary>
        public void SetFlagZ(bool status) {
            if (status) F |= 0x80;
            else F &= 0x70; //bitmask 01110000
        }

        /// <summary>
        /// This bit is set if a subtraction was performed in the last math instruction.
        /// </summary>
        public void SetFlagN(bool status) {
            if (status) F |= 0x40;
            else F &= 0xB0;
        }

        /// <summary>
        /// This bit is set if a carry occurred from the lower nibble in the last math operation. (Game Boy ALU makes 4 bit calculations)
        /// </summary>
        public void SetFlagH(bool status) {
            if (status) F |= 0x20;
            else F &= 0xD0;
        }

        /// <summary>
        /// This bit is set if a carry occurred from the last math operation or if register A is the smaller value when executing the CP instruction.
        /// </summary>
        public void SetFlagC(bool status) {
            if (status) F |= 0x10;
            else F &= 0xE0;
        }

        public byte GetFlagZ() => (byte)(F >> 7);

        public byte GetFlagN() => (byte)((F & 0x40) >> 6);

        public byte GetFlagH() => (byte)((F & 0x20) >> 5);

        public byte GetFlagC() => (byte)((F & 0x10) >> 4);
    }
}
