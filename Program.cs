﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class Program
    {
        private static Field[][] playerBoard;
        private static List<Ship> playerShips = new List<Ship>();
        private static Coordinate playerPreviousCoords = new Coordinate(0, 0);
        private static Field[][] computerBoard;
        private static List<Ship> computerShips = new List<Ship>();
        private static ArtificialIntelligence computer;
        private static Dictionary<int, string> horizontalValueToLetter = new Dictionary<int, string>();
        static void Main(string[] args)
        {
            horizontalValueToLetter.Add(0, "A");
            horizontalValueToLetter.Add(1, "B");
            horizontalValueToLetter.Add(2, "C");
            horizontalValueToLetter.Add(3, "D");
            horizontalValueToLetter.Add(4, "E");
            horizontalValueToLetter.Add(5, "F");
            horizontalValueToLetter.Add(6, "G");
            horizontalValueToLetter.Add(7, "H");
            horizontalValueToLetter.Add(8, "I");
            horizontalValueToLetter.Add(9, "J");
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
            playerPreviousCoords = new Coordinate(0, 0);
            playerBoard = GenerateEmptyTenByTen();
            playerShips.Clear();
            playerShips.Add(new Ship("Aircraft Carrier", 5));
            playerShips.Add(new Ship("Battleship", 4));
            playerShips.Add(new Ship("Destroyer", 3));
            playerShips.Add(new Ship("Uboat", 3));
            playerShips.Add(new Ship("Patrolship", 2));

            // Resetting the Computer.
            computer = new ArtificialIntelligence();
            computerBoard = GenerateEmptyTenByTen();
            computerShips.Clear();
            computerShips.Add(new Ship("Aircraft Carrier", 5));
            computerShips.Add(new Ship("Battleship", 4));
            computerShips.Add(new Ship("Destroyer", 3));
            computerShips.Add(new Ship("Uboat", 3));
            computerShips.Add(new Ship("Patrolship", 2));

            // Time for the player to put down their ships.
            foreach(Ship ship in playerShips)
            {
                Console.WriteLine("This is your board, place your ship!");
                DrawBoard(playerBoard);

                // First we want to know if the player wants to place the ship vertically or horizontally.
                Console.WriteLine("\nYou are about to place down your " + ship.Type + ", it is " + ship.Length + " fields long.\nh. Horizontal | v. Vertical");
                bool horizontal = true;

                bool validSelection = false;
                char input;

                // While NOT validSelection of Horizontal or Vertical, keep asking.
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
                
                // While NOT a valid position.
                // A Valid Position is one where:
                // 1. The coordinates are within the board.
                // 2. THe ship will not stick out of the board.
                // 3. There are currently no other ships placed in the way of the new Ship.
                while(!validPosition)
                {

                    // Acquiring coordinates. X = Horizontal, Y = Vertical
                    // You might notice this: "9 - (horizontal ? Ship.Length : 1)" and the opposite for the Vertical (y) coordiante below.
                    // It means 9 minus either ship length or 0, depending on if it's horizontal or not.
                    // This way, the ship cannot stick outside of the board. So a horizontal carrier (length 5) can at most be placed at horizontal(x) coordinate 5. So it will be placed on 5, 6, 7, 8, 9 (5 total squares)

                    Coordinate coordinate = ManualCoordinates(ship, horizontal);


                    // These positions have Room for these ships, but-!
                    // .. We need to check if any other ships are blocking it
                    if (ShipInTheWay(playerBoard, ship, coordinate, horizontal))
                    {
                        Console.WriteLine("A ship is already placed there!");
                    }
                    else
                    {
                        // Since no ship is blocking it, we're going to place it down on the board!
                        for (int i = 0; i < ship.Length; i++)
                        {
                            if (horizontal)
                            {
                                playerBoard[coordinate.Y][coordinate.X + i]._Ship = ship;
                            }
                            else
                            {
                                playerBoard[coordinate.Y + i][coordinate.X]._Ship = ship;
                            }
                        }
                        validPosition = true;
                        Console.WriteLine(ship.Type + " successfully placed at " + (coordinate.Y+1) + ", " + horizontalValueToLetter[coordinate.X]);
                    }
                }
                Console.Clear();
            }

            // Computer ship placements
            Random rnd = new Random();
            // The computer is dumb, and will need to make randomized choices.

            foreach (Ship ship in computerShips)
            {
                bool validPosition = false;
                // While the computer does Not have a valid position for its ship..
                while (!validPosition)
                {
                    // Horizontal or not?
                    bool horizontal = rnd.Next(0, 2) == 1;

                    // If it is horizontal, than the x-value is adjusted to the length of the ship, thereby preventing the ship being placed out of bounds.
                    // If it is not hortizontal, then the y-value (Vertical coordinate) is adjusted to the length of the ship, -||-
                    
                    Coordinate coordinate = new Coordinate(
                        rnd.Next(0, 10 - (!horizontal ? ship.Length : 0)),
                        rnd.Next(0, 10 - (horizontal ? ship.Length : 0))
                        );

                    // If there is no ship already on this board, along the length of the ship we're about to place, at x and y coordinate, horizontally or not...
                    if (!ShipInTheWay(computerBoard, ship, coordinate, horizontal))
                    {
                        // Then Place the ship on those Fields!
                        for (int i = 0; i < ship.Length; i++)
                        {
                            if (horizontal)
                            {
                                computerBoard[coordinate.Y][coordinate.X + i]._Ship = ship;
                            }
                            else
                            {
                                computerBoard[coordinate.Y + i][coordinate.X]._Ship = ship;
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
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("All of your ships have sunk! You LOSE!");
                    Console.ResetColor();
                }

                if (!gameOver)
                {
                    round++;
                    Console.WriteLine("####### R O U N D   " + round + " #######");

                    DrawBoard(playerBoard);

                    Console.WriteLine("\nYour ships and how they are doing:");
                    foreach (Ship ship in playerShips)
                    {
                        Console.WriteLine(ship.Type + " HP: " + (ship.Length - ship.Hits) + "/" + ship.Length);
                    }
                    Console.WriteLine("Press Any Key to Continue to Shooting");
                    Console.ReadKey();
                    Console.WriteLine();

                    // Player gets first shot!
                    bool validShot = false;
                    while (!validShot)
                    {
                        Console.Clear();

                        Console.WriteLine("Your turn to shoot!");
                        DrawTacticalDisplay(computerBoard);
                        Coordinate coordinate = ManualCoordinates();

                        Field field = computerBoard[coordinate.Y][coordinate.X];
                        if(!field.IsHit)
                        {
                            field.IsHit = true;
                            validShot = true;
                            Console.Clear();
                            DrawTacticalDisplay(computerBoard);
                            if (field._Ship != null)
                            {
                                Console.WriteLine("Hit confirmed! Enemy ship has taken damage!");
                                if (field._Ship.Sunk)
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
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("You have already fired at that position! It is against orders to waste ammunition!");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                    }
                }

                // Post-Player's turn, let's see if Computer has lost.
                if (AllShipsSunk(computerShips))
                {
                    Console.Clear();
                    gameOver = true;
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("You have sunk all of the Enemy's ships! You WIN!");
                    Console.ResetColor();
                }

                // Computer's turn to shoot!
                if (!gameOver)
                {
                    bool validShot = false;
                    Coordinate nextShot;
                    while (!validShot)
                    {
                        nextShot = computer.GetNextShot(playerBoard);
                        Field field = playerBoard[nextShot.Y][nextShot.X];
                        if(!field.IsHit)
                        {
                            Console.WriteLine("Computer: I SHOOT AT " + horizontalValueToLetter[nextShot.X] + ", " + (nextShot.Y + 1));

                            validShot = true;
                            field.IsHit = true;
                            if(field._Ship != null)
                            {
                                Console.WriteLine("Computer has hit your " + field._Ship.Type + "!");

                                if (!computer.LastShipHitSunk && computer.Direction.X == 0 && computer.Direction.Y == 0)
                                {
                                    // The Computer is continueing on the ship it's trying to sink, and therefore knows the direction!
                                    computer.Direction = new Coordinate(nextShot.Y - computer.LastShipHitCoordinate.Y, nextShot.X - computer.LastShipHitCoordinate.X);
                                }

                                computer.LastShipHitSunk = false;
                                computer.LastShipHitCoordinate = nextShot;
                                computer.Streak++;
                                if (field._Ship.Sunk)
                                {
                                    Console.WriteLine("Computer has sunk your " + field._Ship.Type + "!");
                                    computer.ResetTargetting();
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
            Console.WriteLine("This is yours:");
            DrawBoard(playerBoard);
            Console.WriteLine("This is Computer's:");
            DrawBoard(computerBoard);
            Console.ReadKey();
        }

        /// <summary>
        /// Checks if all ships in the list have been sunk.
        /// </summary>
        /// <param name="ships">The list of ships to be checked.</param>
        /// <returns>True if all ships in list are sunk.</returns>
        public static bool AllShipsSunk(List<Ship> ships)
        {
            int sunkenShips = 0;
            foreach (Ship ship in ships)
            {
                if (ship.Sunk)
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
        public static void DrawTacticalDisplay(Field[][] board)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("y\\xA B C D E F G H I J");

            for (int i = 0; i < board.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write((i == 9 ? "" : " ") + (i + 1));
                Console.ForegroundColor = ConsoleColor.White;


                for (int j = 0; j < board[i].Length; j++)
                {
                    Field field = board[i][j];
                    Console.ForegroundColor = ConsoleColor.White;
                    if((i+j) % 2 == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }

                    if(field.IsHit)
                    {
                        if(field._Ship == null)
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
                        // Unhit square. Might contain a ship. Might not.
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Allows user to manually input letters and numbers, corresponding to a coordinate position on the board.
        /// </summary>
        /// <param name="ship">Ship to be placed, if any.</param>
        /// <param name="horizontal">Ship to be placed Horizontally or Vertically, if any ship.</param>
        /// <returns>The coordinate of the final selection.</returns>
        public static Coordinate ManualCoordinates(Ship ship = null, bool horizontal = true)
        {
            Coordinate coor = new Coordinate(0, 0);
            int horizontalMax = 9 - (ship != null && horizontal ? ship.Length-1 : 0);
            int verticalMax = 9 - (ship != null && !horizontal ? ship.Length-1 : 0);

            Console.WriteLine("\nPlease choose a horizontal letter from A to " + horizontalValueToLetter[horizontalMax]);
            string letterInput = "";
            bool validHorizontalCoord = false;
            while (!validHorizontalCoord)
            {
                letterInput = Console.ReadKey().Key.ToString().ToUpper();

                for (int i = 0; i <= horizontalMax; i++)
                {
                    if(letterInput == horizontalValueToLetter[i])
                    {
                        coor.X = i;
                        validHorizontalCoord = true;
                    }
                }
            }
            Console.WriteLine("\nPlease choose a vertical position");
            coor.Y = AcquireValidIntegerWithin(0, verticalMax);

            return coor;
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
        public static Field[][] GenerateEmptyTenByTen()
        {
            Field[][] newBoard = new Field[10][];
            for (int i = 0; i < 10; i++)
            {
                newBoard[i] = new Field[10];
                for (int j = 0; j < 10; j++)
                {
                    newBoard[i][j] = new Field();
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
        /// <returns>If it at any point encountered a ship in the fields of the new Ship.</returns>
        public static bool ShipInTheWay(Field[][] board, Ship ship, Coordinate coord, bool horizontal)
        {
            for (int i = 0; i < ship.Length; i++)
            {
                // If the board already has a Ship in any of the coordinates in the path of the new Ship, then return true that there is a Ship In The Way.
                if (horizontal && board[coord.Y][coord.X + i]._Ship != null || !horizontal && board[coord.Y + i][coord.X]._Ship != null)
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
        public static void DrawBoard(Field[][] board)
        {
            Random rnd = new Random();
            Console.WriteLine("######## Board ########");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("y\\xA B C D E F G H I J");
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

                    Field field = board[i][j];
                    if (field._Ship != null)
                    {
                        if (field.IsHit)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.Write("XX");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write(field._Ship.Type.Substring(0, 1) + field._Ship.Type.Substring(0, 1));
                        }
                    }
                    else if (field.IsHit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        switch (rnd.Next(0, 3))
                        {
                            case 0:
                                
                                Console.Write("▒▒");
                                break;
                            default:
                                Console.Write("▓▓");
                                break;
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
