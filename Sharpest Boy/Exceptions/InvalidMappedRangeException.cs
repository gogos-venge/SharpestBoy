using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpestBoy.Exceptions {
    class InvalidMappedRangeException : Exception {
        public InvalidMappedRangeException(String msg) : base(msg) { }
    }
}
