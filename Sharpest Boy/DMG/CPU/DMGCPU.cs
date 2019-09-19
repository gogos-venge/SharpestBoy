using System;
using SharpestBoy.DMG.CPU.Units;
using SharpestBoy.Components;

namespace SharpestBoy.DMG.CPU {
    [Serializable]
    public class DMGCPU : Components.CPU {

        public Registers Registers { get; set; }
        public ControlUnit ControlUnit { get; set; }
        public InterruptService InterruptService { get; set; }
        public ArithmeticLogicUnit ALU { get; set; }

        public bool Halt { get; set; }
        public bool DoubleInstructionExecutionBug { get; set; }
        private int BugCount = 2;
        
        /// <summary>
        /// The Core of LR35902 circuit by SHARP.
        /// </summary>
        public DMGCPU() {

            Halt = false;
            DoubleInstructionExecutionBug = false;
        }
        
        public override void Initialize() {
            //Initialize Registers
            Registers = new Registers(this);

            //Initialize ALU
            ALU = new ArithmeticLogicUnit(this);

            //Initialize Interrupt Service
            InterruptService = new InterruptService(this);

            //Initialize Control Unit
            ControlUnit = new ControlUnit(this);

            /*
             * Interrupts should be served before fetching opcodes. So we check an interrupt here
             * before fetching the first opcode, as serving an interrupt in the loop before the fetch
             * would not allow us to break the debugger on interrupt vectors at 40h 48h 50h etc.
             */
            InterruptService.InterruptServiceRoutine();
        }

        /// <summary>
        /// Fetches and Executes the opcode, PC is pointing at, serving interrupts or halting when needed.
        /// CPU also advances IPeripheral components when needed to reflect the timing of the physical chip.
        /// </summary>
        public override void FetchAndExecute() {
            
            //Advance the components in time (fetch takes 1 machine cycle)
            GetBoard().AdvanceSystemTime(); //4
            
            //Halt will execute NOP. Essentially this is equivalent to just running the components at 4 clocks and returning
            if (!Halt) {

                //Fetch: Get the current opcode from memory
                byte Opcode = GetBoard().GetMemoryManagementUnit().Read(Registers.PC);

                //Execute: Call the Control Unit
                ControlUnit.Execute(Opcode);
            }

            /*
             * On Game Boy, Interrupts are served first, but since it's last on the loop,
             * this behaves as being first. For the first interrupt, we do an IR check in the Init() function
             */

            InterruptService.InterruptServiceRoutine();
        }

        /**
            This will mimic the behaviour of the Double Instruction bug. It works on instructions with a length greater than 1 byte for example:
            LD A, d8 is two bytes long and 0x3E is its instruction code. Whatever value follows after 0x3E in memory, is loaded in register A
            The next value in memory is for our example 0x07.
            
            PC: Program Counter (points which position in memory we are working with)

            Upon normal execution, the CPU would handle the code like this: Read instruction, then load 0x07 value in register A.
            PC=1    0x3E    Register.A = 0 (Read instruction)
            PC=2    0x07    Register.A = 7 (Load value in register A)

            In Double Execution Bug case though, the instruction becomes corrupted:
            PC=1    0x3E    Register.A = 0
            PC=1    0x3E    Register.A = 0x3E (PC fails to increase, so next byte is the instruction code again! 0x3E)
            PC=2    0x07    Register.A = 0x1C (Since CPU thinks unstruction LD A, d8 is finished executing, it executes 0x07 as instruction instead of a value!
            That turns 0x07 into the instruction RLCA, which rotates left with carry, register A, hence register A's value is now 0x1C)

            Info based on AntonioND The Cycle-Accurate Game Boy Docs
        */
        public override void Quirk() {
            if (DoubleInstructionExecutionBug) {
                if(--BugCount == 0) {
                    Registers.PC--;
                    DoubleInstructionExecutionBug = false;
                    BugCount = 2;
                }
            }
        }

        public override string ToString() {
            return String.Format("PC:\t\t{0:X4}\nSP:\t\t{5:X4}\nAF:\t\t{1:X4}\nBC:\t\t{2:X4}\nDE:\t\t{3:X4}\nHL:\t\t{4:X4}\n{6}",
                Registers.PC, Registers.GetAF(), Registers.GetBC(), Registers.GetDE(), Registers.GetHL(), Registers.SP, InterruptService.ToString());
        }

    }
}
