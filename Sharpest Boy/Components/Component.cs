
namespace SharpestBoy.Components {
    /// <summary>
    /// An object that can be attached to a Board.
    /// </summary>
    public abstract class Component {
        private Board Board;

        /// <summary>
        /// Attaches a Component to a Board.
        /// </summary>
        /// <param name="b">The board to attach</param>
        public void SetBoard(Board b) {
            Board = b;
        }

        /// <summary>
        /// Gets the Board this Component is attached to.
        /// </summary>
        /// <returns>The attached board</returns>
        public Board GetBoard() {
            return Board;
        }

        /// <summary>
        /// Use it as a constructor to initialize Components that depend on a properly initialized Board.
        /// </summary>
        abstract public void Initialize();
        
    }
}
