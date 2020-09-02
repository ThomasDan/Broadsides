using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class ArtificialIntelligence
    {
        private Coordinate lastShipHitCoordinate;
        private bool lastShipHitSunk;
        private int streak;

        public Coordinate LastShipHitCoordinate
        {
            get { return this.lastShipHitCoordinate; }
            set { this.lastShipHitCoordinate = value; }
        }
        
        public bool LastShipHitSunk
        {
            get { return this.lastShipHitSunk; }
            set { this.lastShipHitSunk = value; }
        }
        public int Streak
        {
            get { return this.streak; }
            set { this.streak = value; }
        }

        public ArtificialIntelligence()
        {
            lastShipHitSunk = true;
            lastShipHitCoordinate = new Coordinate(0, 0);
            streak = 0;
        }

        public Random rnd = new Random();
        public Coordinate GetNextShot(field[][] board)
        {
            
            Coordinate nextShot = new Coordinate(this.lastShipHitCoordinate.Y, this.lastShipHitCoordinate.X);
            if (!this.lastShipHitSunk)
            {
                // Smart AI finishes the ship by shooting at adjacent squares!

                // If it does not already know the direction of the ship, it will randomly select one.
                if(this.lastShipHitCoordinate.X - 1 >= 0 && 
                    this.lastShipHitCoordinate.X + 1 < board[0].Length && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1]._Ship != null)
                {
                    
                    if(!board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit)
                    {
                        nextShot.X--;
                    }
                    else
                    {
                        nextShot.X += this.streak;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.X - 1 >= 0 && 
                    this.lastShipHitCoordinate.X + 1 < board[0].Length && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1]._Ship != null
                    )
                {
                    if (!board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit)
                    {
                        nextShot.X++;
                    }
                    else
                    {
                        nextShot.X -= this.streak;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.Y - 1 >= 0 && 
                    this.lastShipHitCoordinate.Y + 1 < board.Length && 
                    board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit && 
                    board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X]._Ship != null
                    )
                {
                    if (!board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y++;
                    }
                    else
                    {
                        nextShot.Y -= this.streak;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.Y - 1 >= 0 && 
                    this.lastShipHitCoordinate.Y + 1 < board.Length && 
                    board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit && 
                    board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X]._Ship != null
                    )
                {
                    if (!board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y--;
                    }
                    else
                    {
                        nextShot.Y += this.streak;
                    }
                }
                else
                {
                    // Since no hit adjacent ships, it will shoot a field, clock-wise rotation, unhit square adjacent
                    if(this.lastShipHitCoordinate.Y - 1 >= 0 && !board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y--;
                    }
                    else if(this.lastShipHitCoordinate.X + 1 < board[0].Length && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit)
                    {
                        nextShot.X++;
                    }
                    else if(this.lastShipHitCoordinate.Y + 1 < board.Length && !board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y++;
                    }
                    else if(this.lastShipHitCoordinate.X - 1 >= 0 && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit)
                    {
                        nextShot.X--;
                    }
                }
            }

            if(board[nextShot.Y][nextShot.X].IsHit)
            {
                // AI does not know of any ships and will therefore shoot randomly until it finds one.
                nextShot.X = rnd.Next(0, 10);
                nextShot.Y = rnd.Next(0, 10);
            }
            return nextShot;
        }
    }
}
