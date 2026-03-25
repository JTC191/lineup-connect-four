using System.ComponentModel.DataAnnotations.Schema;

class GameMenu
{
    public static int PromptForMenuOption(string userRequest, int minOption, int maxOption)
    {
        int menuOption;

        while (true)
        {
            Console.WriteLine(userRequest);
            Console.WriteLine();
            Console.Write("> ");
           
            // Ensure the user enters an integer within the allowed menu range
            if (int.TryParse(Console.ReadLine(), out menuOption) && 
                menuOption >= minOption && menuOption <= maxOption)
            {
                Console.WriteLine();
                return menuOption;
            }

            Console.WriteLine("Invalid input. Please try again.");
            Console.WriteLine();
        }
    }

    // Ensure the user enters an integer greater than or equal to the required minimum.
    public static int PromptForNumber(string userRequest, int minValue) 
    {
        int value;

        while (true)
        {
            Console.WriteLine(userRequest);
            Console.Write("> ");

            if (int.TryParse(Console.ReadLine(), out value) && value >= minValue)
            {
                Console.WriteLine();
                return value;
            }

            Console.WriteLine($"Invalid input. Please enter a number. Please enter a number greater than or equal to {minValue}.");
            Console.WriteLine();
        }
    }
}

class Grid
{
    private char[,] cells; 
    public int Rows { get; }
    public int Columns { get; }

    private int WinCondition;

    public Grid(int rows, int cols) // Validate grid dimensions when constructing a new grid.
    {
        if (rows < 6)
            throw new ArgumentException("Rows must be at least 6.");

        if (cols < 7)
            throw new ArgumentException("Columns must be at least 7.");

        if (rows > cols)
            throw new ArgumentException("Grid cannot have more rows than columns.");
        
        Rows = rows;
        Columns = cols;

        WinCondition = (int)Math.Floor(Rows * Columns * 0.1); 

        cells = new char[Rows, Columns];

        for (int i =0; i < Rows; i++)
        {
            for (int j=0; j < Columns; j++)
            {
                cells[i, j] = ' ';
            }
        }
    }
    public void Display()
    {
        // Prints the grid row-by-row with vertical borders for each cell
        for (int i =0; i<cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                Console.Write($"| {cells[i, j]} ");
            }
            Console.WriteLine("|");
        }
    }
    public bool DropDisc(int column, char discSymbol)
    {
        // Convert the user's 1-based column input to a 0-based array index.
        int colIndex = column - 1; 

        // Start from the bottom row to simulate gravity.
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (cells[row , colIndex] == ' ')
            {
                cells[row, colIndex] = discSymbol;
                return true;
            }
        }
        return false;
    }
    // Return true only when there are no empty cells left in the grid
    public bool CheckDraw()
    {
        for (int row = Rows - 1; row >=0; row--)
        {
            for (int col = 0; col <= Columns; col ++)
            {
                if (cells[row, col] == ' ')
                {
                    return false;
                }

            }   
        }
        return true;
    }

    public bool CheckWin(char disc)
    {
        // Check each row for a horizontal sequence of matching discs
        for (int row = 0; row < Rows; row++)
        {
            // Only check starting columns where a full sequence fits.
            for (int col = 0; col <= Columns - WinCondition; col++)
            {
                bool match = true; // Assume a winning sequence unless proven otherwise

                for (int i = 0; i < WinCondition; i++)
                {
                    if (cells[row, col + i] != disc)
                    {
                        match = false; // One mismatch means starting position is not a win.
                        break;
                    }
                }
                if (match)
                {
                    return true;
                }
            }
        }
        // Check each column for a vertical winning sequence
        for (int col = 0; col < Columns; col++)
        {
            // Only check starting rows where a full sequence can fit within the grid
            for (int row = 0; row <= Rows - WinCondition; row++)
            {
                bool match = true;

                for (int i = 0; i < WinCondition; i++)
                {
                    if (cells[row + i, col] != disc)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return true;
                }
            }
        }
        
        // Check diagonal down-right (only valid starting positions where a full sequence fits)
        for (int row = 0; row <= Rows - WinCondition; row++) 
        {
            for (int col = 0; col <= Columns - WinCondition; col++) 
            {
                bool match = true;

                for (int i = 0; i < WinCondition; i++)
                {
                    if (cells[row + i, col+i] != disc)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }
        }

        // Check diagonal up-right (only valid starting positions where a full sequence fits)
        for (int row = WinCondition - 1; row < Rows; row++) 
        {
            for (int col = 0; col <= Columns - WinCondition; col++) 
            {
                bool match = true;

                for (int i = 0; i < WinCondition; i++)
                {
                    if (cells[row - i, col + i] != disc)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }
        }

        return false;        
    }
}

class Program
{
    static void Main()

    {
        int mainChoice = GameMenu.PromptForMenuOption("Press 1. Load Game\nPress 2. New Game", 1, 2);

        if (mainChoice == 1)
        {
            Console.WriteLine("Load game not implemented yet.");
        }
        else
        {
            bool gameOver = false;
            int gameMode = GameMenu.PromptForMenuOption("Press 1. Human vs Human\nPress 2. Human vs Computer", 1, 2);

            Grid grid = null;

            // Keep asking for dimensions until a valid grid can be created.
            while (grid == null)
            {
                int rows = GameMenu.PromptForNumber("Enter number of rows (minimum 6)", 6);
                int columns = GameMenu.PromptForNumber("Enter number of columns (minimum 7)", 7);

                try
                {
                    grid = new Grid(rows, columns);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }
            }

            char tracker = '@'; 

            // Main game loop: display board, process player input, update state, swtich turns
            while (!gameOver)
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine($"Turn is {tracker}");

                bool validInput = false;

                // Keep asking until the player chooses a valid, non-full column.
                while (!validInput)
                {
                    int columnNumber = GameMenu.PromptForNumber("Enter column number", 1);
                    
                    if (columnNumber > grid.Columns)
                    {
                        Console.WriteLine($"Invalid column. Try between 1 and {grid.Columns}");
                        continue;
                    }

                    validInput = grid.DropDisc(columnNumber, tracker);

                    if (!validInput)
                    {
                        Console.WriteLine("The column is full. Try again.");
                    }
                }

                if (grid.CheckWin(tracker))
                {
                    gameOver = true;
                    Console.Clear();
                    grid.Display();
                    if (tracker == '@')
                    {
                       Console.WriteLine($"Player 1 wins!");
                    }
                    else
                    {
                        Console.WriteLine($"Player 2 wins!");
                    }
                   
                }

                else if (grid.CheckDraw())
                {
                    gameOver = true;
                    Console.WriteLine("Game is a draw");
                }

                else
                {
                    if (tracker == '@') { tracker = '#'; }
                    else { tracker = '@'; }
                }                     
            }
        }
    }
}
