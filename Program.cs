using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadsides
{
    class Program
    {
        public static field[][] playerBoard;
        public static field[][] playerShots;
        public static List<ship> playerShips = new List<ship>();
        public static field[][] computerBoard;
        public static field[][] computerShots;
        public static List<ship> computerShips = new List<ship>();
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

        public static void GameSetup()
        {
            Console.Clear();
            // Resetting the Player.
            playerBoard = GenerateEmptyTenByTen();
            playerShots = GenerateEmptyTenByTen();
            playerShips.Clear();
            playerShips.Add(new ship("Aircraft Carrier", 5));
            playerShips.Add(new ship("Battleship", 4));
            playerShips.Add(new ship("Destroyer", 3));
            playerShips.Add(new ship("Uboat", 3));
            playerShips.Add(new ship("Patrolship", 2));

            // Resetting the Computer.
            computerBoard = GenerateEmptyTenByTen();
            computerShots = GenerateEmptyTenByTen();
            computerShips.Clear();
            computerShips.Add(new ship("Aircraft Carrier", 5));
            computerShips.Add(new ship("Battleship", 4));
            computerShips.Add(new ship("Destroyer", 3));
            computerShips.Add(new ship("Uboat", 3));
            computerShips.Add(new ship("Patrolship", 2));

            foreach(ship Ship in playerShips)
            {
                Console.WriteLine("This is your board, place your ship!");
                drawActorsOwnBoard(playerBoard);


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
                    Console.WriteLine("\nPlease choose a valid Horizontal position:");
                    int x = acquireValidIntegerWithin(0, 9 - (horizontal ? Ship.Length-1 : 0));

                    Console.WriteLine("\nPlease choose a valid Vertical position:");
                    int y = acquireValidIntegerWithin(0, 9 - (!horizontal ? Ship.Length-1 : 0));
                    // These positions have Room for these ships, but-!
                    // .. We need to check if any other ships are blocking it
                    bool shipInTheWay = false;
                    for (int i = 0; i < Ship.Length; i++)
                    {
                        if(horizontal && playerBoard[y][x + i].IsShip || !horizontal && playerBoard[y + i][x].IsShip)
                        {
                            shipInTheWay = true;
                            break;
                        }
                    }
                    if(shipInTheWay)
                    {
                        Console.WriteLine("A ship is already placed there!");
                    }
                    else
                    {
                        for (int i = 0; i < Ship.Length; i++)
                        {
                            if (horizontal)
                            {
                                playerBoard[y][x + i].IsShip = true;
                                playerBoard[y][x + i]._Ship = Ship;
                            }
                            else
                            {
                                playerBoard[y + i][x].IsShip = true;
                                playerBoard[y + i][x]._Ship = Ship;
                            }
                        }
                        validPosition = true;
                        Console.WriteLine(Ship.Type + " successfully placed at " + (y+1) + ", " + (x+1));
                    }
                }
            }
            Console.Clear();
            Console.WriteLine("You have placed all your ships like so:");
            drawActorsOwnBoard(playerBoard);
            Console.ReadKey();
        }

        /// <summary>
        /// Gets user to input an integer inclusively between minValue and maxValue.
        /// </summary>
        /// <param name="minValue">The Minimum value of Integer you want.</param>
        /// <param name="maxValue">The Maximum value of Integer you want.</param>
        /// <returns>A user-specified Integer inclusively between minValue and maxValue</returns>
        public static int acquireValidIntegerWithin(int minValue, int maxValue)
        {
            Console.WriteLine("\nPlease write a number between " + (minValue+1) + " and " + (maxValue+1));
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
        /// Makes a 10x10 grid of fields which have their isShip and isHit set to false.
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

        public static void drawActorsOwnBoard(field[][] board)
        {
            Console.WriteLine("\n######## Board ########");
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    field Field = board[i][j];
                    if (Field.IsShip)
                    {
                        if (Field.IsHit)
                        {
                            Console.Write("X");
                        }
                        else
                        {
                            Console.Write(Field._Ship.Type.Substring(0, 1));

                        }
                    }
                    else
                    {
                        Console.Write("~");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
