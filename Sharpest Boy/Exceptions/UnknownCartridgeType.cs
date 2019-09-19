using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpestBoy.Exceptions {
    class UnknownCartridgeType : Exception{
        public UnknownCartridgeType(String message) : base(message) { }
    }
}
