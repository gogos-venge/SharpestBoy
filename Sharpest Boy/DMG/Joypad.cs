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
using SharpestBoy.Components;

namespace SharpestBoy.DMG {
    public class Joypad : Peripheral {

        const ushort _P1 = 0xFF00;
        
        public enum State {
            Pressed,
            Unpressed
        }

        public enum Keys {
            CrossUp = 0x01,
            CrossLeft = 0x02,
            CrossDown = 0x04,
            CrossRight = 0x08,
            A = 0x10,
            B = 0x20,
            Select = 0x40,
            Start = 0x80
        }

        byte P1 = 0;

        byte _Cross, _Buttons;

        public Joypad() {
            AddMemoryMappedIORange(_P1, _P1);
            _Cross = _Buttons = 0xF;
        }

        public override void Initialize() {
        }

        public override bool MMIORead(out byte value, int readAddress) {
            value = (byte)(P1 | 0xC0);
            return true;
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            P1 = value;
            return true;
        }

        public override void Update(int Clocks) {
            if ((P1 & 0x10) == 0x10)
                P1 |= _Buttons;

            if ((P1 & 0x20) == 0x20)
                P1 |= _Cross;
        }

        public void Cross(Keys key, State state) {
            if(state == State.Pressed) {
                switch (key) {
                    case Keys.CrossUp:
                        _Cross &= 0x0B;
                        break;
                    case Keys.CrossLeft:
                        _Cross &= 0x0D;
                        break;
                    case Keys.CrossDown:
                        _Cross &= 0x07;
                        break;
                    case Keys.CrossRight:
                        _Cross &= 0x0E;
                        break;
                }
            } else {
                switch (key) {
                    case Keys.CrossUp:
                        _Cross |= 0x4;
                        break;
                    case Keys.CrossLeft:
                        _Cross |= 0x2;
                        break;
                    case Keys.CrossDown:
                        _Cross |= 0x8;
                        break;
                    case Keys.CrossRight:
                        _Cross |= 0x1;
                        break;
                }
            }
        }

        public void Buttons(Keys key, State state) {
            if (state == State.Pressed) {
                switch (key) {
                    case Keys.Select:
                        _Buttons &= 0x0B;
                        break;
                    case Keys.B:
                        _Buttons &= 0x0D;
                        break;
                    case Keys.Start:
                        _Buttons &= 0x07;
                        break;
                    case Keys.A:
                        _Buttons &= 0x0E;
                        break;
                }
            }
            else {
                switch (key) {
                    case Keys.Select:
                        _Buttons |= 0x4;
                        break;
                    case Keys.B:
                        _Buttons |= 0x2;
                        break;
                    case Keys.Start:
                        _Buttons |= 0x8;
                        break;
                    case Keys.A:
                        _Buttons |= 0x1;
                        break;
                }
            }
        }

    }
}
