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
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SharpestBoy.DMG;
using SharpestBoy.Components;
using SharpestBoy.DMG.CPU;
using SharpestBoy.DMG.PPU;
using SharpestBoy.Cart;
using Be.Windows.Forms;
using SharpestBoy.Testing.Debugger;

namespace SharpestBoy {
    public partial class Debugger : Form {
        
        List<Symbol.Label> labels;

        Board Board;
        MemoryManagementUnit Memory;
        DMGCPU CPU;
        DMGPPU PPU;
        Cartridge cart;

        List<int> breakpoints = new List<int>();
        List<Disassembler.Assembly> assemblies = new List<Disassembler.Assembly>();
        Disassembler disasm;

        public Debugger(DMGBoard b) {
            InitializeComponent();

            disassembler.RetrieveVirtualItem += Debug_RetrieveVirtualItem;
            disassembler.DoubleBuffered(true);

            StackView.RetrieveVirtualItem += Stack_RetrieveVirtualItem;

            Shown += Debugger_Shown;

            KeyPreview = true;
            KeyDown += new KeyEventHandler(Debugger_KeyDown);
            

            hexBox1.ByteProvider = new DynamicMemoryByteProvider(b);
            hexBox1.Width = hexBox1.RequiredWidth + 20;
            
            
            labels = new List<Symbol.Label>();
            Board = b;
            Memory = b.GetMemoryManagementUnit();
            CPU = (DMGCPU)b.CPU;
            PPU = (DMGPPU)b.PPU;
            cart = (Cartridge)b.GetComponents()[0];
            

            String symbolPath = Path.GetDirectoryName(cart.GetRomPath()) + '\\' + Path.GetFileNameWithoutExtension(cart.GetRomPath()) + ".sym";
            labels = symbol_loader(symbolPath);

            disasm = new Disassembler(CPU);
            assemblies = disasm.Disassemble();
            disassembler.VirtualListSize = assemblies.Count + 0x8000;

            foreach(Components.Component c in Board.GetComponents()) {
                componentList.Items.Add(c.GetType().Name);
            }
            
        }

        private void Stack_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            ushort i = (ushort)((32767 - e.ItemIndex) * 2);
            e.Item = new ListViewItem(String.Format("{0:X4}: {1:X2}{2:X2}", i, Memory.Read((ushort)(i + 1)), Memory.Read(i)));

        }

