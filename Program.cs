﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class Program
    {
        public static field[][] playerBoard;
        public static List<ship> playerShips = new List<ship>();
        public static field[][] computerBoard;
        public static List<ship> computerShips = new List<ship>();
        public static ArtificialIntelligence computer = new ArtificialIntelligence();
        static void Main(string[] args)
        {
            bool quit = false;
            char input;
            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Broadsides!\n\nPlease select an option: 1. New Game | 2. Quit\n");

                input = Console.ReadKey().KeyChar;
                switch (input)
                {
                    case '1':
                        GameSetup();
                        break;
                    case '2':
                        quit = true;
                        break;
                }
            }
        }

        /// <summary>
        /// The Pre-Game setup where Player & Computer sets down their ships.
        /// </summary>
        public static void GameSetup()
        {
            Console.Clear();
            // Resetting the Player.
            playerBoard = GenerateEmptyTenByTen();
            playerShips.Clear();
            playerShips.Add(new ship("Aircraft Carrier", 5));
            playerShips.Add(new ship("Battleship", 4));
            playerShips.Add(new ship("Destroyer", 3));
            playerShips.Add(new ship("Uboat", 3));
            playerShips.Add(new ship("Patrolship", 2));

            // Resetting the Computer.
            computerBoard = GenerateEmptyTenByTen();
            computerShips.Clear();
            computerShips.Add(new ship("Aircraft Carrier", 5));
            computerShips.Add(new ship("Battleship", 4));
            computerShips.Add(new ship("Destroyer", 3));
            computerShips.Add(new ship("Uboat", 3));
            computerShips.Add(new ship("Patrolship", 2));

            foreach(ship Ship in playerShips)
            {
                Console.WriteLine("This is your board, place your ship!");
                DrawBoard(playerBoard);


                Console.WriteLine("\nYou are about to place down your " + Ship.Type + ", it is " + Ship.Length + " fields long.\nh. Horizontal | v. Vertical");
                bool horizontal = true;


                bool validSelection = false;
                char input;
                while (!validSelection)
                {
                    input = Console.ReadKey().KeyChar;
                    switch (input)
                    {
                        case 'h':
                            validSelection = true;
                            horizontal = true;
                            break;
                        case 'v':
                            validSelection = true;
                            horizontal = false;
                            break;
                    }
                }

                
                bool validPosition = false;
                
                while(!validPosition)
                {

                    // Acquiring coordinates. X = Horizontal, Y = Vertical
                    Console.WriteLine("\nHorizontal start position:");
                    int x = AcquireValidIntegerWithin(0, 9 - (horizontal ? Ship.Length-1 : 0));

                    Console.WriteLine("\nVertical start position:");
                    int y = AcquireValidIntegerWithin(0, 9 - (!horizontal ? Ship.Length-1 : 0));
                    // These positions have Room for these ships, but-!
                    // .. We need to check if any other ships are blocking it
                    
                    
                    if(ShipInTheWay(playerBoard, Ship, x, y, horizontal))
                    {
                        Console.WriteLine("A ship is already placed there!");
                    }
                    else
                    {
                        for (int i = 0; i < Ship.Length; i++)
                        {
                            if (horizontal)
                            {
                                playerBoard[y][x + i]._Ship = Ship;
                            }
                            else
                            {
                                playerBoard[y + i][x]._Ship = Ship;
                            }
                        }
                        validPosition = true;
                        Console.WriteLine(Ship.Type + " successfully placed at " + (y+1) + ", " + (x+1));
                    }
                }
            }
            Console.Clear();

            // Computer ship placements
            Random rnd = new Random();
            // The computer is dumb, and will need to make randomized choices.

            foreach (ship Ship in computerShips)
            {
                bool validPosition = false;
                // While the computer does Not have a valid position for its ship..
                while (!validPosition)
                {
                    // Horizontal or not?
                    bool horizontal = (rnd.Next(0, 2) == 1);

                    // If it is horizontal, than the x-value is adjusted to the length of the ship, thereby preventing the ship being placed out of bounds.
                    int x = rnd.Next(0, 10 - (horizontal ? Ship.Length : 0));
                    // If it is not hortizontal, then the y-value (Vertical coordinate) is adjusted to the length of the ship, -||-
                    int y = rnd.Next(0, 10 - (!horizontal ? Ship.Length : 0));

                    // If there is no ship already on this board, along the length of the ship we're about to place, at x and y coordinate, horizontally or not...
                    if (!ShipInTheWay(computerBoard, Ship, x, y, horizontal))
                    {
                        // Then Place the ship on those Fields!
                        for (int i = 0; i < Ship.Length; i++)
                        {
                            if (horizontal)
                            {
                                computerBoard[y][x + i]._Ship = Ship;
                            }
                            else
                            {
                                computerBoard[y + i][x]._Ship = Ship;
                            }
                        }
                        validPosition = true;
                    }
                }
            }

            GameLoop();
        }

        /// <summary>
        /// The game proper, where Computer and Player take turns trying to sink each other's ships.
        /// </summary>
        public static void GameLoop()
        {
            bool gameOver = false;
            int round = 0;
            while (!gameOver)
            {
                Console.Clear();

                // Pre-Round test to see if User has lost.
                if (AllShipsSunk(playerShips))
                {
                    Console.Clear();
                    gameOver = true;
                    Console.WriteLine("All of your ships have sunk! You LOSE!");
                }

                if (!gameOver)
                {
                    round++;
                    Console.WriteLine("####### R O U N D   " + round + " #######");

                    DrawBoard(playerBoard);

                    Console.WriteLine("\nYour ships and how they are doing:");
                    foreach (ship Ship in playerShips)
                    {
                        Console.WriteLine(Ship.Type + " HP: " + (Ship.Length - Ship.Hits) + "/" + Ship.Length);
                    }
                    Console.WriteLine("Press Any Key to Continue to Shooting");
                    Console.ReadKey();
                    Console.WriteLine();


                    // Player gets first shot!

                    bool validShot = false;
                    while (!validShot)
                    {
                        Console.Clear();
                        DrawTacticalDisplay(computerBoard);

                        Console.WriteLine("Please choose a horizontal coordinate:");
                        int x = AcquireValidIntegerWithin(0, 9);
                        Console.WriteLine("Please choose a vertical coordinate:");
                        int y = AcquireValidIntegerWithin(0, 9);

                        field Field = computerBoard[y][x];
                        if(!Field.IsHit)
                        {
                            Field.IsHit = true;
                            validShot = true;
                            Console.Clear();
                            DrawTacticalDisplay(computerBoard);
                            if (Field._Ship != null)
                            {
                                Console.WriteLine("Hit confirmed! Enemy ship has taken damage!");
                                if (Field._Ship.Sunk)
                                {
                                    Console.WriteLine("An enemy vessel has been sunk!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Miss! No ship there!");
                            }
                            Console.WriteLine("Press Any Key to Continue");
                            Console.ReadKey();
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("You have already fired at this position! It is against orders to waste ammunition!");
                        }
                    }
                }

                // Pre-Computer's turn to see if Computer has lost.
                if (AllShipsSunk(computerShips))
                {
                    Console.Clear();
                    gameOver = true;
                    Console.WriteLine("You have sunk all of the Enemy's ships! You WIN!");
                }

                // Computer's turn to shoot!
                if (!gameOver)
                {
                    
                    bool validShot = false;
                    Coordinate nextShot = new Coordinate(0, 0);
                    while (!validShot)
                    {
                        
                        nextShot = computer.GetNextShot(playerBoard);
                        field Field = playerBoard[nextShot.Y][nextShot.X];
                        if(!Field.IsHit)
                        {
                            Console.WriteLine("Computer: I SHOOT AT " + nextShot.Y + ", " + nextShot.X);

                            validShot = true;
                            Field.IsHit = true;
                            if(Field._Ship != null)
                            {
                                Console.WriteLine("Computer has hit your " + Field._Ship.Type + "!");
                                computer.LastShipHitSunk = false;
                                computer.LastShipHitCoordinate = nextShot;
                                computer.Streak++;
                                if (Field._Ship.Sunk)
                                {
                                    Console.WriteLine("Computer has sunk your " + Field._Ship.Type + "!");
                                    computer.Streak = 0;
                                    computer.LastShipHitSunk = true;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Computer has missed!");
                            }
                            Console.WriteLine("Press Any Key to Continue");
                            Console.ReadKey();
                            Console.WriteLine();
                        }
                    }
                }
            }
            Console.WriteLine("This is your board:");
            DrawBoard(playerBoard);
            Console.WriteLine("\nThis is Computer's board:");
            DrawBoard(computerBoard);
            Console.ReadKey();
        }

        /// <summary>
        /// Checks if all ships in the list have been sunk.
        /// </summary>
        /// <param name="ships">The list of ships to be checked.</param>
        /// <returns>True if all ships in list are sunk.</returns>
        public static bool AllShipsSunk(List<ship> ships)
        {
            int sunkenShips = 0;
            foreach (ship Ship in ships)
            {
                if (Ship.Sunk)
                {
                    sunkenShips++;
                }
            }
            if (sunkenShips < ships.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Draw the "Tactical" Display of the enemy's board. 
        /// </summary>
        /// <param name="board">The board of the opposing Player.</param>
        public static void DrawTacticalDisplay(field[][] board)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Choose where to shoot!\ny\\x1 2 3 4 5 6 7 8 9¹0");

            for (int i = 0; i < board.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write((i == 9 ? "" : " ") + (i + 1));
                Console.ForegroundColor = ConsoleColor.White;


                for (int j = 0; j < board[i].Length; j++)
                {
                    field Field = board[i][j];
                    Console.ForegroundColor = ConsoleColor.White;
                    if((i+j) % 2 == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }

                    if(Field.IsHit)
                    {
                        if(Field._Ship == null)
                        {
                            // IF there is no ship, draw mundane hit.
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("()");
                        }
                        else
                        {
                            // If there is a hit ship, draw a Success hit.
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("{}");
                        }
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Gets user to input an integer inclusively between minValue and maxValue.
        /// </summary>
        /// <param name="minValue">The Minimum value of Integer you want.</param>
        /// <param name="maxValue">The Maximum value of Integer you want.</param>
        /// <returns>A user-specified Integer inclusively between minValue and maxValue</returns>
        public static int AcquireValidIntegerWithin(int minValue, int maxValue)
        {
            Console.WriteLine("Valid values are from " + (minValue+1) + " to " + (maxValue+1));
            bool validNumber = false;
            string input;
            int output = 0;
            while (!validNumber)
            {
                input = Console.ReadLine();
                if(int.TryParse(input, out output) && output >= minValue+1 && output <= maxValue+1)
                {
                    validNumber = true;
                }
                else
                {
                    Console.WriteLine(" INCORRECT VALUE ");
                }
            }
            return output-1;
        }
        
        /// <summary>
        /// Makes a 10x10 grid of fields which has their isHit set to false.
        /// </summary>
        /// <returns>10x10 field grid, isShip & isHit are both false.</returns>
        public static field[][] GenerateEmptyTenByTen()
        {
            field[][] newBoard = new field[10][];
            for (int i = 0; i < 10; i++)
            {
                newBoard[i] = new field[10];
                for (int j = 0; j < 10; j++)
                {
                    newBoard[i][j] = new field();
                }
            }
            return newBoard;
        }

        /// <summary>
        /// During GameSetup(), when placing down your ships, this function checks if another ship is placed on the X Y coordinates given, or anywhere along the length of the ship.
        /// </summary>
        /// <param name="board">The board the ship is going to be placed on.</param>
        /// <param name="Ship">The ship that is soon to be placed. Really just need it for its Length.</param>
        /// <param name="x">Horizontal Coordinate</param>
        /// <param name="y">Vertical Coordinate</param>
        /// <param name="horizontal">If the ship is to be placed horizontally, vertically if false</param>
        /// <returns>If it at any point encountered a ship in the fields of the new ship.</returns>
        public static bool ShipInTheWay(field[][] board, ship Ship, int x, int y, bool horizontal)
        {
            for (int i = 0; i < Ship.Length; i++)
            {
                // If the board already has a Ship in any of the coordinates in the path of the new ship, then return true that there is a Ship In The Way.
                if (horizontal && board[y][x + i]._Ship != null || !horizontal && board[y + i][x]._Ship != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draws a player's board with boats on it. Boats are marked with the first letter of their type typed twice, unless it has been hit in that square, in which case it's XX. Water (Ship-less Field) is marked with ▓▓ or ▒▒.
        /// </summary>
        /// <param name="board">The boat-filled board to be displayed.</param>
        public static void DrawBoard(field[][] board)
        {
            Random rnd = new Random();
            Console.WriteLine("\n######## Board ########");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("y\\x1 2 3 4 5 6 7 8 9¹0");
            for (int i = 0; i < board.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write((i == 9 ? "" : " ") + (i+1));
                
                for (int j = 0; j < board[i].Length; j++)
                {
                    // Alternating color!
                    if ((i + j) % 2 == 0)
                    { 
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }

                    field Field = board[i][j];
                    if (Field._Ship != null)
                    {
                        if (Field.IsHit)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.Write("XX");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write(Field._Ship.Type.Substring(0, 1) + Field._Ship.Type.Substring(0, 1));
                        }
                    }
                    else
                    {
                        switch(rnd.Next(0, 3))
                        {
                            case 0:
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.Write("▒▒");
                                break;
                            default:
                                Console.Write("▓▓");
                                break;
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
        }
    }
}
