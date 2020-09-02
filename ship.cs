using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class ship
    {
        private string type;
        private int length;
        private int hits;
        private bool sunk;
        
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }
        public int Hits
        {
            get { return this.hits; }
            set 
            { 
                this.hits = value;
                if(this.hits == length)
                {
                    this.sunk = true;
                }
            }
        }
        public bool Sunk
        {
            get { return this.sunk; }
        }
        public ship(string Type, int Length)
        {
            type = Type;
            length = Length;
            hits = 0;
            sunk = false;
        }
    }
}
