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
using SharpestBoy.Exceptions;

namespace SharpestBoy.Components {

    /// <summary>
    /// Provides a Proxy class which accepts Components. Adding CPU and MMU is done seperately. The term "board" should not be confused
    /// with a PCB, as it may refer to the SoC itself ex. Game Boy where components like the Divider or the PPU are part of the SoC.
    /// The abstraction level does not limit to Game Boy design though as it *should* allow several systems (close to Game Boy) to be implemented.
    /// </summary>
    [Serializable]
    public class Board {

        public CPU CPU;
        private MemoryManagementUnit MMU;
        private Component[] Components;
        private Peripheral[] Peripherals;
        private MMIO[] MMIOs;

        /// <summary>
        /// Initializes a new board.
        /// </summary>
        /// <param name="Components">An array populated with Components (MappedComponents, Peripherals, UnmappedPeripherals or plain Components).</param>
        public Board(Component[] Components) {
            SplitPopulateComponents(Components);
        }

        /// <summary>
        /// Sets a CPU to the board.
        /// </summary>
        /// <param name="cpu">The CPU</param>
        public void SetCPU(CPU cpu) {
            cpu.SetBoard(this);
            CPU = cpu;
        }

        /// <summary>
        /// Gets the CPU of the board.
        /// </summary>
        /// <returns>The CPU</returns>
        public CPU GetCPU() {
            return CPU;
        }

        /// <summary>
        /// Sets a Memory Management Unit to the board.
        /// </summary>
        /// <param name="mmu">The Memory Management Unit</param>
        public void SetMemoryManagementUnit(MemoryManagementUnit mmu) {
            mmu.SetBoard(this);
            MMU = mmu;
        }

        /// <summary>
        /// Gets the Memory Management Unit of the board.
        /// </summary>
        /// <returns>The Memory Management Unit</returns>
        public MemoryManagementUnit GetMemoryManagementUnit() {
            return MMU;
        }

        /// <summary>
        /// Synchronizes the updatable components (Peripherals) with the clocks this board is running at.
        /// </summary>
        public void AdvanceSystemTime() {
            for(int i = 0; i < Peripherals.Length; i++) {
                Peripherals[i].Update(Peripherals[i].Clocks);
            }
            CPU.Quirk();
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <returns>Components</returns>
        public Component[] GetComponents() {
            return Components;
        }

        /// <summary>
        /// Gets the memory mapped components.
        /// </summary>
        /// <returns>MMIOs</returns>
        public MMIO[] GetMappedRanges() {
            return MMIOs;
        }

        /// <summary>
        /// Gets the peripherals.
        /// </summary>
        /// <returns>Peripherals</returns>
        public Peripheral[] GetPeripherals() {
            return Peripherals;
        }

        /// <summary>
        /// Runs the CPU fetch and execute method once.
        /// </summary>
        public void Run() {
            CPU.FetchAndExecute();
        }

        /// <summary>
        /// Calls the Initialize method on every component of this board.
        /// </summary>
        public void InitializeComponents() {
            if(MMU == null) {
                throw new InitializationFailedException("Improper initialization: MMU cannot be null");
            }
            if(CPU == null) {
                throw new InitializationFailedException("Improper initialization: CPU cannot be null");
            }

            MMU.Initialize();
            foreach (Component c in Components) {
                c.Initialize();
            }
            CPU.Initialize();
        }

        private void SplitPopulateComponents(Component[] components) {
            List<MMIO> mios = new List<MMIO>();
            List<Peripheral> peripherals = new List<Peripheral>();

            foreach(Component c in components) {
                if(c is Peripheral p) {
                    peripherals.Add(p);
                }

                if (c is MappedComponent m) {
                    m.Ranges.ForEach(mr => mios.Add(mr));
                }
                c.SetBoard(this);
            }

            MMIOs = mios.ToArray();
            Peripherals = peripherals.ToArray();
            Components = components;
        }

        public class MMIO {

            public int Lo { get; private set; }
            public int Hi { get; private set; }
            public MappedComponent C { get; private set; }

            public MMIO(int lo, int hi, MappedComponent c) {
                Lo = lo;
                Hi = hi;
                C = c;
            }

            public override string ToString() {
                return String.Format("{0:X4}-{1:X4}", Lo, Hi);
            }
        }

    }
}
