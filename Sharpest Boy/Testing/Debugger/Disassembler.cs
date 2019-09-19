﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpestBoy.DMG.CPU;
using SharpestBoy.Components;
using System.Text.RegularExpressions;

namespace SharpestBoy.Testing.Debugger {
    class Disassembler {

        DMGCPU cpu;
        MemoryManagementUnit Memory;
        ushort disPC = 0;
        Regex parenthesi = new Regex(@"\((.+)\)");

        public Disassembler(DMGCPU dmg) {
            cpu = dmg;
            Memory = dmg.GetBoard().GetMemoryManagementUnit();
        }

        public List<Assembly> Disassemble() {
            List<Assembly> dis = new List<Assembly>();
            StringBuilder sb = new StringBuilder();
            char[] s = { ' ' };
            char[] c = { ',' };
            for (disPC = 0; disPC < 0x8000; disPC++) {
                if (disPC > 0x101 && disPC <= 0x14F) continue;
                byte opcode = Memory.Read(disPC);
                string mnemonic = opcodeNames[opcode];
                Assembly a = new Assembly {
                    Address = disPC,
                    Length = 0,
                };
                a.Opcodes[a.Length++] = opcode;
                
                string[] splitMnemonic = mnemonic.Split(s);
                //split space LD <----> A,(a16)
                //check comma in the second part A <------> (a16)
                sb.Append(splitMnemonic[0]);
                a.Instruction = splitMnemonic[0];
                if (mnemonic.Equals("CB")) {
                    a.Opcodes[a.Length++] = Memory.Read(++disPC);
                    a.Operand1 = new Assembly.Operand();
                    a.Operand1.Expression = cbOpcodeNames[a.Opcodes[a.Length - 1]];
                    a.Operand1.Action = Assembly.Type.Ignore;
                }else if (splitMnemonic.Length > 1) {
                    string[] splitOperand = splitMnemonic[1].Split(c);
                    a.Operand1 = new Assembly.Operand();
                    Parse(splitOperand[0], a, a.Operand1, false, sb);
                    if(a.Operand1.Action == Assembly.Type.Value) {
                        if (a.Instruction.Equals("JR")) {
                            a.Operand1.Value += a.Address;
                        }

                        if (a.Instruction.Equals("LDH")) {
                            a.Operand1.Value += 0xFF00;
                        }
                    }
                    
                    if(splitOperand.Length > 1) {
                        a.Operand2 = new Assembly.Operand();
                        Parse(splitOperand[1], a, a.Operand2, true, sb);

                        if (a.Operand2.Action == Assembly.Type.Value || a.Operand2.Action == Assembly.Type.Evaluation) {
                            if (a.Instruction.Equals("JR")) {
                                a.Operand2.Value += disPC;
                            }

                            if (a.Instruction.Equals("LDH")) {
                                a.Operand2.Value += 0xFF00;
                            }
                        }
                    }
                }
                a.Format = sb.ToString();
                sb.Length = 0;
                dis.Add(a);
            }
            return dis;
        }

        
        /// <summary>
        /// Parses an expression.
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        public void Parse(string expression, Assembly a, Assembly.Operand op, bool evaluate, StringBuilder Format) {
            if (!evaluate) {
                for(int i = 0; i < 5 - a.Instruction.Length; i++) {
                    Format.Append(" ");
                }
                if (parenthesi.IsMatch(expression)) {
                    Format.Append("({0})");
                    Match m = parenthesi.Match(expression);
                    op.Expression = m.Groups[1].Value;
                    
                } else {
                    Format.Append("{0}");
                    op.Expression = expression;
                }
                
                LookupImmediate(a, op);
            }
            else {
                if (parenthesi.IsMatch(expression)) {
                    Format.Append(",{1}");
                    Match m = parenthesi.Match(expression);
                    op.Expression = m.Groups[1].Value;
                    LookupImmediate(a, op);
                    op.Action = Assembly.Type.Evaluation;
                }
                else {
                    Format.Append(",{1}");
                    op.Expression = expression;
                    LookupImmediate(a, op);
                }
                
            }
            
        }

