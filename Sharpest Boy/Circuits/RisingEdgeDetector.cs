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

namespace SharpestBoy.Circuits {
    /// <summary>
    /// A circuit that detects a signal change from Low, to Hi
    /// Use true as Hi, and false as Low.
    /// </summary>
    class RisingEdgeDetector {

        private bool Delay = false;

        public bool Check(bool Current) {
            bool outC = Current && !Delay;
            Delay = Current;
            return outC;
        }
    }
}
