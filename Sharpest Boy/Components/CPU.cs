
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
