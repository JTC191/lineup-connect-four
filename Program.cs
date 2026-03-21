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
           
            // Validate that the input is an integer within the allowed menu range.
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

    // Validate that the input is an integer greater than or equal to the required minimum.
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

    public Grid(int rows, int cols) // Enforce valid grid dimensions when a Grid is created
    {
        if (rows < 6)
            throw new ArgumentException("Rows must be at least 6.");

        if (cols < 7)
            throw new ArgumentException("Columns must be at least 7.");

        if (rows > cols)
            throw new ArgumentException("Grid cannot have more rows than columns.");
        
        Rows = rows;
        Columns = cols;

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
        // Displays the grid based on row and column dimensions
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
        int colIndex = column - 1;

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
            int gameMode = GameMenu.PromptForMenuOption("Press 1. Human vs Human\nPress 2. Human vs Computer", 1, 2);

            Grid grid = null;

            // Keep prompting until valid dimensions are entered.
            while (grid == null)
            {
                int rows = GameMenu.PromptForNumber("Enter number of rows (minimum 6)", 6);
                int columns = GameMenu.PromptForNumber("Enter number of columns (minimum 7)", 7);

                try
                {
                    grid = new Grid(rows, columns);
                    grid.DropDisc(1, '@');
                    grid.Display(); 
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }
            }
        }
    }
}
