using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class field
    {
        private bool isShip;
        private bool isHit;
        private ship _ship;
        public bool IsShip
        {
            get { return this.isShip; }
            set { this.isShip = value; }
        }
        public bool IsHit
        {
            get { return this.isHit; }
            set { this.isHit = value; }
        }
        public ship _Ship
        {
            get { return this._ship; }
            set { this._ship = value; }
        }

        public field()
        {
            isShip = false;
            isHit = false;
        }
    }
}
