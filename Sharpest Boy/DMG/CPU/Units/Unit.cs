using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpestBoy.DMG.CPU.Units {
    public class Unit {

        private DMGCPU cpu;

        public Unit(DMGCPU cpu) {
            this.cpu = cpu;
        }

        public void SetCPU(DMGCPU cpu) {
            this.cpu = cpu;
        }

        public DMGCPU GetCPU() {
            return cpu;
        }

    }
}
