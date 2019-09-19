using System;
using System.IO;
using SharpestBoy.Components;
using SharpestBoy.DMG.CPU;
using SharpestBoy.DMG.DIV;
using SharpestBoy.DMG.PPU;
using SharpestBoy.Cart;

namespace SharpestBoy.DMG {

    /// <summary>
    /// Builder class for DMGBoard
    /// </summary>
    public class DMGBoard : Board {

        public Components.PPU PPU;
        public Joypad Joypad;

        public static DMGBoard Builder(String RomPath) {

            Component[] components = {
                new Cartridge(File.OpenRead(RomPath)),
                new DMGPPU(),
                new Divider(),
                new NullSerial(),
                new Joypad(),
                new EchoRAM()
            };

            DMGBoard board = new DMGBoard(components);
            board.SetCPU(new DMGCPU());
            board.SetMemoryManagementUnit(new MMU());
            board.InitializeComponents();

            return board;
        }

        public DMGBoard(Component[] components) : base(components) {
            foreach(Component c in components) {
                if(c is Components.PPU) {
                    PPU = (Components.PPU)c;
                } else if (c is Joypad) {
                    Joypad = (Joypad)c;
                }
            }
        }

        public int[] RunOneFrame() {
            while (!PPU.CheckEndOfFrame()){
                Run();
            }
            return PPU.Draw();
        }
        
    }
}
