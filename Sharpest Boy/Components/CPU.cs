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

namespace SharpestBoy.Components {
    /// <summary>
    /// The Core of a board, capable of fetching and executing instructions.
    /// </summary>
    public abstract class CPU : Component {

        /// <summary>
        /// Fetches and executes one instruction. The CPU may as well serve interrupts during this call.
        /// </summary>
        abstract public void FetchAndExecute();

        /// <summary>
        /// A Quirk is a defect, bug, or strange behaviour a hardware component or peripheral may perform during operation.
        /// It might change the flow of CPU execution, the update of a certain peripheral, or a particular Memory register.
        /// A known quirk is the double instruction execution in Game Boy that happens when IME = false and IE & IF & 0x1F != 0.
        /// Quirks are workarounds (HLE) for blackbox (undocumented or unknown) circuitry operation so use them responsibly.
        /// It will be called each time the system advances by 1 machine clock
        /// </summary>
        abstract public void Quirk();
        
    }
}
