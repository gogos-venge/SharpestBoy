﻿/**
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
    /// A Component that takes up MMIO registers and also needs to be updated using the CPU timer. (ex. Most of components like Display, Audio, Timer etc)
    /// </summary>
    public abstract class Peripheral : MappedComponent, IPeripheral {

        public int Clocks { get; set; } = 4;
        public abstract void Update(int Clocks);

    }
}
