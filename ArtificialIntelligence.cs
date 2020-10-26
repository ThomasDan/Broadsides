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
        private Coordinate direction; // Newest Shot which hit a ship - Oldest Shot which hit a ship
        /*
            DIRECTION:
            NO: 00		 0,  0
            UP: -Y 		-1,  0
            DW: +Y 		 1,  0
            RT: +X		 0,  1
            LT: -X		 0, -1
        */
        private bool lastShipHitSunk;
        private int streak;

        public Coordinate LastShipHitCoordinate
        {
            get { return this.lastShipHitCoordinate; }
            set { this.lastShipHitCoordinate = value; }
        }
        public Coordinate Direction
        {
            
            get { return this.direction; }
            set { this.direction = value; }
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
            direction = new Coordinate(0, 0); // Initially no direction
            streak = 0;
        }


        public Random rnd = new Random();

        public Coordinate NewGetNextShot(Field[][] board)
        {
            /* Modus Operandi of the AI:

            1. Randomly hit ship.
                -lastShipHit Set, LastShipSunk False

            2. Shoot adjacent squares to find Direction of ship.
                -

            3. Follow Direction until ship sinks OR hit Hit Field (Ship or no Ship) OR Point outside Board.

            4. (In case of water) Use STREAK and Direction to turn back around to get other half of ship.
            */


            Coordinate nextShot = new Coordinate(this.lastShipHitCoordinate.Y, this.lastShipHitCoordinate.X);

            if (!this.lastShipHitSunk)
            {
                // The AI knows of an unsunken ship!
                if (this.direction.Y == 0 && this.direction.X == 0)
                {
                    // The AI does Not know the Direction of the Ship, therefore..
                    // ..It will shoot the adjacent fields in a clock-wise rotation until it finds another part of a ship.
                    if (this.lastShipHitCoordinate.Y - 1 >= 0 && !board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y--;
                    }
                    else if (this.lastShipHitCoordinate.X + 1 < board[0].Length && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit)
                    {
                        nextShot.X++;
                    }
                    else if (this.lastShipHitCoordinate.Y + 1 < board.Length && !board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y++;
                    }
                    else if (this.lastShipHitCoordinate.X - 1 >= 0 && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit)
                    {
                        nextShot.X--;
                    }
                }
                else
                {
                    // The AI Knows the Direction of the Ship!
                    Console.WriteLine("I KNOW THE DIRECTION!! " + this.direction.Y + ", " + this.direction.X); // test
                    if (
                        // If the Direction points inside the board or not
                        nextShot.Y + this.direction.Y >= 0 && nextShot.Y + this.direction.Y < board[0].Length &&
                        nextShot.X + this.direction.X >= 0 && nextShot.X + this.direction.X < board[0].Length &&
                        // If the Direction points to a Field which is Hit or not.
                        !board[nextShot.Y + this.direction.Y][nextShot.X + this.direction.X].IsHit
                        )
                    {
                        // The Direction points to a Field inside the Board which has not been hit
                        // The field has not been shot, and is inside the board, and should therefore be shot!
                        nextShot.Y += this.direction.Y;
                        nextShot.X += this.direction.X;
                    }
                    else
                    {
                        // Reversing Direction
                        this.direction.Y = this.direction.Y * -1;
                        this.direction.X = this.direction.X * -1;
                        // The Direction points Outside the board or to a Field which has already been Hit, time for a Reversal using Direction and STREAK!
                        nextShot.Y += this.direction.Y * this.streak;
                        nextShot.X += this.direction.X * this.streak;
                    }
                }
            }

            nextShot = NextShotContingency(nextShot, board);
            return nextShot;
        }

        /// <summary>
        /// Contingency Check and Apply if the Next Shot is a sensible one.
        /// </summary>
        /// <param name="nextShot">AI's suggested next shot.</param>
        /// <param name="board">Player's Board, which the AI is about to shoot at.</param>
        /// <returns>An approved shot or a randomized shot.</returns>
        public Coordinate NextShotContingency(Coordinate nextShot, Field[][] board)
        {
            if (nextShot.Y < 0 || nextShot.Y >= board.Length || nextShot.X < 0 || nextShot.X >= board[0].Length || board[nextShot.Y][nextShot.X].IsHit)
            {
                // AI wants to shoot at a field that has already been hit or is located outside of the board.
                // THese are not acceptable solutions, therefore AI must forget about the current ship (If any) and return to taking random potshots until it hits a ship.
                nextShot.X = rnd.Next(0, 10);
                nextShot.Y = rnd.Next(0, 10);
                this.streak = 0;
                this.direction = new Coordinate(0, 0);
                this.lastShipHitSunk = true;
            }
            return nextShot;
        }

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
                if(
                    this.lastShipHitCoordinate.X + 1 < board[0].Length && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1]._Ship != null &&
                    this.streak > 1)
                {
                    // However! If the next space in the end of a direction has already been hit, it's time to turn around instead, using streak to return to the other end of the ship.
                    if(this.lastShipHitCoordinate.X - 1 >= 0 && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit)
                    {
                        nextShot.X--;
                    }
                    else if (this.lastShipHitCoordinate.X + this.streak < board[0].Length)
                    {
                        nextShot.X += this.streak;
                        this.streak = 0;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.X - 1 >= 0 && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1].IsHit && 
                    board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X - 1]._Ship != null &&
                    this.streak > 1)
                {
                    if (this.lastShipHitCoordinate.X + 1 < board[0].Length && !board[this.lastShipHitCoordinate.Y][this.lastShipHitCoordinate.X + 1].IsHit)
                    {
                        nextShot.X++;
                    }
                    else if (this.lastShipHitCoordinate.X - this.streak >= 0)
                    {
                        nextShot.X -= this.streak;
                        this.streak = 0;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.Y - 1 >= 0 &&
                    board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit && 
                    board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X]._Ship != null &&
                    this.streak > 1)
                {
                    if (this.lastShipHitCoordinate.Y + 1 < board.Length && !board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y++;
                    }
                    else if (this.lastShipHitCoordinate.Y - this.streak >= 0)
                    {
                        nextShot.Y -= this.streak;
                        this.streak = 0;
                    }
                }
                else if(
                    this.lastShipHitCoordinate.Y + 1 < board.Length && 
                    board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X].IsHit && 
                    board[this.lastShipHitCoordinate.Y + 1][this.lastShipHitCoordinate.X]._Ship != null &&
                    this.streak > 1)
                {
                    if (this.lastShipHitCoordinate.Y - 1 >= 0 && !board[this.lastShipHitCoordinate.Y - 1][this.lastShipHitCoordinate.X].IsHit)
                    {
                        nextShot.Y--;
                    }
                    else if(this.lastShipHitCoordinate.Y + this.streak < board.Length)
                    {
                        nextShot.Y += this.streak;
                        this.streak = 0;
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

            nextShot = NextShotContingency(nextShot, board);
            return nextShot;
        }
    }
}