        private void LookupImmediate(Assembly a, Assembly.Operand op) {
            switch (op.Expression) {
                case "r8":
                case "SP+r8":
                    op.Value = (sbyte)Memory.Read(++disPC) + 1;
                    a.Opcodes[a.Length++] = (byte)op.Value;
                    op.Action = Assembly.Type.Value;
                    break;
                case "a8":
                case "d8":
                    op.Value = Memory.Read(++disPC);
                    a.Opcodes[a.Length++] = (byte)op.Value;
                    op.Action = Assembly.Type.Value;
                    break;
                case "a16":
                case "d16":
                    a.Opcodes[a.Length++] = Memory.Read(++disPC);
                    a.Opcodes[a.Length++] = Memory.Read(++disPC);
                    op.Value = a.Opcodes[a.Length - 2] | a.Opcodes[a.Length - 1] << 8;
                    op.Action = Assembly.Type.Value;
                    break;
                default:
                    if(a.Operand2 == op) {
                        op.Action = Assembly.Type.Register;
                        break;
                    }
                    op.Action = Assembly.Type.Ignore;
                    break;
            }
        }

        public int LookupOperand(Assembly.Operand op) {
            switch (op.Expression) {
                case "A":
                    return cpu.Registers.A;
                case "H":
                    return cpu.Registers.H;
                case "L":
                    return cpu.Registers.L;
                case "B":
                    return cpu.Registers.B;
                case "C":
                    return cpu.Registers.C;
                case "D":
                    return cpu.Registers.D;
                case "E":
                    return cpu.Registers.E;
                case "F":
                    return cpu.Registers.F;
                case "HL":
                    return cpu.Registers.GetHL();
                case "AF":
                    return cpu.Registers.GetAF();
                case "BC":
                    return cpu.Registers.GetBC();
                case "DE":
                    return cpu.Registers.GetDE();
                case "SP":
                    return cpu.Registers.SP;
                case "HL+":
                    return cpu.Registers.GetHL() + 1;
                case "HL-":
                    return cpu.Registers.GetHL() - 1;

            }
            return 0;
        }

        public class Assembly {
            //String format like: LD {0},{1}
            public String Format { get; set; }
            public String Instruction { get; set; }
            public ushort Address { get; set; }
            public byte[] Opcodes { get; set; }
            public Operand Operand1 { get; set; }
            public Operand Operand2 { get; set; }
            public int Length { get; set; }
            public Assembly() {
                Opcodes = new byte[3];
            }
            public enum Type {
                Ignore,
                Value,
                Evaluation,
                StackPointer, 
                Register
            }

            public class Operand {
                public int Value { get; set; }
                public string Expression { get; set; }
                public Type Action { get; set; }
                
            }
        }

        

