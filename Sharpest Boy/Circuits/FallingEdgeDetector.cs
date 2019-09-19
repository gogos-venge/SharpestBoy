namespace SharpestBoy.Circuits {
    /// <summary>
    /// A circuit that detects a signal change from Hi, to Low
    /// Use true as Hi, and false as Low.
    /// </summary>
    class FallingEdgeDetector {

        private bool Delay = false;

        public bool Check(bool Current) {
            bool outC = !Current && Delay;
            Delay = Current;
            return outC;
        }
    }
}
