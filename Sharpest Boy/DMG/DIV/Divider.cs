using System.IO;
using SharpestBoy.Components;
using SharpestBoy.Circuits;
using SharpestBoy.DMG.CPU;
using System.Text;
using System.Reflection;

namespace SharpestBoy.DMG.DIV {
    class Divider : Peripheral {

        const ushort _DIV = 0xFF04;
        const ushort _TIMA = 0xFF05;
        const ushort _TMA = 0xFF06;
        const ushort _TAC = 0xFF07;
        const ushort IF = 0xFF0F;

        private ushort DIV = 0xABCC;
        private byte TIMA = 0;
        private byte TMA = 0;
        private byte TAC = 0;

        private int MASK = 0;

        private bool TIMACARRY = false;
        private bool Overflowing = false;
        private bool ReleaseOverflow = false;

        private FallingEdgeDetector FallingEdgeDetector;
        private MemoryManagementUnit Memory;
        
        public Divider() {
            AddMemoryMappedIORange(0xFF04, 0xFF07);
            FallingEdgeDetector = new FallingEdgeDetector();
        }

        public override void Initialize() {
            Memory = GetBoard().GetMemoryManagementUnit();
        }

        public override bool MMIORead(out byte value, int readAddress) {
            switch (readAddress) {
                case _DIV:
                    value = (byte)(DIV >> 8);
                    return true;
                case _TIMA:
                    value = TIMA;
                    return true;
                case _TMA:
                    value = TMA;
                    return true;
                case _TAC:
                    value = (byte)(TAC | 0xF8);
                    return true;
            }
            value = 0;
            return false;
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            switch (writeAddress) {
                case _DIV:
                    DIV = 0;
                    Update(0);
                    return true;
                case _TIMA:
                    //During this moment, we cannot affect TIMA by directly writing to it.
                    if (ReleaseOverflow) return true;
                    TIMA = value;
                    TIMACARRY = false;
                    Overflowing = false;
                    ReleaseOverflow = false;
                    return true;
                case _TMA:
                    //During this moment, TIMA will be overwritten with anything written at TMA.
                    if (ReleaseOverflow) {
                        TIMA = value;
                    }
                    TMA = value;
                    return true;
                case _TAC:

                    switch (value & 3) {
                        case 0:
                            MASK = 0x200; //Selects 9th bit
                            break;
                        case 3:
                            MASK = 0x80; //Selects 7th bit
                            break;
                        case 2:
                            MASK = 0x20; //Selects 5th bit
                            break;
                        case 1:
                            MASK = 0x8; //Selects 3rd bit
                            break;
                    }

                    TAC = (byte)(value & 7);

                    //not sure about this, but changing TAC **SEEMS** to affect TIMA overflow immediately.
                    Update(0);
                    Update(0);
                    return true;
            }
            
            return false;
        }

        /**
         * "It (DIV) works by using an internal system 16 bit counter. The counter increases each clock (4 clocks per
         * nop) and the value of DIV is the 8 upper bits of the counter: it increases every 256 oscillator clocks.
         * The value of DIV is the actual bits of the system internal counter, not a mirror, not a register that
         * increases with the system internal counter: The actual bits of the counter mapped to memory."
         * Source: AntonioND, The Cycle-Accurate Game Boy Docs
         * 
         * "The signal used to increase the TIMA register is generated in a weird way. It selects the actual value
         * of one bit of the system internal counter, and performs an and operation with the enable bit in TAC.
         * This means that writing to the DIV register affects the timer too (writing to timer registers doesn't
         * affect the DIV register)."
         * Source: AntonioND, The Cycle-Accurate Game Boy Docs
         * 
         * Tima is not blackbox so we can actually implement what Antonio mentions in his Docs about TIMA and indeed:
         * bool TIMASIGNAL = (((DIVREG & MASK) / MASK) & ((TACREG & 4) / 4)) != 0;
         * or to avoid the costly divisions: TIMASIGNAL = (DIV & MASK) == MASK && (TAC & 4) == 4;
         * TIMASIGNAL is true when the circuit is HI. TIMA increases if the FallingEdgeDetector
         * detects a 1 -> 0 transition of TIMASIGNAL.
         */
        public override void Update(int clocks) {

            DIV = (ushort)(clocks + DIV);
            
            bool TIMASIGNAL = (DIV & MASK) == MASK && (TAC & 4) == 4;

            if (ReleaseOverflow) {
                //TIME: 8
                Overflowing = false;
                ReleaseOverflow = false;
            }

            //After the brief period, TIMA will do its regular routine.
            if (Overflowing) {
                //TIME: 4
                TIMA = TMA;
                Memory.DirectWrite((byte)(Memory.DirectRead(IF) | 0x4), IF);
                TIMACARRY = false;
                ReleaseOverflow = true;
            }

            if (FallingEdgeDetector.Check(TIMASIGNAL)) {
                TIMA++;
                //For a brief period (1 cycle or 4 clocks) TIMA has the value 0, and it's currently overflowing (AntonioND)
                if (TIMA == 0x0 && TIMACARRY) {
                    //TIME: 0
                    Overflowing = true;
                }
                else if (TIMA == 0xFF) {
                    //about to overflow
                    TIMACARRY = true;
                }
                
            }
            
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
                object value = fi.GetValue(this);
                if(value is int || value is bool || value is byte || value is ushort)
                sb.Append(System.String.Format("{0}\t{1:X}\n", fi.Name, value));
            }
            return sb.ToString();
        }

    }
}