        public static string[] opcodeNames = new string[]
        {"NOP",
        "LD BC,d16",
        "LD (BC),A",
        "INC BC",
        "INC B",
        "DEC B",
        "LD B,d8",
        "RLCA",
        "LD (a16),SP",
        "ADD HL,BC",
        "LD A,(BC)",
        "DEC BC",
        "INC C",
        "DEC C",
        "LD C,d8",
        "RRCA",

        "STOP",
        "LD DE,d16",
        "LD (DE),A",
        "INC DE",
        "INC D",
        "DEC D",
        "LD D,d8",
        "RLA",
        "JR r8",
        "ADD HL,DE",
        "LD A,(DE)",
        "DEC DE",
        "INC E",
        "DEC E",
        "LD E,d8",
        "RRA",

        "JR NZ,r8",
        "LD HL,d16",
        "LD (HL+)",
        "INC HL",
        "INC H",
        "DEC H",
        "LD H,d8",
        "DAA",
        "JR Z,r8",
        "ADD HL,HL",
        "LD A,(HL+)",
        "DEC HL",
        "INC L",
        "DEC L",
        "LD L,d8",
        "CPL",

        "JR NC,r8",
        "LD SP,d16",
        "LD (HL-),A",
        "INC SP",
        "INC (HL)",
        "DEC (HL)",
        "LD (HL),d8",
        "SCF",
        "JR C,r8",
        "ADD HL,SP",
        "LD A,(HL-)",
        "DEC SP",
        "INC A",
        "DEC A",
        "LD A,d8",
        "CCF",

        "LD B,B",
        "LD B,C",
        "LD B,D",
        "LD B,E",
        "LD B,H",
        "LD B,L",
        "LD B,(HL)",
        "LD B,A",
        "LD C,B",
        "LD C,C",
        "LD C,D",
        "LD C,E",
        "LD C,H",
        "LD C,L",
        "LD C,(HL)",
        "LD C,A",

        "LD D,B",
        "LD D,C",
        "LD D,D",
        "LD D,E",
        "LD D,H",
        "LD D,L",
        "LD D,(HL)",
        "LD D,A",
        "LD E,B",
        "LD E,C",
        "LD E,D",
        "LD E,E",
        "LD E,H",
        "LD E,L",
        "LD E,(HL)",
        "LD E,A",

        "LD H,B",
        "LD H,C",
        "LD H,D",
        "LD H,E",
        "LD H,H",
        "LD H,L",
        "LD H,(HL)",
        "LD H,A",
        "LD L,B",
        "LD L,C",
        "LD L,D",
        "LD L,E",
        "LD L,H",
        "LD L,L",
        "LD L,(HL)",
        "LD L,A",

        "LD (HL),B",
        "LD (HL),C",
        "LD (HL),D",
        "LD (HL),E",
        "LD (HL),H",
        "LD (HL),L",
        "HALT",
        "LD (HL),A",
        "LD A,B",
        "LD A,C",
        "LD A,D",
        "LD A,E",
        "LD A,H",
        "LD A,L",
        "LD A,(HL)",
        "LD A,A",

        "ADD A,B",
        "ADD A,C",
        "ADD A,D",
        "ADD A,E",
        "ADD A,H",
        "ADD A,L",
        "ADD A,(HL)",
        "ADD A,A",
        "ADC A,B",
        "ADC A,C",
        "ADC A,D",
        "ADC A,E",
        "ADC A,H",
        "ADC A,L",
        "ADC A,(HL)",
        "ADC A,A",

        "SUB B",
        "SUB C",
        "SUB D",
        "SUB E",
        "SUB H",
        "SUB L",
        "SUB (HL)",
        "SUB A",
        "SBC A,B",
        "SBC A,C",
        "SBC A,D",
        "SBC A,E",
        "SBC A,H",
        "SBC A,L",
        "SBC A,(HL)",
        "SBC A,A",

        "AND B",
        "AND C",
        "AND D",
        "AND E",
        "AND H",
        "AND L",
        "AND (HL)",
        "AND A",
        "XOR B",
        "XOR C",
        "XOR D",
        "XOR E",
        "XOR H",
        "XOR L",
        "XOR (HL)",
        "XOR A",

        "OR B",
        "OR C",
        "OR D",
        "OR E",
        "OR H",
        "OR L",
        "OR (HL)",
        "OR A",
        "CP B",
        "CP C",
        "CP D",
        "CP E",
        "CP H",
        "CP L",
        "CP (HL)",
        "CP A",

        "RET NZ",
        "POP BC",
        "JP NZ,a16",
        "JP a16",
        "CALL NZ,a16",
        "PUSH BC",
        "ADD A,d8",
        "RST 00H",
        "RET Z",
        "RET",
        "JP Z,a16",
        "CB",
        "CALL Z,a16",
        "CALL a16",
        "ADC A,d8",
        "RST 08H",

        "RET NC",
        "POP DE",
        "JP NC,a16",
        "-",
        "CALL NC,a16",
        "PUSH DE",
        "SUB d8",
        "RST 10H",
        "RET C",
        "RETI",
        "JP C,a16",
        "-",
        "CALL C,a16",
        "-",
        "SBC A,d8",
        "RST 18H",

        "LDH (a8),A",
        "POP HL",
        "LD (C),A",
        "-",
        "-",
        "PUSH HL",
        "AND d8",
        "RST 20H",
        "ADD SP,r8",
        "JP (HL)",
        "LD (a16),A",
        "-",
        "-",
        "-",
        "XOR d8",
        "RST 28H",

        "LDH A,(a8)",
        "POP AF",
        "LD A,(C)",
        "DI",
        "-",
        "PUSH AF",
        "OR d8",
        "RST 30H",
        "LD HL,SP+r8",
        "LD SP,HL",
        "LD A,(a16)",
        "EI",
        "-",
        "-",
        "CP d8",
        "RST 38H"};

