﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class Coordinate
    {
        private int x; // Horizontal / Row value
        private int y; // Vertical / Column value

        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        public Coordinate(int Y, int X)
        {
            this.x = X;
            this.y = Y;
        }
        public Coordinate(Coordinate coordinate)
        {
            this.x = coordinate.X;
            this.y = coordinate.Y;
        }

        public Coordinate Add(Coordinate direction)
        {
            return new Coordinate(this.y + direction.y, this.x + direction.x);
        }
    }
}
