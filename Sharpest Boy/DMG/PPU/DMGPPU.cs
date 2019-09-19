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

using SharpestBoy.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpestBoy.DMG.PPU {
    public class DMGPPU : Components.PPU {

        /* 0x8000-0x9FFF
         * 0xFF40-0xFF9F
         */
        private int Clock;
        private int LcdPointer;
        private bool EndOfFrame = false;
        private bool IgnoreIRQ = false;
        private int Vram_Clock_Offset = 0;
        private bool ZeroLineDelay = true;

        //MMIO Addresses
        const ushort _LCDC = 0xFF40;
        const ushort _STAT = 0xFF41;
        const ushort _SCY = 0xFF42;
        const ushort _SCX = 0xFF43;
        const ushort _LY = 0xFF44;
        const ushort _LYC = 0xFF45;
        const ushort _DMA = 0xFF46;
        const ushort _BGP = 0xFF47;
        const ushort _OBP0 = 0xFF48;
        const ushort _OBP1 = 0xFF49;
        const ushort _WY = 0xFF4A;
        const ushort _WX = 0xFF4B;

        //Other constants
        const byte VBLANK = 1;
        const byte HBLANK = 0;
        const byte OAM_READ = 2;
        const byte VRAM_READ = 3;

        //Dimensions
        const int ScreenWidth = 160;
        const int ScreenHeight = 144;

        //LCDC
        byte LCDC_0; //BG Display On
        byte LCDC_1; //Obj On flag
        byte LCDC_2; //Obj Block composition selection flag
        byte LCDC_3; //BG code area selection flag
        byte LCDC_4; //Character Data selection flag
        byte LCDC_5; //Windowing On flag
        byte LCDC_6; //Window code area selection flag
        byte LCDC_7; //LCD controller operation stop flag

        //LCDC helper
        int BGCodeArea = 0x1800; //0x9800
        int BGTileArea = 0; //0x8000
        int BGPrecalculatedCodeAddressY = 0; //optimization
        int BGPrecalculatedTileOffsetY = 0;
        bool BGSignedTile = false;

        //STAT
        byte STAT_01 = 1; //Mode flag
        byte STAT_2 = 0x4; //Match flag
        byte STAT_3; //Mode 0 H-Blank Check Enable
        byte STAT_4; //Mode 1 V-Blank Check Enable
        byte STAT_5; //Mode 2 OAM Check Enable
        byte STAT_6; //LY=LYC Check Enable 
        byte STAT_7 = 0x80; //Unused Always 1

        //BGP
        int[] BGP;

        //OBJ
        int[] OBP0;
        int[] OBP1;

        //Video memory
        byte[] VRAM;

        //Rest MMIO
        byte BGPREG = 0;
        byte OBP0REG = 0;
        byte OBP1REG = 0;
        byte SCY = 0;
        byte SCX = 0;
        byte LY = 0;
        byte LYC = 0;
        byte DMA = 0;
        byte WY = 0;
        byte WX = 0;

        MemoryManagementUnit Memory;
        Renderer Renderer;
        int[,] Screen;
        int[] Bitmap;

        public DMGPPU() {
            AddMemoryMappedIORange(0x8000, 0x9FFF);
            AddMemoryMappedIORange(0xFF40, 0xFF9F);
        }

        public override void Initialize() {
            Clock = 416;
            LcdPointer = 0;
            Memory = GetBoard().GetMemoryManagementUnit();
            Renderer = new Renderer();

            BGP = new int[4];
            OBP0 = new int[4];
            OBP1 = new int[4];
            VRAM = new byte[0x2000];

            Screen = new int[ScreenWidth, ScreenHeight];
            Bitmap = new int[ScreenWidth * ScreenHeight];

            MMIOWrite(0x91, _LCDC);
            MMIOWrite(0x85, _STAT);
        }

        public override int[] Draw() {
            for (int x = 0; x < ScreenWidth; x++) {
                for (int y = 0; y < ScreenHeight; y++) {
                    Bitmap[y * ScreenWidth + x] = Screen[x, y]; //we "serialize" the pixels with one simple linear equation.
                }
            }
            EndOfFrame = false;
            return Bitmap;
        }

        public override bool MMIORead(out byte value, int readAddress) {
            if (readAddress < 0xA000) {
                if(STAT_01 != 0 || true) {
                    value = VRAM[readAddress - 0x8000];
                    
                } else {
                    value = 0xFF;
                }
                return true;
            }

            switch (readAddress) {
                case _LCDC:
                    value = (byte)(LCDC_0 | LCDC_1 | LCDC_2 | LCDC_3 | LCDC_4 | LCDC_5 | LCDC_6 | LCDC_7);
                    return true;
                case _STAT:
                    value = (byte)(STAT_01 | STAT_2 | STAT_3 | STAT_4 | STAT_5 | STAT_6 | STAT_7);
                    return true;
                case _SCY:
                    value = SCY;
                    return true;
                case _SCX:
                    value = SCX;
                    return true;
                case _LY:
                    value = LY;
                    return true;
                case _LYC:
                    value = LYC;
                    return true;
                case _DMA:
                    //TODO
                    value = 0;
                    return false;
                case _BGP:
                    value = BGPREG;
                    return true;
                case _OBP0:
                    value = OBP0REG;
                    return true;
                case _OBP1:
                    value = OBP1REG;
                    return true;
                case _WY:
                    value = WY;
                    return true;
                case _WX:
                    value = WX;
                    return true;
            }
            value = 0;
            return false;
        }

        public override bool MMIOWrite(byte value, int writeAddress) {
            if (writeAddress < 0xA000) {
                if(STAT_01 != 3 || true) {
                    VRAM[writeAddress - 0x8000] = value;
                }                
                return true;
            }

            switch (writeAddress) {
                case _LCDC:
                    LCDC_0 = (byte)(value & 0x01);
                    LCDC_1 = (byte)(value & 0x02);
                    LCDC_2 = (byte)(value & 0x04);
                    LCDC_3 = (byte)(value & 0x08);
                    LCDC_4 = (byte)(value & 0x10);
                    LCDC_5 = (byte)(value & 0x20);
                    LCDC_6 = (byte)(value & 0x40);
                    LCDC_7 = (byte)(value & 0x80);

                    BGCodeArea = LCDC_3 == 0 ? 0x1800 : 0x1C00;
                    BGTileArea = (BGSignedTile = LCDC_4 == 0) ? 0x1000 : 0x0000;
                    return true;
                case _STAT:
                    STAT_3 = (byte)(value & 0x08);
                    STAT_4 = (byte)(value & 0x10);
                    STAT_5 = (byte)(value & 0x20);
                    STAT_6 = (byte)(value & 0x40);
                    return true;
                case _SCY:
                    SCY = value;
                    return true;
                case _SCX:
                    SCX = value;
                    return true;
                case _LY:
                    PrecalculateSelectedOffsetAddressY(LY = 0);
                    return true;
                case _LYC:
                    LYC = value;
                    return true;
                case _DMA:
                    //TODO
                    return false;
                case _BGP:
                    BGP[0] = (value & 0x03);
                    BGP[1] = ((value & 0x0C) >> 2);
                    BGP[2] = ((value & 0x30) >> 4);
                    BGP[3] = ((value & 0xC0) >> 6);
                    BGPREG = value;
                    return true;
                case _OBP0:
                    OBP0[0] = (value & 0x03);
                    OBP0[1] = ((value & 0x0C) >> 2);
                    OBP0[2] = ((value & 0x30) >> 4);
                    OBP0[3] = ((value & 0xC0) >> 6);
                    OBP0REG = value;
                    return true;
                case _OBP1:
                    OBP1[0] = (value & 0x03);
                    OBP1[1] = ((value & 0x0C) >> 2);
                    OBP1[2] = ((value & 0x30) >> 4);
                    OBP1[3] = ((value & 0xC0) >> 6);
                    OBP1REG = value;
                    return true;
                case _WY:
                    WY = value;
                    return true;
                case _WX:
                    WX = value;
                    return true;
            }
            return false;
        }

        public override void Update(int clocks) {
            if (STAT_7 != 0) {
                if (STAT_01 != VBLANK) {
                    //HBLANK EVENT ^^^^^^^^^^ (varies based on background position and sprites) ^^^^^^^^^^^^
                    if (Clock == 260 + Vram_Clock_Offset) {
                        //It's 172. But actually it's 174 (snaps on 2mhz) plus 4 the delay of HBLANK interrupt = 178.
                        //So clock should be 258 actually.
                        STAT_01 = HBLANK;

                        //RenderSpritesLine();

                        //in H-BLANK the lcd pointer goes back to zero
                        LcdPointer = 0;

                        //interrupt logic
                        HBLANKIRQ();

                        Vram_Clock_Offset = 0;

                    }
                    //OAM & VRAM EVENT ^^^^^^^^^ (change at 84 cycles) ^^^^^^^^^^^^^
                    else if (Clock == 88) { //should actually be 82 but that doesn't snap to CPU clock.
                        //mode 2 (reading OAM) to 3 (Reading OAM & VRAM) change logic
                        STAT_01 = VRAM_READ;

                        Vram_Clock_Offset = SCX & 7; //Background position affects timing
                        //LimitSpritesLine(); //limit sprites line should also affect Vram Clock
                    } else if (Clock == 4) {

                        //VBLANK EVENT ^^^^^^^^ (change at line 144 cycle 4) ^^^^^^^^^^^^^^
                        if (LY == 144) {
                            STAT_01 = VBLANK;

                            //VBLANK main interrupt
                            IRQ(1);

                            //Stop Drawing
                            EndOfFrame = true;

                            //coincidence 144
                            //LYCIRQ(144);

                            //VBLANK STAT IRQ
                            VBLANKIRQ();

                            //check_OAM_IRQ();
                            //OAMIRQ();

                            //window set to 0 (TODO: Fix this)
                            WY = 0;

                        }
                        //SCANLINE 0 STARTED EVENT ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                        else if (LY == 0) {
                            STAT_01 = OAM_READ;

                            //check LYC but don't generate interrupt
                            //LYCIRQ();

                            //OAMIRQ();
                        }
                        //SCANLINE 1-143 STARTED EVENT (CLOCK 4) ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                        else {
                            //Enters OAM read here
                            STAT_01 = OAM_READ;

                            //check LYC IRQ
                            LYCIRQ();
                            OAMIRQ();

                        }
                        //LINE STARTS EVENT AT CLOCK 0 ^^^^^^^^^^^^^^^^^^^^^^^^^^^
                    } else if (Clock == 0) {

                        if (LY > 0 && LY < 144) {
                            //For 1-143 Oam happens immediately
                            //OAMIRQ();
                        }

                    }

                    //VRAM & OAM RUNNING -------------------------
                    if (STAT_01 == VRAM_READ) {
                        //TODO: increase mode 3 duration based on B01s reads.
                        for (; LcdPointer < Clock - 84; LcdPointer++) {
                            if (LcdPointer < 160) { //"the rest of the pixels are unceremoniously cut off" (Nitty Gritty)
                                int TileAddress = GetTileAddress((LcdPointer + SCX) & 0xFF, out int Position); //Base address + offset (ex. 0x8200)
                                Screen[LcdPointer, LY] = Renderer.GetPixel(VRAM[TileAddress], VRAM[TileAddress + 1], Position, BGP);
                            }
                        }
                    }

                } else {
                    //VBLANK RUNNING -------------------------
                    if (Clock == 4) {
                        if (LY == 153) {
                            ZeroLineDelay = true;
                            PrecalculateSelectedOffsetAddressY(LY = 0);
                            //LYCIRQ();
                            //VBLANKIRQ();
                        }

                        //VBLANK STAT IRQ (not to confuse with the main interrupt that happens each VBL)
                        //check_OAM_IRQ();

                    } /*else if (Clock == 12) {
                        if (LY == 0) {
                            LYCIRQ(0);
                            OAMIRQ();
                            //IgnoreIRQ = false;
                        }
                    }*/
                }

                //LINE FINISHED DRAWING EVENT ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                if (Clock == 456) {
                    /*
                     * LY = 0 isn't changed immediately. After 153 -> 0, 0 spends 452
                     * cycles at VBLANK, then changes mode to 2
                     */
                    if (!ZeroLineDelay) {
                        PrecalculateSelectedOffsetAddressY(++LY);
                    } else {
                        ZeroLineDelay = !ZeroLineDelay;
                    }

                    //shouldn't change to 0 when VBLANK is not finished.
                    if (LY < 144) {
                        STAT_01 = HBLANK;

                    }
                    Clock = -clocks;
                }

                Clock += clocks;
            }
        }

        

        private void HBLANKIRQ() {
            if (STAT_3 != 0 && !IgnoreIRQ) {
                IRQ(2);
                //IgnoreIRQ = false;
            }
        }

        private void VBLANKIRQ() {
            if (STAT_4 != 0 && !IgnoreIRQ) {
                IRQ(2);
                //IgnoreIRQ = true;
            }
        }

        private void OAMIRQ() {
            if (STAT_5 != 0 && !IgnoreIRQ) {
                IRQ(2);
                //IgnoreIRQ = true;
            }
        }

        private void LYCIRQ() {
            if (LY == LYC) {
                STAT_2 = 4;
                if (STAT_6 != 0 && !IgnoreIRQ) {
                    IRQ(2);
                }
            } else STAT_2 = 0;
        }

        private void LYCIRQ(int LYX) {
            if (LY == LYX) {
                STAT_2 = 4;
                if (STAT_6 != 0 && !IgnoreIRQ) {
                    IRQ(2);
                }
            } else STAT_2 = 0;
        }

        private void IRQ(byte BIT) {
            Memory.DirectWrite((byte)(Memory.DirectRead(0xFF0F) | BIT), 0xFF0F);
        }

        /// <summary>
        /// Gets the Tile Address based on the X pixel and the Precalculated Y address (which is calculated in every PrecalculateSelectedOffsetAddressY call).
        /// </summary>
        /// <param name="LX">The current LCD X position</param>
        /// <param name="TileOffsetX">Has the value of the current pixel offset</param>
        /// <returns>The Tile Address</returns>
        private int GetTileAddress(int X, out int TileOffsetX) {
            int CodeAddressX = X >> 3; //Divide by 8 ("Snaps" to Code Map)
            int CodeAddress = BGPrecalculatedCodeAddressY + CodeAddressX; //Add to already calculated vertical Code Address
            int TileAddressStart = BGTileArea + (BGSignedTile ? (sbyte)(VRAM[CodeAddress]) << 4 : VRAM[CodeAddress] << 4); //Tiles are 16byte long and this is the Number of the tile so multiplying by 16 gives us its address
            TileOffsetX = X % 8;
            return TileAddressStart + BGPrecalculatedTileOffsetY;
        }

        /// <summary>
        /// Calculates the Y offset of the Code area (0x9800 | 0x9C00). Code area contains the offset for the Tile address (0x8000 | 0x9000)
        /// This is done for optimization. Since LX changes MUCH more frequently (160 times more than Y), we don't have to calculate LY in each call.
        /// Instead we calculate the address every time LY or SCY changes and save it.
        /// </summary>
        /// <param name="LY">The Current LCD Y position</param>
        private void PrecalculateSelectedOffsetAddressY(int LY) {
            int Y = (LY + SCY) & 0xFF;
            BGPrecalculatedCodeAddressY = ((Y >> 3) << 5) + BGCodeArea;
            BGPrecalculatedTileOffsetY = (Y % 8) << 1;  //Tiles come in a pair of bytes so we multiply by 2
        }

        public override bool CheckEndOfFrame() {
            return EndOfFrame;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
                object value = fi.GetValue(this);
                if (value is int || value is bool || value is byte || value is ushort) {
                    string name = fi.Name;
                    if(name.Length > 10) {
                        name = name.Substring(0, 7) + "...";
                    } else if (name.Length < 10) {
                        name = name.PadRight(10);
                    }
                    if (fi.Name.Equals("Clock")) {
                        sb.Append(System.String.Format("{0}\t{1}\n", name, value));
                        continue;
                    }
                    sb.Append(System.String.Format("{0}\t{1:X}\n", name, value));
                }
                    
            }
            return sb.ToString();
        }


    }
}