        private void Debugger_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F7) {
                nextInstructionToolStripMenuItem_Click(sender, e);
            }

            if(e.KeyCode == Keys.F6) {
                nextCycleToolStripMenuItem_Click(sender, e);
            }

            if(e.KeyCode == Keys.F9) {
                nextIRQToolStripMenuItem_Click(sender, e);
            }

            if (e.KeyCode == Keys.F12) {
                nextHALTToolStripMenuItem_Click(sender, e);
            }
        }

        private void Debugger_Shown(object sender, EventArgs e) {
            
            SelectCurrentOpcode();
            populateState();
            
        }

        private void SelectCurrentOpcode() {
            int index = assemblies.FindIndex(o => o.Address == CPU.Registers.PC);
            disassembler.SelectedIndices.Add(index);
            disassembler.EnsureVisible(index);
            StackView.Invalidate();
            StackView.SelectedIndices.Add(65534 - CPU.Registers.SP);
            StackView.EnsureVisible(65534 - CPU.Registers.SP);
            hexBox1.Invalidate();

        }


        private void Debug_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            int i = e.ItemIndex;
            byte op = Memory.Read((ushort)i);
            e.Item = new ListViewItem();
            
            if(i < assemblies.Count) {
                if (breakpoints.Contains(assemblies[i].Address)) {
                    e.Item.BackColor = System.Drawing.Color.Red;
                }

                Disassembler.Assembly a = assemblies[e.ItemIndex];
                e.Item.SubItems.Add(String.Format("{1} ${0:X4}", a.Address, getLabelForAddress(0, a.Address))); //address

                StringBuilder sb = new StringBuilder();
                for(int y = 0; y < a.Length; y++) {
                    sb.Append(String.Format("{0:X2} ", a.Opcodes[y]));
                }
                e.Item.SubItems.Add(sb.ToString()); //byte

                string disassemble = "";
                if(a.Operand1 != null && a.Operand2 == null) {
                    
                    string left = "";
                    switch (a.Operand1.Action) {
                        case Disassembler.Assembly.Type.Value:
                            
                            left = String.Format("${0:X4}", a.Operand1.Value);
                            break;
                        case Disassembler.Assembly.Type.Ignore:
                            left = a.Operand1.Expression;
                            break;
                    }
                    disassemble = String.Format(a.Format, left);
                } else if (a.Operand2 != null) {
                    
                    string left = "";
                    switch (a.Operand1.Action) {
                        case Disassembler.Assembly.Type.Value:
                            left = String.Format("${0:X4}", a.Operand1.Value);
                            break;
                        case Disassembler.Assembly.Type.Ignore:
                            left = a.Operand1.Expression;
                            break;
                    }
                    string right = "";
                    switch (a.Operand2.Action) {
                        case Disassembler.Assembly.Type.Value:
                            
                            right = String.Format("${0:X4}", a.Operand2.Value);
                            break;
                        case Disassembler.Assembly.Type.Evaluation:
                            right = String.Format("(${1:X4}):${0:X2}", Memory.Read((ushort)a.Operand2.Value), a.Operand2.Value);
                            break;
                        case Disassembler.Assembly.Type.Register:
                            right = String.Format("${0:X2}", disasm.LookupOperand(a.Operand2));
                            break;
                    }
                    disassemble = String.Format(a.Format, left, right);
                } else {
                    disassemble = a.Format;
                }
                e.Item.SubItems.Add(disassemble); //byte
                e.Item.SubItems.Add(Disassembler.opcodeNames[a.Opcodes[0]]); //byte

                e.Item.SubItems.Add(String.Format("{0}", Disassembler.opTimes[a.Opcodes[0]] << 2)); //clocks

            } else {
                e.Item.SubItems.Add(String.Format("{1} ${0:X4}", (ushort)i, getLabelForAddress(0, (ushort)i))); //address
                e.Item.SubItems.Add(String.Format("0x{0:X2}", Memory.Read((ushort)e.ItemIndex))); //byte
                e.Item.SubItems.Add(String.Format("{0:X2}", "-"));
                e.Item.SubItems.Add(String.Format("{0:X2}", "-"));
            }
            
            
            e.Item.SubItems.Add(String.Format("{0}", "")); //comment

        }

        private void populateState() {
            State.Text = "";
            printComponent(CPU);
            for(int i = 0; i < componentList.CheckedIndices.Count; i++) {
                printComponent(Board.GetComponents()[componentList.CheckedIndices[i]]);
            }
            
        }

        private void printComponent(Components.Component c) {
            State.AppendText(c.GetType().Name + "\n");
            State.AppendText(c.ToString() + "\n\n");
        }

        private String IRQPerBit(byte ie) {
            StringBuilder sb = new StringBuilder();
            for(int i = 1; i<=0x10; i <<= 1) {
                int test = i & ie;
                switch (test) {
                    case 0x1:
                        sb.Append("VBL ");
                        break;
                    case 0x2:
                        sb.Append("LCD ");
                        break;
                    case 0x4:
                        sb.Append("TIMA ");
                        break;
                    case 0x8:
                        sb.Append("SERIAL ");
                        break;
                    case 0x10:
                        sb.Append("JOYPAD");
                        break;

                }
            }
            return sb.ToString();
        }

        private void nextInstructionToolStripMenuItem_Click(object sender, EventArgs e) {
            //run
            Board.Run();
            this.Suspend();
            populateState();
            SelectCurrentOpcode();
            this.Resume();
        }

        private void nextCycleToolStripMenuItem_Click(object sender, EventArgs e) {
            /*CPU.Interpreter(4);
            populateState();
            SelectCurrentOpcode();*/
        }

        private void nextIRQToolStripMenuItem_Click(object sender, EventArgs e) {
            while (true) {
                Board.Run();
                if (breakpoints.Contains(CPU.Registers.PC)) { break; }

            }
            populateState();
            SelectCurrentOpcode();
        }

        private void nextHALTToolStripMenuItem_Click(object sender, EventArgs e) {
            /*Display.end_of_frame = false;
            while (!Display.end_of_frame) {
                CPU.Interpreter(4);
                if (CPU.halt) { break; }

            }
            populateState();
            SelectCurrentOpcode();*/
        }

        private String getLabelForAddress(byte mbc, ushort address) {
            //look for symbol label
            Symbol.Label sl = labels.Find(s => s.address == address);
            if(sl != null) {
                return sl.description;
            }
            return "";
        }

        private List<Symbol.Label> symbol_loader(String path) {
            List<Symbol.Label> tmp = new List<Symbol.Label>();
            if (File.Exists(path)) {
                using (StreamReader symbolFileReader = new StreamReader(File.OpenRead(path))) {
                    Regex r = new Regex(@"(\d+):([a-f0-9]+)\s(.+)", RegexOptions.IgnoreCase);
                    while (!symbolFileReader.EndOfStream) {
                        String s = symbolFileReader.ReadLine();
                        if (r.IsMatch(s)) {
                            GroupCollection gc = r.Match(s).Groups;
                            Symbol.Label label = new Symbol.Label();
                            if (byte.TryParse(gc[1].Value, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out byte b)) {
                                label.bank = b;
                            }

                            if (ushort.TryParse(gc[2].Value, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out ushort a)) {
                                label.address = a;
                            }
                                
                            label.description = gc[3].Value;
                            tmp.Add(label);
                        }
                    }
                    
                }

            }

            return tmp;
        }

        private class Symbol {
            public class Label {
                public byte bank { get; set; }
                public ushort address { get; set; }
                public String description { get; set; }
            }
        }
        
        private void debug_DoubleClick(object sender, EventArgs e) {
            ListView lv = (ListView)sender;
            int indx = lv.SelectedIndices[0];
            ushort address = assemblies[indx].Address;
            if (breakpoints.Contains(address)) {
                breakpoints.Remove(address);
                lv.Items[indx].BackColor = System.Drawing.Color.White;
            } else {
                breakpoints.Add(assemblies[indx].Address);
                lv.Items[indx].BackColor = System.Drawing.Color.Red;
            }
            
            
        }

        private class DynamicMemoryByteProvider : IByteProvider {
            DMGBoard B;
            MemoryManagementUnit mmu;

            public DynamicMemoryByteProvider(DMGBoard b) {
                B = b;
                mmu = b.GetMemoryManagementUnit();
            }

            public long Length => GetLength();

            public event EventHandler LengthChanged;
            public event EventHandler Changed;

            public void ApplyChanges() {
                
            }

            public void DeleteBytes(long index, long length) {
                throw new NotImplementedException();
            }

            public bool HasChanges() {
                return true;
            }

            public void InsertBytes(long index, byte[] bs) {
                
            }

            public byte ReadByte(long address) {
                return mmu.Read((ushort)address);
            }

            public bool SupportsDeleteBytes() {
                return false;
            }

            public bool SupportsInsertBytes() {
                return false;
            }

            public bool SupportsWriteByte() {
                return true;
            }

            public void WriteByte(long address, byte b) {
                mmu.Write(b, (ushort)address);
            }

            private long GetLength() {
                return 0x10000;
            }
        }

        private void hexBox1_CurrentPositionInLineChanged(object sender, EventArgs e) {
            Status.Text = String.Format("{0:X4}", hexBox1.SelectionStart);
        }

        private void componentList_Click(object sender, EventArgs e) {
            populateState();
        }
    }

    
}
