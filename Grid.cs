using System;
using System.Collections.Generic;

using Game.Foundation;
using Microsoft.Xna.Framework;

namespace LD10
{
    /*
    public class Grid : Behavior
    {
        int columns = 1, rows = 1;
        float tileSize = 1;

        public event EventHandler Changed;

        private void OnChanged()
        {
            if (Changed != null) {
                Changed(this, EventArgs.Empty);
            }
        }

        public Grid()
            : this(10, 10, 1) { }

        public Grid(int columns, int rows, float tileSize)
        {
            this.columns = columns;
            this.rows = rows;
            this.tileSize = tileSize;
        }

        public float TileSize
        {
            get
            {
                return tileSize;
            }
            set
            {
                if (tileSize != value) {
                    tileSize = value;

                    OnChanged();
                }
            }
        }

        public int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (columns != value) {
                    columns = value;

                    OnChanged();
                }
            }
        }

        public int Rows
        {
            get
            {
                return rows;
            }
            set
            {
                if (rows != value) {
                    rows = value;

                    OnChanged();
                }
            }
        }
    }*/
}
