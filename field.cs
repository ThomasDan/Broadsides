using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class field
    {
        private bool isHit;
        private ship _ship;
        public bool IsHit
        {
            get { return this.isHit; }
            set { 
                this.isHit = value; 
                if(this._ship != null && value)
                {
                    this._ship.Hits++;
                }
            }
        }
        public ship _Ship
        {
            get { return this._ship; }
            set { this._ship = value; }
        }

        public field()
        {
            isHit = false;
        }
    }
}
