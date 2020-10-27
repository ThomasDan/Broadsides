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
            NO: 00		 Y,  X
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

        /// <summary>
        /// Gets the next planned or random shot for the AI.
        /// </summary>
        /// <param name="board">Player's board.</param>
        /// <returns>Coordinates for the next potential shot</returns>
        public Coordinate GetNextShot(Field[][] board)
        {
            /* Modus Operandi of the AI:
           
            1. Randomly hit ship.
                -lastShipHit Set, LastShipSunk False, streak++
                -Go through next steps until ship is Sunk or lost.
           
            2. Shoot adjacent squares to find Direction of ship.
                -When next part found: lastShipHit set, Direction set, streak++
           
            3. Shoot in the Direction until ship sinks OR...
                -lastShipHit set, streak++
        
            4. ..AI wants to shoot a Hit Field OR it points outside the Board. Time to turn back around to get the other half of ship.
                -lastShipHit set, Direction Inverted, 
            */

            Coordinate nextShot = new Coordinate(this.lastShipHitCoordinate.Y, this.lastShipHitCoordinate.X);

            if (!this.lastShipHitSunk)
            {
                // The AI knows of an unsunken ship!
                if (this.direction.Y == 0 && this.direction.X == 0)
                {
                    // The AI does Not know the Direction of the Ship, therefore..
                    // ..It will shoot at a random adjacent field, if it has not already been hit.

                    // List of Potential Directions: Down, Up, Right, Left
                    List<Coordinate> potenDirects = new List<Coordinate>
                    {
                        new Coordinate(1,0), new Coordinate(-1,0), new Coordinate(0,1), new Coordinate(0,-1)
                    };
                    bool goodPotDirect = false;
                    int index;
                    while (!goodPotDirect)
                    {
                        index = rnd.Next(0, potenDirects.Count);
                        if(
                            // If the Potentail Direction points to a field Outside the Board...
                            this.lastShipHitCoordinate.Y + potenDirects[index].Y < 0 || this.lastShipHitCoordinate.Y + potenDirects[index].Y >= board.Length ||
                            this.lastShipHitCoordinate.X + potenDirects[index].X < 0 || this.lastShipHitCoordinate.X + potenDirects[index].X >= board.Length ||
                            // ..or to a Field which has already been hit..
                            board[this.lastShipHitCoordinate.Y + potenDirects[index].Y][this.lastShipHitCoordinate.X + potenDirects[index].X].IsHit
                            )
                        {
                            // Remove the potential direction from Potential Directions, for it has no potential!
                            potenDirects.RemoveAt(index);
                        }
                        else
                        {
                            // This Potential Direction is a Good Potential Direction, and will be shot at!
                            goodPotDirect = true;
                            nextShot.Y += potenDirects[index].Y;
                            nextShot.X += potenDirects[index].X;
                        }
                    }
                }
                else
                {
                    // The AI Knows the Direction of the Ship!
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
                        // The Direction points Outside the board or to a Field which has already been Hit, so it is time for a Reversal using Direction and STREAK!

                        // Reversing Direction first
                        this.direction.Y = this.direction.Y * -1;
                        this.direction.X = this.direction.X * -1;
                        
                        // Applying Streak multiplied with Direction, which lands the shot right on the other half of the ship.
                        nextShot.Y += this.direction.Y * this.streak;
                        nextShot.X += this.direction.X * this.streak;
                    }
                }
            }
            // One last Contingency Check, just in case that the AI wants to shoot at a Field which has already been Hit or does not exist (Outside the board).
            nextShot = NextShotContingency(nextShot, board);
            return nextShot;
        }

        /// <summary>
        /// Contingency Check and Apply if the Next Shot is not a sensible one, or it does not know what to shoot at.
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
                this.ResetTargetting();
            }
            return nextShot;
        }

        /// <summary>
        /// Resets the AI's targetting, making it forget about any sunk ships or ships it failed to sink.
        /// </summary>
        public void ResetTargetting()
        {
            this.streak = 0;
            this.direction = new Coordinate(0, 0);
            this.lastShipHitSunk = true;
        }
    }
}
