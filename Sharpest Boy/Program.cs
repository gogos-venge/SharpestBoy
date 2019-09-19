using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.Windows;
using SharpestBoy.DMG;
using SharpestBoy.Testing.PPUStats;

using Device = SharpDX.Direct3D11.Device;
using FactoryD2D = SharpDX.Direct2D1.Factory;

namespace SharpestBoy {
    static class Program {

        static Bitmap backBufferBMP;
        static Form form;
        static Surface backBuffer;
        static RenderTarget renderTarget;
        static SwapChain swapChain;
        public static int[] Diagnostics = new int[17556];
        static bool STOP;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            for (int i = 0; i < Diagnostics.Length; i++) {
                Diagnostics[i] = 0;
            }
            // Create render target window
            form = new RenderForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ClientSize = new System.Drawing.Size(160 * 4, 144 * 4);

            form.WindowState = FormWindowState.Minimized;
            form.Show();
            form.Focus();
            form.WindowState = FormWindowState.Normal;
            

            // Create swap chain description
            var swapChainDesc = new SwapChainDescription() {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(160, 144, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
            
            // Create swap chain and Direct3D device
            // The BgraSupport flag is needed for Direct2D compatibility otherwise new RenderTarget() will fail!
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDesc, out Device device, out swapChain);

            // Get back buffer in a Direct2D-compatible format (DXGI surface)
            backBuffer = Surface.FromSwapChain(swapChain, 0);

            // Create Direct2D factory
            using (var factory = new FactoryD2D()) {
                var dpi = factory.DesktopDpi;

                // Create bitmap render target from DXGI surface
                renderTarget = new RenderTarget(factory, backBuffer, new RenderTargetProperties() {
                    DpiX = 0,
                    DpiY = 0,
                    MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                    PixelFormat = new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Ignore),
                    Type = RenderTargetType.Hardware,
                    Usage = RenderTargetUsage.None
                });
            }

            backBufferBMP = new Bitmap(renderTarget, new Size2(160, 144), new BitmapProperties(renderTarget.PixelFormat));

            
            Color[] palette = new Color[4];
            palette[0] = Color.White;
            palette[1] = Color.LightGray;
            palette[2] = Color.DarkGray;
            palette[3] = Color.Black;

            Color[] bitmap = new Color[160 * 144];

            

            while (true) {
                STOP = false;
                OpenFileDialog of = new OpenFileDialog {
                    Filter = "Game Boy Files (*.gb)|*.gb|Game Boy Color Files (*.gbc)|*.gbc",
                    RestoreDirectory = true
                };
                of.ShowDialog();
                DMGBoard Board = DMGBoard.Builder(of.FileName);

                AddKeyListeners(form, Board);
                Debugger d = new Debugger(Board);
                //d.Show();
                //Application.Run();

                RenderLoop rl = new RenderLoop(form);
                while (rl.NextFrame() && !STOP) {
                    
                    int[] temp = Board.RunOneFrame();
                    for (int i = 0; i < temp.Length; i++) {
                        bitmap[i] = palette[temp[i]];
                    }

                    renderTarget.BeginDraw();
                    renderTarget.Transform = Matrix3x2.Identity;
                    renderTarget.Clear(Color.Black);
                    backBufferBMP.CopyFromMemory(bitmap, 160 * 4);
                    renderTarget.DrawBitmap(backBufferBMP, 1f, BitmapInterpolationMode.NearestNeighbor);

                    renderTarget.EndDraw();
                    swapChain.Present(1, PresentFlags.Restart);

                }
                d.Dispose();

            }

            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
            

            
            //Application.Run();


        }

        public static void AddKeyListeners(Form form, DMGBoard board) {
            Joypad j = board.Joypad;
            
            form.KeyDown += (o, e) => {

                if(e.KeyCode == Keys.F1) {
                    STOP = true;
                }
                
                if (e.Alt && e.KeyCode == Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
                

                if (e.KeyCode == Keys.A)
                    j.Buttons(Joypad.Keys.A, Joypad.State.Pressed);

                if (e.KeyCode == Keys.Enter)
                    j.Buttons(Joypad.Keys.Start, Joypad.State.Pressed);

                if (e.KeyCode == Keys.Space)
                    j.Buttons(Joypad.Keys.Select, Joypad.State.Pressed);

                if (e.KeyCode == Keys.S)
                    j.Buttons(Joypad.Keys.B, Joypad.State.Pressed);

                //cross
                if (e.KeyCode == Keys.Down) {
                    j.Cross(Joypad.Keys.CrossDown, Joypad.State.Pressed);
                }

                if (e.KeyCode == Keys.Up) {
                    j.Cross(Joypad.Keys.CrossUp, Joypad.State.Pressed);
                }

                if (e.KeyCode == Keys.Left) {
                    j.Cross(Joypad.Keys.CrossLeft, Joypad.State.Pressed);
                }

                if (e.KeyCode == Keys.Right) {
                    j.Cross(Joypad.Keys.CrossRight, Joypad.State.Pressed);
                }
            };

            form.KeyUp += (o, e) => {
                if (e.KeyCode == Keys.A)
                    j.Buttons(Joypad.Keys.A, Joypad.State.Unpressed);

                if (e.KeyCode == Keys.Enter)
                    j.Buttons(Joypad.Keys.Start, Joypad.State.Unpressed);

                if (e.KeyCode == Keys.Space)
                    j.Buttons(Joypad.Keys.Select, Joypad.State.Unpressed);

                if (e.KeyCode == Keys.S)
                    j.Buttons(Joypad.Keys.B, Joypad.State.Unpressed);

                //cross
                if (e.KeyCode == Keys.Down)
                    j.Cross(Joypad.Keys.CrossDown, Joypad.State.Unpressed);


                if (e.KeyCode == Keys.Up)
                    j.Cross(Joypad.Keys.CrossUp, Joypad.State.Unpressed);


                if (e.KeyCode == Keys.Left)
                    j.Cross(Joypad.Keys.CrossLeft, Joypad.State.Unpressed);


                if (e.KeyCode == Keys.Right)
                    j.Cross(Joypad.Keys.CrossRight, Joypad.State.Unpressed);

            };
        }
    }
}
