using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpestBoy.Exceptions {
    class UnknownOpcodeException : Exception {

        public UnknownOpcodeException(int opcode, int address) : base(String.Format("The opcode 0x{0:X2} at ${1:X} cannot be decoded", opcode, address)) { 
            
        }
    }
}
