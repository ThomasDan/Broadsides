﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class Coordinate
    {
        private int x;
        private int y;

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
            x = X;
            y = Y;
        }
    }
}