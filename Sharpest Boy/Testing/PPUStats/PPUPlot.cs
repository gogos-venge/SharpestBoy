using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OxyPlot.Series;
using System.IO;

namespace SharpestBoy.Testing.PPUStats {
    public partial class PPUPlot : Form {
        private DMG.PPU.DMGPPU PPU;
        public PPUPlot(DMG.PPU.DMGPPU ppu) {
            InitializeComponent();
            PPU = ppu;

        }

        private void Button1_Click(object sender, EventArgs e) {
            /*
            ColumnSeries columnSeries = new ColumnSeries();
            for(int i = 0; i < PPU.Diagnostics.Length; i++) {
                columnSeries.Items.Add(new ColumnItem(PPU.Diagnostics[i], i));
            }
            
            OxyPlot.PlotModel model = new OxyPlot.PlotModel();
            model.Series.Add(columnSeries);

            plotView1.Model = model;

            plotView1.InvalidatePlot(true);
            */

            
            using (StreamWriter sw = new StreamWriter(new FileStream("clocks.txt", FileMode.Create))) {
                for (int i = 0; i < Program.Diagnostics.Length; i++) {
                    if(i == 16416) {
                        sw.WriteLine("============== VBLANK ===============");
                    }
                    sw.WriteLine("{0}({2}):{1}", i, Program.Diagnostics[i], (i*4) % 456);
                }
            }

        }
    }
}
