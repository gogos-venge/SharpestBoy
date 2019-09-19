using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpestBoy.DMG.CPU.Units {
    public class ArithmeticLogicUnit : Unit {

        private Registers Registers;

        public ArithmeticLogicUnit(DMGCPU dmgcpu) : base(dmgcpu){
            Registers = dmgcpu.Registers;
        }

        public byte SLA(byte reg) {
            Registers.SetFlagC((reg & 0x80) == 0x80);
            reg <<= 1;
            Registers.SetFlagZ(reg == 0);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            return reg;
        }

        public byte RLC(byte a) {
            Registers.F = 0;
            Registers.SetFlagC((a & 0x80) == 0x80);
            a = (byte)((a << 1) | Registers.GetFlagC());
            Registers.SetFlagZ(a == 0);
            return a;
        }

        public byte RLCA(byte a) {
            Registers.F = 0;
            Registers.SetFlagC((a & 0x80) == 0x80);
            a = (byte)((a << 1) | Registers.GetFlagC());
            return a;
        }

        public byte RRCA(byte a) {
            Registers.F = 0;
            Registers.SetFlagC((a & 0x01) == 0x01);
            a = (byte)((a >> 1) | Registers.GetFlagC() << 7);
            return a;
        }

        public byte RL(byte a) {
            byte oldbit7 = Registers.GetFlagC();
            Registers.SetFlagC((a & 0x80) == 0x80);
            a <<= 1;
            a |= (byte)(oldbit7);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            Registers.SetFlagZ(a == 0);
            return a;
        }

        public byte RLA(byte a) {
            byte oldbit7 = Registers.GetFlagC();
            Registers.F = 0;
            Registers.SetFlagC((a & 0x80) == 0x80);
            a = (byte)(a << 1 | oldbit7);
            return a;
        }

        public byte RR(byte a) {
            byte oldbit0 = Registers.GetFlagC();
            Registers.SetFlagC((a & 0x01) == 0x01);
            a >>= 1;
            a |= (byte)(oldbit0 << 7);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            Registers.SetFlagZ(a == 0);
            return a;
        }

        public byte RRA(byte a) {
            byte oldbit0 = Registers.GetFlagC();
            Registers.F = 0;
            Registers.SetFlagC((a & 0x01) == 0x01);
            a = (byte)(a >> 1 | (oldbit0 << 7));
            return a;
        }

        public byte RRC(byte a) {
            Registers.SetFlagC((a & 0x01) == 0x01);
            a = (byte)((a >> 1) | Registers.GetFlagC() << 7);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            Registers.SetFlagZ(a == 0);
            return a;
        }

        public byte SRA(byte reg) {
            byte temp = (byte)(reg & 0x80);
            Registers.SetFlagC((reg & 0x1) == 1);
            reg = (byte)((reg >> 1) | temp);
            Registers.SetFlagZ(reg == 0);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            return reg;
        }

        public byte SRL(byte reg) {
            Registers.SetFlagC((reg & 0x1) == 1);
            reg = (byte)(reg >> 1);
            Registers.SetFlagZ(reg == 0);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
            return reg;
        }

        public void BIT(byte pos, byte reg) {
            Registers.SetFlagZ((reg & (1 << pos)) == 0);
            Registers.SetFlagN(false);
            Registers.SetFlagH(true);

        }

        public void DAA() {
            //complete bite from crystal boy
            //https://code.google.com/p/crystalboy/source/browse/trunk/CrystalBoy.Emulation/Processor.Generated.cs

            if (Registers.GetFlagN() != 0) {
                if (Registers.GetFlagH() != 0) Registers.A -= 0x06;
                if (Registers.GetFlagC() != 0) Registers.A -= 0x60;
            }
            else {
                if (Registers.GetFlagC() != 0 || Registers.A > 0x99) {
                    Registers.A += Registers.GetFlagH() != 0 || (Registers.A & 0x0F) > 0x09 ? (byte)0x66 : (byte)0x60;
                    Registers.SetFlagC(true);
                }
                else if (Registers.GetFlagH() != 0 || (Registers.A & 0x0F) > 0x09) Registers.A += 0x06;
            }
            Registers.SetFlagZ(Registers.A == 0);
            Registers.SetFlagH(false);

        }

        public byte SWAP(byte b) {
            byte temp = (byte)((b & 0x0F) << 4 | (b & 0xF0) >> 4);
            Registers.F = 0;
            Registers.SetFlagZ(temp == 0);
            return temp;

        }

        public byte RES(byte value, byte pos) {
            return (byte)(value & ~(byte)(1 << pos));
        }

        public byte SET(byte value, byte pos) {
            return (byte)(value | (byte)(1 << pos));
        }
        
        public ushort ADD16(ushort n1, ushort n2) {
            int temp = n1 + n2;
            Registers.SetFlagN(false);
            Registers.SetFlagH(((n1 & 0xFFF) + (n2 & 0xFFF)) > 0xFFF);
            Registers.SetFlagC(temp > 0xFFFF);
            return (ushort)temp;

        }

        public byte ADD8(byte n1, byte n2) {
            Registers.F = 0; //reset flag
            byte temp = (byte)(n1 + n2);
            Registers.SetFlagZ(temp == 0);
            Registers.SetFlagH((((n1 & 0x0F) + (n2 & 0x0F)) & 0x10) == 0x10);
            Registers.SetFlagC(n1 + n2 > 0xFF);
            return (byte)temp;

        }

        public ushort ADD8_to_16(byte n2, ushort n1) {
            Registers.F = 0; //reset flag
            ushort temp = (ushort)(n1 + n2);
            Registers.SetFlagZ(temp == 0);
            Registers.SetFlagH((((n1 & 0x0F) + (n2 & 0x0F)) & 0x10) == 0x10);
            Registers.SetFlagC((n1 & 0xFF) + (n2 & 0xFF) > 0xFF);
            return temp;

        }

        public byte INC(Byte register) {
            Registers.SetFlagN(false);
            Registers.SetFlagH((register & 0x0F) == 0x0F);
            register++;
            Registers.SetFlagZ(register == 0);
            return register;
        }

        public byte DEC(Byte register) {
            Registers.SetFlagN(true);
            Registers.SetFlagH((register & 0x0F) == 0x00);
            register--;
            Registers.SetFlagZ(register == 0);
            return register;

        }

        public ushort ADD_SP_n(sbyte n2, ushort SP) {
            Registers.F = 0; //reset flag
            ushort temp = (ushort)(SP + n2);
            Registers.SetFlagH((SP & 0x0F) + (n2 & 0x0F) > 0x0F);
            Registers.SetFlagC((SP & 0xFF) + (n2 & 0xFF) > 0xFF);
            return temp;

        }

        public byte ADC8(byte n1, byte n2) {
            byte c = Registers.GetFlagC();
            byte temp = (byte)(c + n1 + n2);

            Registers.F = 0; //reset flag
            Registers.SetFlagZ(temp == 0);
            Registers.SetFlagH(((n1 & 0x0F) + (n2 & 0x0F) + c) > 0x0F);
            Registers.SetFlagC(n1 + n2 + c > 0xFF);
            return (byte)temp;

        }

        public byte SUB8(byte n1, byte n2) {
            Registers.F = 0; //reset flag
            byte temp = (byte)(n1 - n2);
            Registers.SetFlagZ(temp == 0);
            Registers.SetFlagN(true);
            Registers.SetFlagH((n1 & 0x0F) < (n2 & 0x0F));
            Registers.SetFlagC(n1 < n2);
            return temp;

        }

        public byte SBC8(byte n1, byte n2) {
            Registers.SetFlagN(true);
            if (Registers.GetFlagC() == 1) {
                Registers.SetFlagH(((n1 & 0x0F) - (n2 & 0x0F)) < 1);
                Registers.SetFlagC((n1 - n2) < 1);
                Registers.SetFlagZ((n1 = (byte)(n1 - n2 - 1)) == 0);
                return n1;
            }
            else {
                Registers.SetFlagH((n1 & 0x0F) < (n2 & 0x0F));
                Registers.SetFlagC(n1 < n2);
                Registers.SetFlagZ((n1 -= n2) == 0);
                return n1;
            }
        }

        public void CMP(byte n1, byte n2) {
            Registers.SetFlagZ(n1 == n2);
            Registers.SetFlagN(true);
            Registers.SetFlagH((n1 & 0x0F) < (n2 & 0x0F));
            Registers.SetFlagC(n1 < n2);

        }

        public ushort INC_16(ushort reg_pair) {
            return ++reg_pair;
        }

        public ushort DEC_16(ushort reg_pair) {
            return --reg_pair;
        }

        //BITWISE LOGICAL OPERATIONS ==========================================

        public byte AND8(byte n1, byte n2) {
            byte temp = (byte)(n1 & n2);
            Registers.F = 0;
            Registers.SetFlagZ(temp == 0);
            Registers.SetFlagH(true);
            return temp;
        }

        public byte OR8(byte n1, byte n2) {
            byte temp = (byte)(n1 | n2);
            Registers.F = 0;
            Registers.SetFlagZ(temp == 0);
            return temp;
        }

        public byte XOR8(byte n1, byte n2) {
            byte temp = (byte)(n1 ^ n2);
            Registers.F = 0;
            Registers.SetFlagZ(temp == 0);
            return temp;
        }

        public void CPL() {
            Registers.SetFlagN(true);
            Registers.SetFlagH(true);
            Registers.A = (byte)~Registers.A;
        }

        // MISC OPERATIONS =========================================

        public byte LDHL_SP_n(sbyte n, ushort SP) {
            Registers.F = 0; //reset flag
            ushort temp = (ushort)(SP + n);
            Registers.SetFlagH((((SP & 0x0F) + (n & 0x0F)) & 0x10) == 0x10);
            Registers.SetFlagC((((SP & 0xFF) + n) & 0x100) == 0x100);
            return (byte)temp;

        }

        //Set carry flag
        public void SCF() {
            Registers.SetFlagC(true);
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
        }

        //Clear carry flag
        public void CCF() {
            Registers.SetFlagC(!(Registers.GetFlagC() == 1));
            Registers.SetFlagH(false);
            Registers.SetFlagN(false);
        }

    }
}
