using System;
using SharpestBoy.Components;

namespace SharpestBoy.DMG.CPU.Units {
    [Serializable]
    public class InterruptService : Unit{

        int[] IRQLookup = new int[32];

        bool IME = false;
        bool Schedule = false;
        bool DoubleInstructionEnable = true;

        const ushort IF = 0xFF0F;
        const ushort IE = 0xFFFF;

        MemoryManagementUnit Memory;
        Board Board;
        Registers Registers;

        public InterruptService(DMGCPU dmgcpu) : base(dmgcpu) {
            for (int i = 0; i <= 0x1F; i++) {
                if ((i & 0x10) == 0x10) IRQLookup[i] = 4;
                if ((i & 0x8) == 0x8) IRQLookup[i] = 3;
                if ((i & 0x4) == 0x4) IRQLookup[i] = 2;
                if ((i & 0x2) == 0x2) IRQLookup[i] = 1;
                if ((i & 0x1) == 0x1) IRQLookup[i] = 0;
                if (i == 0) IRQLookup[i] = -1;
            }
            Board = dmgcpu.GetBoard();
            Memory = Board.GetMemoryManagementUnit();
            Registers = dmgcpu.Registers;

        }
        
        public void InterruptServiceRoutine() {
            byte bIF = Memory.DirectRead(IF);
            byte bIE = Memory.DirectRead(IE);
            int f = IRQLookup[bIF & bIE & 0x1F]; //-1 means no interrupt

            if (IME) {
                if(f != -1) {
                    //2 Internal delays
                    Board.AdvanceSystemTime(); //4
                    Board.AdvanceSystemTime(); //8

                    //PUSH PC
                    Board.AdvanceSystemTime(); //12
                    Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                    
                    Board.AdvanceSystemTime(); //16
                    Memory.Write((byte)Registers.PC, --Registers.SP);

                    //SERVE
                    bIF = ServeIRQ(f, bIF);

                    Board.AdvanceSystemTime(); //20
                    Memory.DirectWrite(bIF, IF);

                    if (GetCPU().Halt) {
                        Board.AdvanceSystemTime(); //24
                        GetCPU().Halt = false;
                    }

                    SetIME(false);
                }
            } else {
                if (f != -1) {
                    if (GetCPU().Halt) {
                        Board.AdvanceSystemTime(); //4
                        GetCPU().Halt = false;
                        GetCPU().DoubleInstructionExecutionBug = DoubleInstructionEnable;
                        DoubleInstructionEnable = true;
                    }
                } else {
                    DoubleInstructionEnable = false;
                }
            }

            if (Schedule) {
                IME = true;
                Schedule = false;
            }
        }

        public bool GetIME() {
            return IME;
        }

        /// <summary>
        /// Enables IME flag immediately
        /// </summary>
        /// <param name="flag"></param>
        public void SetIME(bool flag) {
            IME = flag;
        }

        /// <summary>
        /// Will enable IME flag the second time InterruptServiceRoutine is called. Typical behaviour of EI
        /// </summary>
        public void ScheduleEnableIME() {
            Schedule = true;
        }

        private byte ServeIRQ(int f, byte bIF) {
            switch (f) {
                case 0:
                    Registers.PC = 0x40;
                    return (byte)(bIF & 0xFE);
                case 1:
                    Registers.PC = 0x48;
                    return (byte)(bIF & 0xFD);
                case 2:
                    Registers.PC = 0x50;
                    return (byte)(bIF & 0xFB);
                case 3:
                    Registers.PC = 0x58;
                    return (byte)(bIF & 0xF7);
                case 4:
                    Registers.PC = 0x60;
                    return (byte)(bIF & 0xEF);
            }

            return bIF;
        }

        public override string ToString() {
            return String.Format("IME:\t\t{0}\nIF:\t\t{1:X2}\nIE:\t\t{2:X2}", IME, Memory.DirectRead(IF), Memory.DirectRead(IE));

        }

    }
}
