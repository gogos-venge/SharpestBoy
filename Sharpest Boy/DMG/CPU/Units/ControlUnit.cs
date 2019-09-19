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
using System.Text;
using SharpestBoy.Components;
using SharpestBoy.Exceptions;

namespace SharpestBoy.DMG.CPU.Units {
    public class ControlUnit : Unit {

        private Action[] Normal;
        private Action[] Cb;
        private Board Board;
        private Registers Registers;
        private MemoryManagementUnit Memory;
        private ArithmeticLogicUnit ALU;

        public ControlUnit(DMGCPU Dmgcpu) : base(Dmgcpu) {
            Normal = new Action[256];
            Cb = new Action[256];
            Board = Dmgcpu.GetBoard();
            Memory = Board.GetMemoryManagementUnit();
            Registers = Dmgcpu.Registers;
            ALU = Dmgcpu.ALU;

            NormalInstructionsPopulate();
            CbInstructionsPopulate();
        }

        public void Execute(int opcode) {
            Normal[opcode]();
        }

        private void NormalInstructionsPopulate() {
            //At this point CPU has already fetched the opcode so all instructions are 4 clocks SHORTER than what's described in the documentation

            Normal[0x00] = delegate {
                //NOP
                Registers.PC++;
            };

            Normal[0x01] = delegate {
                //LD BC,d16
                Board.AdvanceSystemTime(); //8
                ushort LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Registers.SetBC((ushort)(LSB | Memory.Read(++Registers.PC) << 8));

                Registers.PC++;
            };

            Normal[0x02] = delegate {
                //LD (BC),A
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, Registers.GetBC());

                Registers.PC++;
            };

            Normal[0x03] = delegate {
                //INC BC
                Board.AdvanceSystemTime(); //8
                Registers.SetBC(ALU.INC_16(Registers.GetBC()));

                Registers.PC++;
            };

            Normal[0x04] = delegate {
                //INC B
                Registers.B = ALU.INC(Registers.B);
                Registers.PC++;
            };

            Normal[0x05] = delegate {
                //DEC B
                Registers.B = ALU.DEC(Registers.B);
                Registers.PC++;
            };

            Normal[0x06] = delegate {
                //LD B, d8
                Board.AdvanceSystemTime(); //8
                Registers.B = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x07] = delegate {
                //RLCA
                Registers.A = ALU.RLCA(Registers.A);

                Registers.PC++;
            };

            Normal[0x08] = delegate {
                //LD (a16),SP
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.SP, address);

                Board.AdvanceSystemTime(); //20
                Memory.Write((byte)(Registers.SP >> 8), ++address);

                Registers.PC++;
            };

            Normal[0x09] = delegate {
                //ADD HL,BC
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.ADD16(Registers.GetHL(), Registers.GetBC()));

