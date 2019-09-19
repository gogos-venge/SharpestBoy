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