        public static string[] cbOpcodeNames = new string[] {
            "RLC B",
            "RLC C",
            "RLC D",
            "RLC E",
            "RLC H",
            "RLC L",
            "RLC (HL)",
            "RLC A",
            "RRC B",
            "RRC C",
            "RRC D",
            "RRC E",
            "RRC H",
            "RRC L",
            "RRC (HL)",
            "RRC A",
            "RL B",
            "RL C",
            "RL D",
            "RL E",
            "RL H",
            "RL L",
            "RL (HL)",
            "RL A",
            "RR B",
            "RR C",
            "RR D",
            "RR E",
            "RR H",
            "RR L",
            "RR (HL)",
            "RR A",
            "SLA B",
            "SLA C",
            "SLA D",
            "SLA E",
            "SLA H",
            "SLA L",
            "SLA (HL)",
            "SLA A",
            "SRA B",
            "SRA C",
            "SRA D",
            "SRA E",
            "SRA H",
            "SRA L",
            "SRA (HL)",
            "SRA A",
            "SWAP B",
            "SWAP C",
            "SWAP D",
            "SWAP E",
            "SWAP H",
            "SWAP L",
            "SWAP (HL)",
            "SWAP A",
            "SRL B",
            "SRL C",
            "SRL D",
            "SRL E",
            "SRL H",
            "SRL L",
            "SRL (HL)",
            "SRL A",
            "BIT 0,B",
            "BIT 0,C",
            "BIT 0,D",
            "BIT 0,E",
            "BIT 0,H",
            "BIT 0,L",
            "BIT 0,(HL)",
            "BIT 0,A",
            "BIT 1,B",
            "BIT 1,C",
            "BIT 1,D",
            "BIT 1,E",
            "BIT 1,H",
            "BIT 1,L",
            "BIT 1,(HL)",
            "BIT 1,A",
            "BIT 2,B",
            "BIT 2,C",
            "BIT 2,D",
            "BIT 2,E",
            "BIT 2,H",
            "BIT 2,L",
            "BIT 2,(HL)",
            "BIT 2,A",
            "BIT 3,B",
            "BIT 3,C",
            "BIT 3,D",
            "BIT 3,E",
            "BIT 3,H",
            "BIT 3,L",
            "BIT 3,(HL)",
            "BIT 3,A",
            "BIT 4,B",
            "BIT 4,C",
            "BIT 4,D",
            "BIT 4,E",
            "BIT 4,H",
            "BIT 4,L",
            "BIT 4,(HL)",
            "BIT 4,A",
            "BIT 5,B",
            "BIT 5,C",
            "BIT 5,D",
            "BIT 5,E",
            "BIT 5,H",
            "BIT 5,L",
            "BIT 5,(HL)",
            "BIT 5,A",
            "BIT 6,B",
            "BIT 6,C",
            "BIT 6,D",
            "BIT 6,E",
            "BIT 6,H",
            "BIT 6,L",
            "BIT 6,(HL)",
            "BIT 6,A",
            "BIT 7,B",
            "BIT 7,C",
            "BIT 7,D",
            "BIT 7,E",
            "BIT 7,H",
            "BIT 7,L",
            "BIT 7,(HL)",
            "BIT 7,A",
            "RES 0,B",
            "RES 0,C",
            "RES 0,D",
            "RES 0,E",
            "RES 0,H",
            "RES 0,L",
            "RES 0,(HL)",
            "RES 0,A",
            "RES 1,B",
            "RES 1,C",
            "RES 1,D",
            "RES 1,E",
            "RES 1,H",
            "RES 1,L",
            "RES 1,(HL)",
            "RES 1,A",
            "RES 2,B",
            "RES 2,C",
            "RES 2,D",
            "RES 2,E",
            "RES 2,H",
            "RES 2,L",
            "RES 2,(HL)",
            "RES 2,A",
            "RES 3,B",
            "RES 3,C",
            "RES 3,D",
            "RES 3,E",
            "RES 3,H",
            "RES 3,L",
            "RES 3,(HL)",
            "RES 3,A",
            "RES 4,B",
            "RES 4,C",
            "RES 4,D",
            "RES 4,E",
            "RES 4,H",
            "RES 4,L",
            "RES 4,(HL)",
            "RES 4,A",
            "RES 5,B",
            "RES 5,C",
            "RES 5,D",
            "RES 5,E",
            "RES 5,H",
            "RES 5,L",
            "RES 5,(HL)",
            "RES 5,A",
            "RES 6,B",
            "RES 6,C",
            "RES 6,D",
            "RES 6,E",
            "RES 6,H",
            "RES 6,L",
            "RES 6,(HL)",
            "RES 6,A",
            "RES 7,B",
            "RES 7,C",
            "RES 7,D",
            "RES 7,E",
            "RES 7,H",
            "RES 7,L",
            "RES 7,(HL)",
            "RES 7,A",
            "SET 0,B",
            "SET 0,C",
            "SET 0,D",
            "SET 0,E",
            "SET 0,H",
            "SET 0,L",
            "SET 0,(HL)",
            "SET 0,A",
            "SET 1,B",
            "SET 1,C",
            "SET 1,D",
            "SET 1,E",
            "SET 1,H",
            "SET 1,L",
            "SET 1,(HL)",
            "SET 1,A",
            "SET 2,B",
            "SET 2,C",
            "SET 2,D",
            "SET 2,E",
            "SET 2,H",
            "SET 2,L",
            "SET 2,(HL)",
            "SET 2,A",
            "SET 3,B",
            "SET 3,C",
            "SET 3,D",
            "SET 3,E",
            "SET 3,H",
            "SET 3,L",
            "SET 3,(HL)",
            "SET 3,A",
            "SET 4,B",
            "SET 4,C",
            "SET 4,D",
            "SET 4,E",
            "SET 4,H",
            "SET 4,L",
            "SET 4,(HL)",
            "SET 4,A",
            "SET 5,B",
            "SET 5,C",
            "SET 5,D",
            "SET 5,E",
            "SET 5,H",
            "SET 5,L",
            "SET 5,(HL)",
            "SET 5,A",
            "SET 6,B",
            "SET 6,C",
            "SET 6,D",
            "SET 6,E",
            "SET 6,H",
            "SET 6,L",
            "SET 6,(HL)",
            "SET 6,A",
            "SET 7,B",
            "SET 7,C",
            "SET 7,D",
            "SET 7,E",
            "SET 7,H",
            "SET 7,L",
            "SET 7,(HL)",
            "SET 7,A"
        };

        public static int[] opTimes =
       {1,3,2,2,1,1,2,1,5,2,2,2,1,1,2,1,
        2,3,2,2,1,1,2,1,3,2,2,2,1,1,2,1,
        2,3,2,2,1,1,2,1,2,2,2,2,1,1,2,1,
        2,3,2,2,3,3,3,1,2,2,2,2,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        2,2,2,2,2,2,1,2,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        1,1,1,1,1,1,2,1,1,1,1,1,1,1,2,1,
        2,3,3,4,3,4,2,4,2,4,3,1,3,6,2,4,
        2,3,3,0,3,4,2,4,2,4,3,0,3,0,2,4,
        3,3,2,0,0,4,2,4,4,1,4,0,0,0,2,4,
        3,3,2,1,0,4,2,4,3,2,4,1,0,0,2,4};
    }
}