                Registers.PC++;
            };

            Normal[0x0A] = delegate {
                //LD A,(BC)
                Board.AdvanceSystemTime(); //8
                byte next = Memory.Read(Registers.GetBC());
                Registers.A = next;

                Registers.PC++;
            };

            Normal[0x0B] = delegate {
                //DEC BC
                Board.AdvanceSystemTime(); //8
                Registers.SetBC(ALU.DEC_16(Registers.GetBC()));

                Registers.PC++;
            };

            Normal[0x0C] = delegate {
                //INC C
                Registers.C = ALU.INC(Registers.C);

                Registers.PC++;
            };

            Normal[0x0D] = delegate {
                //DEC C
                Registers.C = ALU.DEC(Registers.C);
                Registers.PC++;
            };

            Normal[0x0E] = delegate {
                //LD C, d8
                Board.AdvanceSystemTime(); //8
                Registers.C = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x0F] = delegate {
                //RRCA
                Registers.A = ALU.RRCA(Registers.A);

                Registers.PC++;
            };

            Normal[0x10] = delegate {
                //STOP
                Registers.PC += 2;
            };

            Normal[0x11] = delegate {
                //LD DE,d16
                Board.AdvanceSystemTime(); //8
                ushort LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Registers.SetDE((ushort)(LSB | Memory.Read(++Registers.PC) << 8));

                Registers.PC++;
            };

            Normal[0x12] = delegate {
                //LD (DE),A
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, Registers.GetDE());

                Registers.PC++;
            };

            Normal[0x13] = delegate {
                //INC DE
                Board.AdvanceSystemTime(); //8
                Registers.SetDE(ALU.INC_16(Registers.GetDE()));

                Registers.PC++;
            };

            Normal[0x14] = delegate {
                //INC D
                Registers.D = ALU.INC(Registers.D);
                Registers.PC++;
            };

            Normal[0x15] = delegate {
                //DEC D
                Registers.D = ALU.DEC(Registers.D);
                Registers.PC++;
            };

            Normal[0x16] = delegate {
                //LD D, d8
                Board.AdvanceSystemTime(); //8
                Registers.D = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x17] = delegate {
                //RLCA
                Registers.A = ALU.RLA(Registers.A);

                Registers.PC++;
            };

            Normal[0x18] = delegate {
                //JR r8
                Board.AdvanceSystemTime(); //8
                sbyte n = (sbyte)Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                JUMP((ushort)(Registers.PC + n));
                
                //Signed jump adjustment
                Registers.PC++;
            };

            Normal[0x19] = delegate {
                //ADD HL,DE
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.ADD16(Registers.GetHL(), Registers.GetDE()));

                Registers.PC++;
            };

            Normal[0x1A] = delegate {
                //LD A,(DE)
                Board.AdvanceSystemTime(); //8
                byte next = Memory.Read(Registers.GetDE());
                Registers.A = next;

                Registers.PC++;
            };

            Normal[0x1B] = delegate {
                //DEC DE
                Board.AdvanceSystemTime(); //8
                Registers.SetDE(ALU.DEC_16(Registers.GetDE()));

                Registers.PC++;
            };

            Normal[0x1C] = delegate {
                //INC E
                Registers.E = ALU.INC(Registers.E);

                Registers.PC++;
            };

            Normal[0x1D] = delegate {
                //DEC E
                Registers.E = ALU.DEC(Registers.E);
                Registers.PC++;
            };

            Normal[0x1E] = delegate {
                //LD E, d8
                Board.AdvanceSystemTime(); //8
                Registers.E = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x1F] = delegate {
                //RRA
                Registers.A = ALU.RRA(Registers.A);

                Registers.PC++;
            };

            Normal[0x20] = delegate {
                //JR NZ,r8
                Board.AdvanceSystemTime(); //8
                sbyte n = (sbyte)Memory.Read(++Registers.PC);

                if (Registers.GetFlagZ() == 0) {
                    Board.AdvanceSystemTime(); //12
                    JUMP((ushort)(Registers.PC + n));
                }

                Registers.PC++;
            };

            Normal[0x21] = delegate {
                //LD HL,d16
                Board.AdvanceSystemTime(); //8
                ushort LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Registers.SetHL((ushort)(LSB | Memory.Read(++Registers.PC) << 8));

                Registers.PC++;
            };

            Normal[0x22] = delegate {
                //LD (HL+),A
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, Registers.GetHL());
                Registers.IncHL();

                Registers.PC++;
            };

            Normal[0x23] = delegate {
                //INC HL
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.INC_16(Registers.GetHL()));

                Registers.PC++;
            };

            Normal[0x24] = delegate {
                //INC H
                Registers.H = ALU.INC(Registers.H);

                Registers.PC++;
            };

            Normal[0x25] = delegate {
                //DEC H
                Registers.H = ALU.DEC(Registers.H);

                Registers.PC++;
            };

            Normal[0x26] = delegate {
                //LD H, d8
                Board.AdvanceSystemTime(); //8
                Registers.H = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x27] = delegate {
                //DAA
                ALU.DAA();

                Registers.PC++;
            };

            Normal[0x28] = delegate {
                //JR Z,r8
                Board.AdvanceSystemTime(); //8
                sbyte n = (sbyte)Memory.Read(++Registers.PC);

                if (Registers.GetFlagZ() == 1) {
                    Board.AdvanceSystemTime(); //12
                    JUMP((ushort)(Registers.PC + n));
                }

                Registers.PC++;
            };

            Normal[0x29] = delegate {
                //ADD HL,HL
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.ADD16(Registers.GetHL(), Registers.GetHL()));

                Registers.PC++;
            };

            Normal[0x2A] = delegate {
                //LD A,(HL+)
                Board.AdvanceSystemTime(); //8
                Registers.A = Memory.Read(Registers.GetHL());
                Registers.IncHL();

                Registers.PC++;
            };

            Normal[0x2B] = delegate {
                //DEC HL
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.DEC_16(Registers.GetHL()));

                Registers.PC++;
            };

            Normal[0x2C] = delegate {
                //INC L
                Registers.L = ALU.INC(Registers.L);

                Registers.PC++;
            };

            Normal[0x2D] = delegate {
                //DEC L
                Registers.L = ALU.DEC(Registers.L);

                Registers.PC++;
            };

            Normal[0x2E] = delegate {
                //LD L, d8
                Board.AdvanceSystemTime(); //8
                Registers.L = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x2F] = delegate {
                //CPL
                ALU.CPL();

                Registers.PC++;
            };

            Normal[0x30] = delegate {
                //JR NC,r8
                Board.AdvanceSystemTime(); //8
                sbyte n = (sbyte)Memory.Read(++Registers.PC);

                if (Registers.GetFlagC() == 0) {
                    Board.AdvanceSystemTime(); //12
                    JUMP((ushort)(Registers.PC + n));
                }

                Registers.PC++;
            };

            Normal[0x31] = delegate {
                //LD SP,d16
                Board.AdvanceSystemTime(); //8
                ushort LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Registers.SP = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                Registers.PC++;
            };

            Normal[0x32] = delegate {
                //LD (HL-),A
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, Registers.GetHL());
                Registers.DecHL();

                Registers.PC++;
            };

            Normal[0x33] = delegate {
                //INC SP
                Board.AdvanceSystemTime(); //8
                Registers.SP++;

                Registers.PC++;
            };

            Normal[0x34] = delegate {
                //INC (HL)
                Board.AdvanceSystemTime(); //8
                byte register = Memory.Read(Registers.GetHL());
                Registers.SetFlagN(false);
                Registers.SetFlagH((register & 0x0F) + 1 > 0x0F);
                register++;
                Registers.SetFlagZ(register == 0);

                Board.AdvanceSystemTime(); //12
                Memory.Write(register, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x35] = delegate {
                //DEC (HL)
                Board.AdvanceSystemTime(); //8
                byte register = Memory.Read(Registers.GetHL());
                Registers.SetFlagN(true);
                Registers.SetFlagH((register & 0x0F) == 0);
                register--;
                Registers.SetFlagZ(register == 0);

                Board.AdvanceSystemTime(); //12
                Memory.Write(register, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x36] = delegate {
                //LD (HL),d8
                Board.AdvanceSystemTime(); //8
                byte n = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Memory.Write(n, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x37] = delegate {
                //SCF
                ALU.SCF();

                Registers.PC++;
            };

            Normal[0x38] = delegate {
                //JR C,r8
                Board.AdvanceSystemTime(); //8
                sbyte n = (sbyte)Memory.Read(++Registers.PC);

                if (Registers.GetFlagC() == 1) {
                    Board.AdvanceSystemTime(); //12
                    JUMP((ushort)(Registers.PC + n));
                }

                Registers.PC++;
            };

            Normal[0x39] = delegate {
                //ADD HL,SP
                Board.AdvanceSystemTime(); //8
                Registers.SetHL(ALU.ADD16(Registers.GetHL(), Registers.SP));

                Registers.PC++;
            };

            Normal[0x3A] = delegate {
                //LD A,(HL+)
                Board.AdvanceSystemTime(); //8
                Registers.A = Memory.Read(Registers.GetHL());
                Registers.DecHL();

                Registers.PC++;
            };

            Normal[0x3B] = delegate {
                //DEC SP

                Board.AdvanceSystemTime(); //8
                Registers.SP--;

                Registers.PC++;
            };

            Normal[0x3C] = delegate {
                //INC A
                Registers.A = ALU.INC(Registers.A);

                Registers.PC++;
            };

            Normal[0x3D] = delegate {
                //DEC A
                Registers.A = ALU.DEC(Registers.A);

                Registers.PC++;
            };

            Normal[0x3E] = delegate {
                //LD A, d8
                Board.AdvanceSystemTime(); //8
                Registers.A = Memory.Read(++Registers.PC);

                Registers.PC++;
            };

            Normal[0x3F] = delegate {
                //LD L, d8
                ALU.CCF();

                Registers.PC++;
            };

            Normal[0x40] = delegate {
                //LD B, B

                Registers.PC++;
            };

            Normal[0x41] = delegate {
                //LD B, B
                Registers.B = Registers.C;

                Registers.PC++;
            };

            Normal[0x42] = delegate {
                //LD B, B
                Registers.B = Registers.D;

                Registers.PC++;
            };

            Normal[0x43] = delegate {
                //LD B, B
                Registers.B = Registers.E;

                Registers.PC++;
            };

            Normal[0x44] = delegate {
                //LD B, B
                Registers.B = Registers.H;

                Registers.PC++;
            };

            Normal[0x45] = delegate {
                //LD B, B
                Registers.B = Registers.L;

                Registers.PC++;
            };

            Normal[0x46] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.B = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x47] = delegate {
                //LD B, B
                Registers.B = Registers.A;

                Registers.PC++;
            };

            Normal[0x48] = delegate {
                //LD C, B
                Registers.C = Registers.B;

                Registers.PC++;
            };

            Normal[0x49] = delegate {
                //LD C, C

                Registers.PC++;
            };

            Normal[0x4A] = delegate {
                //LD B, B
                Registers.C = Registers.D;

                Registers.PC++;
            };

            Normal[0x4B] = delegate {
                //LD B, B
                Registers.C = Registers.E;

                Registers.PC++;
            };

            Normal[0x4C] = delegate {
                //LD B, B
                Registers.C = Registers.H;

                Registers.PC++;
            };

            Normal[0x4D] = delegate {
                //LD B, B
                Registers.C = Registers.L;

                Registers.PC++;
            };

            Normal[0x4E] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.C = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x4F] = delegate {
                //LD B, B
                Registers.C = Registers.A;

                Registers.PC++;
            };

            Normal[0x50] = delegate {
                //LD B, B
                Registers.D = Registers.B;

                Registers.PC++;
            };

            Normal[0x51] = delegate {
                //LD B, B
                Registers.D = Registers.C;

                Registers.PC++;
            };

            Normal[0x52] = delegate {
                //LD B, B

                Registers.PC++;
            };

            Normal[0x53] = delegate {
                //LD B, B
                Registers.D = Registers.E;

                Registers.PC++;
            };

            Normal[0x54] = delegate {
                //LD B, B
                Registers.D = Registers.H;

                Registers.PC++;
            };

            Normal[0x55] = delegate {
                //LD B, B
                Registers.D = Registers.L;

                Registers.PC++;
            };

            Normal[0x56] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.D = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x57] = delegate {
                //LD B, B
                Registers.D = Registers.A;

                Registers.PC++;
            };

            Normal[0x58] = delegate {
                //LD C, B
                Registers.E = Registers.B;

                Registers.PC++;
            };

            Normal[0x59] = delegate {
                //LD C, C
                Registers.E = Registers.C;

                Registers.PC++;
            };

            Normal[0x5A] = delegate {
                //LD B, B
                Registers.E = Registers.D;

                Registers.PC++;
            };

            Normal[0x5B] = delegate {
                //LD B, B

                Registers.PC++;
            };

            Normal[0x5C] = delegate {
                //LD B, B
                Registers.E = Registers.H;

                Registers.PC++;
            };

            Normal[0x5D] = delegate {
                //LD B, B
                Registers.E = Registers.L;

                Registers.PC++;
            };

            Normal[0x5E] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.E = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x5F] = delegate {
                //LD B, B
                Registers.E = Registers.A;

                Registers.PC++;
            };

            Normal[0x60] = delegate {
                //LD B, B
                Registers.H = Registers.B;

                Registers.PC++;
            };

            Normal[0x61] = delegate {
                //LD B, B
                Registers.H = Registers.C;

                Registers.PC++;
            };

            Normal[0x62] = delegate {
                //LD B, B
                Registers.H = Registers.D;

                Registers.PC++;
            };

            Normal[0x63] = delegate {
                //LD B, B
                Registers.H = Registers.E;

                Registers.PC++;
            };

            Normal[0x64] = delegate {
                //LD B, B

                Registers.PC++;
            };

            Normal[0x65] = delegate {
                //LD B, B
                Registers.H = Registers.L;

                Registers.PC++;
            };

            Normal[0x66] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.H = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x67] = delegate {
                //LD B, B
                Registers.H = Registers.A;

                Registers.PC++;
            };

            Normal[0x68] = delegate {
                //LD C, B
                Registers.L = Registers.B;

                Registers.PC++;
            };

            Normal[0x69] = delegate {
                //LD C, C
                Registers.L = Registers.C;

                Registers.PC++;
            };

            Normal[0x6A] = delegate {
                //LD B, B
                Registers.L = Registers.D;

                Registers.PC++;
            };

            Normal[0x6B] = delegate {
                //LD B, B
                Registers.L = Registers.E;

                Registers.PC++;
            };

            Normal[0x6C] = delegate {
                //LD B, B
                Registers.L = Registers.H;

                Registers.PC++;
            };

            Normal[0x6D] = delegate {
                //LD B, B

                Registers.PC++;
            };

            Normal[0x6E] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.L = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x6F] = delegate {
                //LD B, B
                Registers.L = Registers.A;

                Registers.PC++;
            };

            Normal[0x70] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.B, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x71] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.C, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x72] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.D, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x73] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.E, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x74] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.H, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x75] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.L, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x76] = delegate {
                //HALT
                GetCPU().Halt = true;

                Registers.PC++;
            };

            Normal[0x77] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x78] = delegate {
                //LD C, B
                Registers.A = Registers.B;

                Registers.PC++;
            };

            Normal[0x79] = delegate {
                //LD C, C
                Registers.A = Registers.C;

                Registers.PC++;
            };

            Normal[0x7A] = delegate {
                //LD B, B
                Registers.A = Registers.D;

                Registers.PC++;
            };

            Normal[0x7B] = delegate {
                //LD B, B
                Registers.A = Registers.E;

                Registers.PC++;
            };

            Normal[0x7C] = delegate {
                //LD B, B
                Registers.A = Registers.H;

                Registers.PC++;
            };

            Normal[0x7D] = delegate {
                //LD B, B
                Registers.A = Registers.L;

                Registers.PC++;
            };

            Normal[0x7E] = delegate {
                //LD B, B
                Board.AdvanceSystemTime(); //8
                Registers.A = Memory.Read(Registers.GetHL());

                Registers.PC++;
            };

            Normal[0x7F] = delegate {
                //LD B, B
                Registers.PC++;
            };

            Normal[0x80] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0x81] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0x82] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0x83] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0x84] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0x85] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0x86] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.ADD8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0x87] = delegate {
                Registers.A = ALU.ADD8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0x88] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0x89] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0x8A] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0x8B] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0x8C] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0x8D] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0x8E] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.ADC8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0x8F] = delegate {
                Registers.A = ALU.ADC8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0x90] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0x91] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0x92] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0x93] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0x94] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0x95] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0x96] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.SUB8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0x97] = delegate {
                Registers.A = ALU.SUB8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0x98] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0x99] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0x9A] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0x9B] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0x9C] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0x9D] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0x9E] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.SBC8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0x9F] = delegate {
                Registers.A = ALU.SBC8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0xA0] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0xA1] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0xA2] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0xA3] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0xA4] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0xA5] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0xA6] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.AND8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0xA7] = delegate {
                Registers.A = ALU.AND8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0xA8] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0xA9] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0xAA] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0xAB] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0xAC] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0xAD] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0xAE] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.XOR8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0xAF] = delegate {
                Registers.A = ALU.XOR8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0xB0] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0xB1] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0xB2] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0xB3] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0xB4] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0xB5] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0xB6] = delegate {
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.OR8(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0xB7] = delegate {
                Registers.A = ALU.OR8(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0xB8] = delegate {
                ALU.CMP(Registers.A, Registers.B);
                Registers.PC++;
            };

            Normal[0xB9] = delegate {
                ALU.CMP(Registers.A, Registers.C);
                Registers.PC++;
            };

            Normal[0xBA] = delegate {
                ALU.CMP(Registers.A, Registers.D);
                Registers.PC++;
            };

            Normal[0xBB] = delegate {
                ALU.CMP(Registers.A, Registers.E);
                Registers.PC++;
            };

            Normal[0xBC] = delegate {
                ALU.CMP(Registers.A, Registers.H);
                Registers.PC++;
            };

            Normal[0xBD] = delegate {
                ALU.CMP(Registers.A, Registers.L);
                Registers.PC++;
            };

            Normal[0xBE] = delegate {
                Board.AdvanceSystemTime(); //8
                ALU.CMP(Registers.A, Memory.Read(Registers.GetHL()));
                Registers.PC++;
            };

            Normal[0xBF] = delegate {
                ALU.CMP(Registers.A, Registers.A);
                Registers.PC++;
            };

            Normal[0xC0] = delegate {
                //RET NZ

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                if (Registers.GetFlagZ() == 0) {
                    Board.AdvanceSystemTime(); //12 --------------------------------------
                    byte LSB = Memory.Read(Registers.SP++);//                            |
                    //                                                                   |  POP
                    Board.AdvanceSystemTime(); //16                                      |
                    ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                    //--------------------------------------------------------------------

                    Board.AdvanceSystemTime(); //20
                    JUMP(address);

                    return;
                }

                Registers.PC++;
            };

            Normal[0xC1] = delegate {
                //POP BC
                Board.AdvanceSystemTime(); //8
                Registers.C = Memory.Read(Registers.SP++);

                Board.AdvanceSystemTime(); //12
                Registers.B = Memory.Read(Registers.SP++);

                Registers.PC++;
            };

            Normal[0xC2] = delegate {
                //JP NZ,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagZ() == 0) {
                    Board.AdvanceSystemTime(); //16
                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xC3] = delegate {
                //JP a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                Board.AdvanceSystemTime(); //16
                JUMP(address);
            };

            Normal[0xC4] = delegate {
                //CALL NZ,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagZ() == 0) {
                    //Internal Delay
                    Board.AdvanceSystemTime(); //16
                    
                    //PUSH PC + 1
                    Registers.PC++;

                    Board.AdvanceSystemTime(); //20
                    Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                    
                    Board.AdvanceSystemTime(); //24
                    Memory.Write((byte)Registers.PC, --Registers.SP);

                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xC5] = delegate {
                //PUSH BC

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Board.AdvanceSystemTime(); //12
                Memory.Write(Registers.B, --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write(Registers.C, --Registers.SP);

                Registers.PC++;
            };

            Normal[0xC6] = delegate {
                //ADD A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.ADD8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xC7] = delegate {
                //RST 00H

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x00);
            };

            Normal[0xC8] = delegate {
                //RET Z

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                if (Registers.GetFlagZ() == 1) {
                    //Internal Delay
                    Board.AdvanceSystemTime(); //12

                    Board.AdvanceSystemTime(); //16 --------------------------------------
                    byte LSB = Memory.Read(Registers.SP++);//                            |
                    //                                                                   |  POP
                    Board.AdvanceSystemTime(); //20                                      |
                    ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                    //--------------------------------------------------------------------

                    JUMP(address);

                    return;
                }

                Registers.PC++;
            };

            Normal[0xC9] = delegate {
                //RET

                Board.AdvanceSystemTime(); //8 --------------------------------------
                byte LSB = Memory.Read(Registers.SP++);//                            |
                //                                                                   |  POP
                Board.AdvanceSystemTime(); //12                                      |
                ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                //--------------------------------------------------------------------

                //Internal Delay
                Board.AdvanceSystemTime(); //16
                JUMP(address);
            };

            Normal[0xCA] = delegate {
                //JP NZ,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagZ() == 1) {
                    Board.AdvanceSystemTime(); //16
                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xCB] = delegate {
                //CB XX
                Board.AdvanceSystemTime(); //8
                byte Cbopcode = Memory.Read(++Registers.PC);

                Cb[Cbopcode]();

                Registers.PC++;
            };

            Normal[0xCC] = delegate {
                //CALL Z,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagZ() == 1) {

                    //Internal Delay
                    Board.AdvanceSystemTime(); //16

                    Registers.PC++;

                    //PUSH
                    Board.AdvanceSystemTime(); //20
                    Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                    
                    Board.AdvanceSystemTime(); //24
                    Memory.Write((byte)Registers.PC, --Registers.SP);

                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xCD] = delegate {
                //CALL Z,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                //Internal Delay
                Board.AdvanceSystemTime(); //16

                Registers.PC++;

                //PUSH
                Board.AdvanceSystemTime(); //20
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //24
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(address);

            };

            Normal[0xCE] = delegate {
                //ADD A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.ADC8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xCF] = delegate {
                //RST 08H
                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x08);
            };

            Normal[0xD0] = delegate {
                //RET NC

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                if (Registers.GetFlagC() == 0) {
                    Board.AdvanceSystemTime(); //12 --------------------------------------
                    byte LSB = Memory.Read(Registers.SP++);//                            |
                    //                                                                   |  POP
                    Board.AdvanceSystemTime(); //16                                      |
                    ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                    //--------------------------------------------------------------------

                    Board.AdvanceSystemTime(); //20
                    JUMP(address);

                    return;
                }

                Registers.PC++;
            };

            Normal[0xD1] = delegate {
                //POP DE
                Board.AdvanceSystemTime(); //8
                Registers.E = Memory.Read(Registers.SP++);

                Board.AdvanceSystemTime(); //12
                Registers.D = Memory.Read(Registers.SP++);

                Registers.PC++;
            };

            Normal[0xD2] = delegate {
                //JP NC,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagC() == 0) {
                    Board.AdvanceSystemTime(); //16
                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xD3] = delegate {
                throw new UnknownOpcodeException(0xD3, Registers.PC);
            };

            Normal[0xD4] = delegate {
                //CALL NZ,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagC() == 0) {
                    //Internal Delay
                    Board.AdvanceSystemTime(); //16

                    Registers.PC++;

                    //PUSH
                    Board.AdvanceSystemTime(); //20
                    Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                    
                    Board.AdvanceSystemTime(); //24
                    Memory.Write((byte)Registers.PC, --Registers.SP);

                    JUMP(address);

                    return;
                }

                Registers.PC++;
            };

            Normal[0xD5] = delegate {
                //PUSH DE
                Board.AdvanceSystemTime(); //8

                Board.AdvanceSystemTime(); //12
                Memory.Write(Registers.D, --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write(Registers.E, --Registers.SP);

                Registers.PC++;
            };

            Normal[0xD6] = delegate {
                //SUB A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.SUB8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xD7] = delegate {
                //RST 10H
                //PUSH
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x10);
            };

            Normal[0xD8] = delegate {
                //RET C

                //Internal Delay
                Board.AdvanceSystemTime(); //8

                if (Registers.GetFlagC() == 1) {
                    Board.AdvanceSystemTime(); //12 --------------------------------------
                    byte LSB = Memory.Read(Registers.SP++);//                            |
                    //                                                                   |  POP
                    Board.AdvanceSystemTime(); //16                                      |
                    ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                    //--------------------------------------------------------------------

                    Board.AdvanceSystemTime(); //20
                    JUMP(address);

                    return;
                }

                Registers.PC++;
            };

            Normal[0xD9] = delegate {
                //RETI

                Board.AdvanceSystemTime(); //8 --------------------------------------
                byte LSB = Memory.Read(Registers.SP++);//                            |
                //                                                                   |  POP
                Board.AdvanceSystemTime(); //12                                     |
                ushort address = (ushort)(LSB | Memory.Read(Registers.SP++) << 8);// |
                //--------------------------------------------------------------------

                Board.AdvanceSystemTime(); //16
                JUMP(address);

                GetCPU().InterruptService.SetIME(true);
            };

            Normal[0xDA] = delegate {
                //JP C,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagC() == 1) {
                    Board.AdvanceSystemTime(); //16
                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xDB] = delegate {
                throw new UnknownOpcodeException(0xDB, Registers.PC);
            };

            Normal[0xDC] = delegate {
                //CALL C,a16
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                if (Registers.GetFlagC() == 1) {

                    //Internal Delay
                    Board.AdvanceSystemTime(); //16

                    Registers.PC++;

                    //PUSH
                    Board.AdvanceSystemTime(); //20
                    Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                    
                    Board.AdvanceSystemTime(); //24
                    Memory.Write((byte)Registers.PC, --Registers.SP);

                    JUMP(address);
                    return;
                }

                Registers.PC++;
            };

            Normal[0xDD] = delegate {
                throw new UnknownOpcodeException(0xDE, Registers.PC);
            };

            Normal[0xDE] = delegate {
                //ADD A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.SBC8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xDF] = delegate {
                //RST 18H
                //PUSH
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x18);
            };

            Normal[0xE0] = delegate {
                //LDH (a8),A
                Board.AdvanceSystemTime(); //8
                ushort address = (ushort)(0xFF00 + Memory.Read(++Registers.PC));

                Board.AdvanceSystemTime(); //12
                Memory.Write(Registers.A, address);

                Registers.PC++;
            };

            Normal[0xE1] = delegate {
                //POP HL
                Board.AdvanceSystemTime(); //8
                Registers.L = Memory.Read(Registers.SP++);

                Board.AdvanceSystemTime(); //12
                Registers.H = Memory.Read(Registers.SP++);

                Registers.PC++;
            };

            Normal[0xE2] = delegate {
                //LDH (C),A
                ushort address = (ushort)(0xFF00 + Registers.C);

                Board.AdvanceSystemTime(); //8
                Memory.Write(Registers.A, address);

                Registers.PC++;
            };

            Normal[0xE3] = delegate {
                throw new UnknownOpcodeException(0xE3, Registers.PC);
            };

            Normal[0xE4] = delegate {
                throw new UnknownOpcodeException(0xE4, Registers.PC);
            };

            Normal[0xE5] = delegate {
                //PUSH HL
                Board.AdvanceSystemTime(); //8

                Board.AdvanceSystemTime(); //12
                Memory.Write(Registers.H, --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write(Registers.L, --Registers.SP);

                Registers.PC++;
            };

            Normal[0xE6] = delegate {
                //SUB A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.AND8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xE7] = delegate {
                //RST 20H
                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);
                
                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x20);
            };

            Normal[0xE8] = delegate {
                //
                Board.AdvanceSystemTime(); //8
                sbyte next = (sbyte)Memory.Read(++Registers.PC);

                Registers.SP = ALU.ADD_SP_n(next, Registers.SP);

                //Internal Delay
                Board.AdvanceSystemTime(); //12

                //Internal Delay
                Board.AdvanceSystemTime(); //16

                Registers.PC++;
            };

            Normal[0xE9] = delegate {
                //JUMP HL
                JUMP(Registers.GetHL());
            };

            Normal[0xEA] = delegate {
                //LD (a16),A
                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                Board.AdvanceSystemTime(); //16
                Memory.Write(Registers.A, address);

                Registers.PC++;
            };

            Normal[0xEB] = delegate {
                throw new UnknownOpcodeException(0xEB, Registers.PC);
            };

            Normal[0xEC] = delegate {
                throw new UnknownOpcodeException(0xEC, Registers.PC);
            };

            Normal[0xED] = delegate {
                throw new UnknownOpcodeException(0xED, Registers.PC);
            };

            Normal[0xEE] = delegate {
                //SUB A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.XOR8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xEF] = delegate {
                //RST 18H
                //PUSH
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x28);
            };

            Normal[0xF0] = delegate {
                //
                Board.AdvanceSystemTime(); //8
                byte next = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                Registers.A = Memory.Read((ushort)(0xFF00 + next));

                Registers.PC++;
            };

            Normal[0xF1] = delegate {
                //POP HL
                Board.AdvanceSystemTime(); //8
                Registers.F = (byte)(Memory.Read(Registers.SP++) & 0xF0);

                Board.AdvanceSystemTime(); //12
                Registers.A = Memory.Read(Registers.SP++);

                Registers.PC++;
            };

            Normal[0xF2] = delegate {
                //
                Board.AdvanceSystemTime(); //8
                Registers.A = Memory.Read((ushort)(0xFF00 + Registers.C));

                Registers.PC++;
            };

            Normal[0xF3] = delegate {
                //DI
                GetCPU().InterruptService.SetIME(false);

                Registers.PC++;
            };

            Normal[0xF4] = delegate {
                throw new UnknownOpcodeException(0xF4, Registers.PC);
            };

            Normal[0xF5] = delegate {
                //PUSH AF
                Board.AdvanceSystemTime(); //8

                Board.AdvanceSystemTime(); //12
                Memory.Write(Registers.A, --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write(Registers.F, --Registers.SP);

                Registers.PC++;
            };

            Normal[0xF6] = delegate {
                //SUB A,d8
                Board.AdvanceSystemTime(); //8
                Registers.A = ALU.OR8(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xF7] = delegate {
                //RST 30H
                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x30);
            };

            Normal[0xF8] = delegate {
                //
                Board.AdvanceSystemTime(); //8
                sbyte next = (sbyte)Memory.Read(++Registers.PC);
                
                Registers.SetHL(ALU.ADD_SP_n(next, Registers.SP));

                //internal delay
                Board.AdvanceSystemTime(); //12

                Registers.PC++;
            };

            Normal[0xF9] = delegate {
                //
                Board.AdvanceSystemTime(); //8
                Registers.SP = Registers.GetHL();

                Registers.PC++;
            };

            Normal[0xFA] = delegate {
                //LD A,(a16)

                Board.AdvanceSystemTime(); //8
                byte LSB = Memory.Read(++Registers.PC);

                Board.AdvanceSystemTime(); //12
                ushort address = (ushort)(LSB | Memory.Read(++Registers.PC) << 8);

                Board.AdvanceSystemTime(); //16
                
                Registers.A = Memory.Read(address);

                Registers.PC++;
            };

            Normal[0xFB] = delegate {
                //LD A,(a16)
                GetCPU().InterruptService.ScheduleEnableIME();

                Registers.PC++;
            };

            Normal[0xFC] = delegate {
                throw new UnknownOpcodeException(0xFC, Registers.PC);
            };

            Normal[0xFD] = delegate {
                throw new UnknownOpcodeException(0xFD, Registers.PC);
            };

            Normal[0xFE] = delegate {
                //SUB A,d8
                Board.AdvanceSystemTime(); //8
                ALU.CMP(Registers.A, Memory.Read(++Registers.PC));

                Registers.PC++;
            };

            Normal[0xFF] = delegate {
                //RST 38H
                //Internal Delay
                Board.AdvanceSystemTime(); //8

                Registers.PC++;

                Board.AdvanceSystemTime(); //12
                Memory.Write((byte)(Registers.PC >> 8), --Registers.SP);

                Board.AdvanceSystemTime(); //16
                Memory.Write((byte)Registers.PC, --Registers.SP);

                JUMP(0x38);
            };
        }

        private void CbInstructionsPopulate() {
            //At this point we already fetched the opcode next of CB instruction so we advanced 8 clocks totally.
            //so all opcodes are now 8 clocks SHORTER than the documented clocks

            Cb[0x00] = delegate {
                Registers.B = ALU.RLC(Registers.B);
            };

            Cb[0x01] = delegate {
                Registers.C = ALU.RLC(Registers.C);
            };

            Cb[0x02] = delegate {
                Registers.D = ALU.RLC(Registers.D);
            };

            Cb[0x03] = delegate {
                Registers.E = ALU.RLC(Registers.E);
            };

            Cb[0x04] = delegate {
                Registers.H = ALU.RLC(Registers.H);
            };

            Cb[0x05] = delegate {
                Registers.L = ALU.RLC(Registers.L);
            };

            Cb[0x06] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RLC(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x07] = delegate {
                Registers.A = ALU.RLC(Registers.A);
            };

            Cb[0x08] = delegate {
                Registers.B = ALU.RRC(Registers.B);
            };

            Cb[0x09] = delegate {
                Registers.C = ALU.RRC(Registers.C);
            };

            Cb[0x0A] = delegate {
                Registers.D = ALU.RRC(Registers.D);
            };

            Cb[0x0B] = delegate {
                Registers.E = ALU.RRC(Registers.E);
            };

            Cb[0x0C] = delegate {
                Registers.H = ALU.RRC(Registers.H);
            };

            Cb[0x0D] = delegate {
                Registers.L = ALU.RRC(Registers.L);
            };

            Cb[0x0E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RRC(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x0F] = delegate {
                Registers.A = ALU.RRC(Registers.A);
            };

            Cb[0x10] = delegate {
                Registers.B = ALU.RL(Registers.B);
            };

            Cb[0x11] = delegate {
                Registers.C = ALU.RL(Registers.C);
            };

            Cb[0x12] = delegate {
                Registers.D = ALU.RL(Registers.D);
            };

            Cb[0x13] = delegate {
                Registers.E = ALU.RL(Registers.E);
            };

            Cb[0x14] = delegate {
                Registers.H = ALU.RL(Registers.H);
            };

            Cb[0x15] = delegate {
                Registers.L = ALU.RL(Registers.L);
            };

            Cb[0x16] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RL(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x17] = delegate {
                Registers.A = ALU.RL(Registers.A);
            };

            Cb[0x18] = delegate {
                Registers.B = ALU.RR(Registers.B);
            };

            Cb[0x19] = delegate {
                Registers.C = ALU.RR(Registers.C);
            };

            Cb[0x1A] = delegate {
                Registers.D = ALU.RR(Registers.D);
            };

            Cb[0x1B] = delegate {
                Registers.E = ALU.RR(Registers.E);
            };

            Cb[0x1C] = delegate {
                Registers.H = ALU.RR(Registers.H);
            };

            Cb[0x1D] = delegate {
                Registers.L = ALU.RR(Registers.L);
            };

            Cb[0x1E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RR(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x1F] = delegate {
                Registers.A = ALU.RR(Registers.A);
            };

            Cb[0x20] = delegate {
                Registers.B = ALU.SLA(Registers.B);
            };

            Cb[0x21] = delegate {
                Registers.C = ALU.SLA(Registers.C);
            };

            Cb[0x22] = delegate {
                Registers.D = ALU.SLA(Registers.D);
            };

            Cb[0x23] = delegate {
                Registers.E = ALU.SLA(Registers.E);
            };

            Cb[0x24] = delegate {
                Registers.H = ALU.SLA(Registers.H);
            };

            Cb[0x25] = delegate {
                Registers.L = ALU.SLA(Registers.L);
            };

            Cb[0x26] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SLA(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x27] = delegate {
                Registers.A = ALU.SLA(Registers.A);
            };

            Cb[0x28] = delegate {
                Registers.B = ALU.SRA(Registers.B);
            };

            Cb[0x29] = delegate {
                Registers.C = ALU.SRA(Registers.C);
            };

            Cb[0x2A] = delegate {
                Registers.D = ALU.SRA(Registers.D);
            };

            Cb[0x2B] = delegate {
                Registers.E = ALU.SRA(Registers.E);
            };

            Cb[0x2C] = delegate {
                Registers.H = ALU.SRA(Registers.H);
            };

            Cb[0x2D] = delegate {
                Registers.L = ALU.SRA(Registers.L);
            };

            Cb[0x2E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SRA(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x2F] = delegate {
                Registers.A = ALU.SRA(Registers.A);
            };

            Cb[0x30] = delegate {
                Registers.B = ALU.SWAP(Registers.B);
            };

            Cb[0x31] = delegate {
                Registers.C = ALU.SWAP(Registers.C);
            };

            Cb[0x32] = delegate {
                Registers.D = ALU.SWAP(Registers.D);
            };

            Cb[0x33] = delegate {
                Registers.E = ALU.SWAP(Registers.E);
            };

            Cb[0x34] = delegate {
                Registers.H = ALU.SWAP(Registers.H);
            };

            Cb[0x35] = delegate {
                Registers.L = ALU.SWAP(Registers.L);
            };

            Cb[0x36] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SWAP(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x37] = delegate {
                Registers.A = ALU.SWAP(Registers.A);
            };

            Cb[0x38] = delegate {
                Registers.B = ALU.SRL(Registers.B);
            };

            Cb[0x39] = delegate {
                Registers.C = ALU.SRL(Registers.C);
            };

            Cb[0x3A] = delegate {
                Registers.D = ALU.SRL(Registers.D);
            };

            Cb[0x3B] = delegate {
                Registers.E = ALU.SRL(Registers.E);
            };

            Cb[0x3C] = delegate {
                Registers.H = ALU.SRL(Registers.H);
            };

            Cb[0x3D] = delegate {
                Registers.L = ALU.SRL(Registers.L);
            };

            Cb[0x3E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SRL(Memory.Read(Registers.GetHL()));

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x3F] = delegate {
                Registers.A = ALU.SRL(Registers.A);
            };

            Cb[0x40] = delegate {
                ALU.BIT(0, Registers.B);
            };

            Cb[0x41] = delegate {
                ALU.BIT(0, Registers.C);
            };

            Cb[0x42] = delegate {
                ALU.BIT(0, Registers.D);
            };

            Cb[0x43] = delegate {
                ALU.BIT(0, Registers.E);
            };

            Cb[0x44] = delegate {
                ALU.BIT(0, Registers.H);
            };

            Cb[0x45] = delegate {
                ALU.BIT(0, Registers.L);
            };

            Cb[0x46] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(0, Memory.Read(Registers.GetHL()));
            };

            Cb[0x47] = delegate {
                ALU.BIT(0, Registers.A);
            };

            Cb[0x48] = delegate {
                ALU.BIT(1, Registers.B);
            };

            Cb[0x49] = delegate {
                ALU.BIT(1, Registers.C);
            };

            Cb[0x4A] = delegate {
                ALU.BIT(1, Registers.D);
            };

            Cb[0x4B] = delegate {
                ALU.BIT(1, Registers.E);
            };

            Cb[0x4C] = delegate {
                ALU.BIT(1, Registers.H);
            };

            Cb[0x4D] = delegate {
                ALU.BIT(1, Registers.L);
            };

            Cb[0x4E] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(1, Memory.Read(Registers.GetHL()));
            };

            Cb[0x4F] = delegate {
                ALU.BIT(1, Registers.A);
            };

            Cb[0x50] = delegate {
                ALU.BIT(2, Registers.B);
            };

            Cb[0x51] = delegate {
                ALU.BIT(2, Registers.C);
            };

            Cb[0x52] = delegate {
                ALU.BIT(2, Registers.D);
            };

            Cb[0x53] = delegate {
                ALU.BIT(2, Registers.E);
            };

            Cb[0x54] = delegate {
                ALU.BIT(2, Registers.H);
            };

            Cb[0x55] = delegate {
                ALU.BIT(2, Registers.L);
            };

            Cb[0x56] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(2, Memory.Read(Registers.GetHL()));
            };

            Cb[0x57] = delegate {
                ALU.BIT(2, Registers.A);
            };

            Cb[0x58] = delegate {
                ALU.BIT(3, Registers.B);
            };

            Cb[0x59] = delegate {
                ALU.BIT(3, Registers.C);
            };

            Cb[0x5A] = delegate {
                ALU.BIT(3, Registers.D);
            };

            Cb[0x5B] = delegate {
                ALU.BIT(3, Registers.E);
            };

            Cb[0x5C] = delegate {
                ALU.BIT(3, Registers.H);
            };

            Cb[0x5D] = delegate {
                ALU.BIT(3, Registers.L);
            };

            Cb[0x5E] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(3, Memory.Read(Registers.GetHL()));
            };

            Cb[0x5F] = delegate {
                ALU.BIT(3, Registers.A);
            };

            Cb[0x60] = delegate {
                ALU.BIT(4, Registers.B);
            };

            Cb[0x61] = delegate {
                ALU.BIT(4, Registers.C);
            };

            Cb[0x62] = delegate {
                ALU.BIT(4, Registers.D);
            };

            Cb[0x63] = delegate {
                ALU.BIT(4, Registers.E);
            };

            Cb[0x64] = delegate {
                ALU.BIT(4, Registers.H);
            };

            Cb[0x65] = delegate {
                ALU.BIT(4, Registers.L);
            };

            Cb[0x66] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(4, Memory.Read(Registers.GetHL()));
            };

            Cb[0x67] = delegate {
                ALU.BIT(4, Registers.A);
            };

            Cb[0x68] = delegate {
                ALU.BIT(5, Registers.B);
            };

            Cb[0x69] = delegate {
                ALU.BIT(5, Registers.C);
            };

            Cb[0x6A] = delegate {
                ALU.BIT(5, Registers.D);
            };

            Cb[0x6B] = delegate {
                ALU.BIT(5, Registers.E);
            };

            Cb[0x6C] = delegate {
                ALU.BIT(5, Registers.H);
            };

            Cb[0x6D] = delegate {
                ALU.BIT(5, Registers.L);
            };

            Cb[0x6E] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(5, Memory.Read(Registers.GetHL()));
            };

            Cb[0x6F] = delegate {
                ALU.BIT(5, Registers.A);
            };

            Cb[0x70] = delegate {
                ALU.BIT(6, Registers.B);
            };

            Cb[0x71] = delegate {
                ALU.BIT(6, Registers.C);
            };

            Cb[0x72] = delegate {
                ALU.BIT(6, Registers.D);
            };

            Cb[0x73] = delegate {
                ALU.BIT(6, Registers.E);
            };

            Cb[0x74] = delegate {
                ALU.BIT(6, Registers.H);
            };

            Cb[0x75] = delegate {
                ALU.BIT(6, Registers.L);
            };

            Cb[0x76] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(6, Memory.Read(Registers.GetHL()));
            };

            Cb[0x77] = delegate {
                ALU.BIT(6, Registers.A);
            };

            Cb[0x78] = delegate {
                ALU.BIT(7, Registers.B);
            };

            Cb[0x79] = delegate {
                ALU.BIT(7, Registers.C);
            };

            Cb[0x7A] = delegate {
                ALU.BIT(7, Registers.D);
            };

            Cb[0x7B] = delegate {
                ALU.BIT(7, Registers.E);
            };

            Cb[0x7C] = delegate {
                ALU.BIT(7, Registers.H);
            };

            Cb[0x7D] = delegate {
                ALU.BIT(7, Registers.L);
            };

            Cb[0x7E] = delegate {
                Board.AdvanceSystemTime();
                ALU.BIT(7, Memory.Read(Registers.GetHL()));
            };

            Cb[0x7F] = delegate {
                ALU.BIT(7, Registers.A);
            };

            Cb[0x80] = delegate {
                Registers.B = ALU.RES(Registers.B, 0);
            };

            Cb[0x81] = delegate {
                Registers.C = ALU.RES(Registers.C, 0);
            };

            Cb[0x82] = delegate {
                Registers.D = ALU.RES(Registers.D, 0);
            };

            Cb[0x83] = delegate {
                Registers.E = ALU.RES(Registers.E, 0);
            };

            Cb[0x84] = delegate {
                Registers.H = ALU.RES(Registers.H, 0);
            };

            Cb[0x85] = delegate {
                Registers.L = ALU.RES(Registers.L, 0);
            };

            Cb[0x86] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 0);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x87] = delegate {
                Registers.A = ALU.RES(Registers.A, 0);
            };

            Cb[0x88] = delegate {
                Registers.B = ALU.RES(Registers.B, 1);
            };

            Cb[0x89] = delegate {
                Registers.C = ALU.RES(Registers.C, 1);
            };

            Cb[0x8A] = delegate {
                Registers.D = ALU.RES(Registers.D, 1);
            };

            Cb[0x8B] = delegate {
                Registers.E = ALU.RES(Registers.E, 1);
            };

            Cb[0x8C] = delegate {
                Registers.H = ALU.RES(Registers.H, 1);
            };

            Cb[0x8D] = delegate {
                Registers.L = ALU.RES(Registers.L, 1);
            };

            Cb[0x8E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 1);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x8F] = delegate {
                Registers.A = ALU.RES(Registers.A, 1);
            };

            Cb[0x90] = delegate {
                Registers.B = ALU.RES(Registers.B, 2);
            };

            Cb[0x91] = delegate {
                Registers.C = ALU.RES(Registers.C, 2);
            };

            Cb[0x92] = delegate {
                Registers.D = ALU.RES(Registers.D, 2);
            };

            Cb[0x93] = delegate {
                Registers.E = ALU.RES(Registers.E, 2);
            };

            Cb[0x94] = delegate {
                Registers.H = ALU.RES(Registers.H, 2);
            };

            Cb[0x95] = delegate {
                Registers.L = ALU.RES(Registers.L, 2);
            };

            Cb[0x96] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 2);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x97] = delegate {
                Registers.A = ALU.RES(Registers.A, 2);
            };

            Cb[0x98] = delegate {
                Registers.B = ALU.RES(Registers.B, 3);
            };

            Cb[0x99] = delegate {
                Registers.C = ALU.RES(Registers.C, 3);
            };

            Cb[0x9A] = delegate {
                Registers.D = ALU.RES(Registers.D, 3);
            };

            Cb[0x9B] = delegate {
                Registers.E = ALU.RES(Registers.E, 3);
            };

            Cb[0x9C] = delegate {
                Registers.H = ALU.RES(Registers.H, 3);
            };

            Cb[0x9D] = delegate {
                Registers.L = ALU.RES(Registers.L, 3);
            };

            Cb[0x9E] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 3);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0x9F] = delegate {
                Registers.A = ALU.RES(Registers.A, 3);
            };

            Cb[0xA0] = delegate {
                Registers.B = ALU.RES(Registers.B, 4);
            };

            Cb[0xA1] = delegate {
                Registers.C = ALU.RES(Registers.C, 4);
            };

            Cb[0xA2] = delegate {
                Registers.D = ALU.RES(Registers.D, 4);
            };

            Cb[0xA3] = delegate {
                Registers.E = ALU.RES(Registers.E, 4);
            };

            Cb[0xA4] = delegate {
                Registers.H = ALU.RES(Registers.H, 4);
            };

            Cb[0xA5] = delegate {
                Registers.L = ALU.RES(Registers.L, 4);
            };

            Cb[0xA6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 4);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xA7] = delegate {
                Registers.A = ALU.RES(Registers.A, 4);
            };

            Cb[0xA8] = delegate {
                Registers.B = ALU.RES(Registers.B, 5);
            };

            Cb[0xA9] = delegate {
                Registers.C = ALU.RES(Registers.C, 5);
            };

            Cb[0xAA] = delegate {
                Registers.D = ALU.RES(Registers.D, 5);
            };

            Cb[0xAB] = delegate {
                Registers.E = ALU.RES(Registers.E, 5);
            };

            Cb[0xAC] = delegate {
                Registers.H = ALU.RES(Registers.H, 5);
            };

            Cb[0xAD] = delegate {
                Registers.L = ALU.RES(Registers.L, 5);
            };

            Cb[0xAE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 5);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xAF] = delegate {
                Registers.A = ALU.RES(Registers.A, 5);
            };

            Cb[0xB0] = delegate {
                Registers.B = ALU.RES(Registers.B, 6);
            };

            Cb[0xB1] = delegate {
                Registers.C = ALU.RES(Registers.C, 6);
            };

            Cb[0xB2] = delegate {
                Registers.D = ALU.RES(Registers.D, 6);
            };

            Cb[0xB3] = delegate {
                Registers.E = ALU.RES(Registers.E, 6);
            };

            Cb[0xB4] = delegate {
                Registers.H = ALU.RES(Registers.H, 6);
            };

            Cb[0xB5] = delegate {
                Registers.L = ALU.RES(Registers.L, 6);
            };

            Cb[0xB6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 6);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xB7] = delegate {
                Registers.A = ALU.RES(Registers.A, 6);
            };

            Cb[0xB8] = delegate {
                Registers.B = ALU.RES(Registers.B, 7);
            };

            Cb[0xB9] = delegate {
                Registers.C = ALU.RES(Registers.C, 7);
            };

            Cb[0xBA] = delegate {
                Registers.D = ALU.RES(Registers.D, 7);
            };

            Cb[0xBB] = delegate {
                Registers.E = ALU.RES(Registers.E, 7);
            };

            Cb[0xBC] = delegate {
                Registers.H = ALU.RES(Registers.H, 7);
            };

            Cb[0xBD] = delegate {
                Registers.L = ALU.RES(Registers.L, 7);
            };

            Cb[0xBE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.RES(Memory.Read(Registers.GetHL()), 7);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xBF] = delegate {
                Registers.A = ALU.RES(Registers.A, 7);
            };

            Cb[0xC0] = delegate {
                Registers.B = ALU.SET(Registers.B, 0);
            };

            Cb[0xC1] = delegate {
                Registers.C = ALU.SET(Registers.C, 0);
            };

            Cb[0xC2] = delegate {
                Registers.D = ALU.SET(Registers.D, 0);
            };

            Cb[0xC3] = delegate {
                Registers.E = ALU.SET(Registers.E, 0);
            };

            Cb[0xC4] = delegate {
                Registers.H = ALU.SET(Registers.H, 0);
            };

            Cb[0xC5] = delegate {
                Registers.L = ALU.SET(Registers.L, 0);
            };

            Cb[0xC6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 0);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xC7] = delegate {
                Registers.A = ALU.SET(Registers.A, 0);
            };

            Cb[0xC8] = delegate {
                Registers.B = ALU.SET(Registers.B, 1);
            };

            Cb[0xC9] = delegate {
                Registers.C = ALU.SET(Registers.C, 1);
            };

            Cb[0xCA] = delegate {
                Registers.D = ALU.SET(Registers.D, 1);
            };

            Cb[0xCB] = delegate {
                Registers.E = ALU.SET(Registers.E, 1);
            };

            Cb[0xCC] = delegate {
                Registers.H = ALU.SET(Registers.H, 1);
            };

            Cb[0xCD] = delegate {
                Registers.L = ALU.SET(Registers.L, 1);
            };

            Cb[0xCE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 1);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xCF] = delegate {
                Registers.A = ALU.SET(Registers.A, 1);
            };

            Cb[0xD0] = delegate {
                Registers.B = ALU.SET(Registers.B, 2);
            };

            Cb[0xD1] = delegate {
                Registers.C = ALU.SET(Registers.C, 2);
            };

            Cb[0xD2] = delegate {
                Registers.D = ALU.SET(Registers.D, 2);
            };

            Cb[0xD3] = delegate {
                Registers.E = ALU.SET(Registers.E, 2);
            };

            Cb[0xD4] = delegate {
                Registers.H = ALU.SET(Registers.H, 2);
            };

            Cb[0xD5] = delegate {
                Registers.L = ALU.SET(Registers.L, 2);
            };

            Cb[0xD6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 2);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xD7] = delegate {
                Registers.A = ALU.SET(Registers.A, 2);
            };

            Cb[0xD8] = delegate {
                Registers.B = ALU.SET(Registers.B, 3);
            };

            Cb[0xD9] = delegate {
                Registers.C = ALU.SET(Registers.C, 3);
            };

            Cb[0xDA] = delegate {
                Registers.D = ALU.SET(Registers.D, 3);
            };

            Cb[0xDB] = delegate {
                Registers.E = ALU.SET(Registers.E, 3);
            };

            Cb[0xDC] = delegate {
                Registers.H = ALU.SET(Registers.H, 3);
            };

            Cb[0xDD] = delegate {
                Registers.L = ALU.SET(Registers.L, 3);
            };

            Cb[0xDE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 3);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xDF] = delegate {
                Registers.A = ALU.SET(Registers.A, 3);
            };

            Cb[0xE0] = delegate {
                Registers.B = ALU.SET(Registers.B, 4);
            };

            Cb[0xE1] = delegate {
                Registers.C = ALU.SET(Registers.C, 4);
            };

            Cb[0xE2] = delegate {
                Registers.D = ALU.SET(Registers.D, 4);
            };

            Cb[0xE3] = delegate {
                Registers.E = ALU.SET(Registers.E, 4);
            };

            Cb[0xE4] = delegate {
                Registers.H = ALU.SET(Registers.H, 4);
            };

            Cb[0xE5] = delegate {
                Registers.L = ALU.SET(Registers.L, 4);
            };

            Cb[0xE6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 4);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xE7] = delegate {
                Registers.A = ALU.SET(Registers.A, 4);
            };

            Cb[0xE8] = delegate {
                Registers.B = ALU.SET(Registers.B, 5);
            };

            Cb[0xE9] = delegate {
                Registers.C = ALU.SET(Registers.C, 5);
            };

            Cb[0xEA] = delegate {
                Registers.D = ALU.SET(Registers.D, 5);
            };

            Cb[0xEB] = delegate {
                Registers.E = ALU.SET(Registers.E, 5);
            };

            Cb[0xEC] = delegate {
                Registers.H = ALU.SET(Registers.H, 5);
            };

            Cb[0xED] = delegate {
                Registers.L = ALU.SET(Registers.L, 5);
            };

            Cb[0xEE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 5);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xEF] = delegate {
                Registers.A = ALU.SET(Registers.A, 5);
            };

            Cb[0xF0] = delegate {
                Registers.B = ALU.SET(Registers.B, 6);
            };

            Cb[0xF1] = delegate {
                Registers.C = ALU.SET(Registers.C, 6);
            };

            Cb[0xF2] = delegate {
                Registers.D = ALU.SET(Registers.D, 6);
            };

            Cb[0xF3] = delegate {
                Registers.E = ALU.SET(Registers.E, 6);
            };

            Cb[0xF4] = delegate {
                Registers.H = ALU.SET(Registers.H, 6);
            };

            Cb[0xF5] = delegate {
                Registers.L = ALU.SET(Registers.L, 6);
            };

            Cb[0xF6] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 6);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xF7] = delegate {
                Registers.A = ALU.SET(Registers.A, 6);
            };

            Cb[0xF8] = delegate {
                Registers.B = ALU.SET(Registers.B, 7);
            };

            Cb[0xF9] = delegate {
                Registers.C = ALU.SET(Registers.C, 7);
            };

            Cb[0xFA] = delegate {
                Registers.D = ALU.SET(Registers.D, 7);
            };

            Cb[0xFB] = delegate {
                Registers.E = ALU.SET(Registers.E, 7);
            };

            Cb[0xFC] = delegate {
                Registers.H = ALU.SET(Registers.H, 7);
            };

            Cb[0xFD] = delegate {
                Registers.L = ALU.SET(Registers.L, 7);
            };

            Cb[0xFE] = delegate {
                Board.AdvanceSystemTime();
                byte n = ALU.SET(Memory.Read(Registers.GetHL()), 7);

                Board.AdvanceSystemTime(); //16
                Memory.Write(n, Registers.GetHL());
            };

            Cb[0xFF] = delegate {
                Registers.A = ALU.SET(Registers.A, 7);
            };

        }

        private void JUMP(ushort address) {
            Registers.PC = address;
        }

    }
}
