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
            lastShipHitCoordinate = new Coordinate(rnd.Next(0,10), rnd.Next(0, 10));
            streak = 0;
        }

        public Random rnd = new Random();

        /// <summary>
        /// Get the next coordinates that the AI wants to shoot at.
        /// </summary>
        /// <param name="board">Opposing player's board.</param>
        /// <returns>Coordinates Computer wants to shoot at.</returns>
        public Coordinate GetNextShot(Field[][] board)
        {
            Coordinate nextShot = new Coordinate(this.lastShipHitCoordinate.Y, this.lastShipHitCoordinate.X);
            if (!this.lastShipHitSunk)
            {
                // Smart AI finishes the ship by shooting at adjacent squares!

                // Computer checks if it can find a likely direction for the ship (By having hit two adjacent squares, based on the lastShipHitCoordinate)
                // FOr example, if it hit the ship last turn, and there's a ship left of that, the next part of the ship might be on the right.
                if(this.lastShipHitCoordinate.X - 1 >= 0 && 
                    this.lastShipHitCoordinate.X + 1 < board[0].Length && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1]._Ship != null)
                {
                    // However! If the next space in the end of a direction has already been hit, it's time to turn around instead, using streak to return to the other end of the ship.
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
                // AI wants to shoot at a field that has already been hit. This is either due to not knowing of a ship or the AI spazzing out and trying to hit fields it has already hit.
                // So this acts as not just the AI getting random coordinates, but also as a contingency in case the AI goes bonkers.
                nextShot.X = rnd.Next(0, 10);
                nextShot.Y = rnd.Next(0, 10);
                this.streak = 0;
                this.lastShipHitSunk = true;
            }
            return nextShot;
        }
    }
}
