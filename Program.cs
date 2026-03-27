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
            for (int col = 0; col < Columns; col ++)
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

class Game
{
    private Grid grid;
    private char currentPlayer;
    private int gameMode;
    private bool gameOver;

    public Game(int rows, int columns, int selectedGameMode)
    {
        grid = new Grid(rows, columns);
        currentPlayer = '@';
        gameMode = selectedGameMode;
        gameOver = false;
    }

    public void Run()
    {

        // Main game loop runs until a win or draw condition is met
        while (!gameOver)
        {
            Console.Clear();
            grid.Display();
            Console.WriteLine($"Turn is {currentPlayer}");

            HandleTurn();

            if (grid.CheckWin(currentPlayer))
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine($"Player {currentPlayer} wins!");
                gameOver = true;
            }
            else if (grid.CheckDraw())
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine("Game is a draw!");
                gameOver = true;
            }
            else
            {
                SwitchPlayer();
            }
        }
    }

    private void SwitchPlayer()
    {
        if (currentPlayer == '@')
        {
            currentPlayer = '#';
        }
        else
        {
            currentPlayer = '@';
        }
    }

    private void MakeMove()
    {
        bool validInput = false;

        // Keep prompting until the player chooses a valid, non-full column.
        while (!validInput)
        {
            int columnNumber = GameMenu.PromptForNumber("Enter column number", 1);
                
            // Check column is within bounds
            if (columnNumber > grid.Columns)
            {
                Console.WriteLine($"Invalid column. Try between 1 and {grid.Columns}");
                continue;
            }

            validInput = grid.DropDisc(columnNumber, currentPlayer);

            if (!validInput)
            {
                Console.WriteLine("The column is full. Try again.");
            }
        }
        
    }

    //Handles one player's turn until a valid move is made
    private void HandleTurn()
    {
        bool turnComplete = false; 
        
        while (!turnComplete)
        {
            int userSelection = GameMenu.PromptForMenuOption("1. Make a move\n2. Save game\n3. Help", 1, 3);
            if (userSelection == 1)
            {
                MakeMove();
                turnComplete = true;
            }

            else if (userSelection == 2)
            {
                Console.WriteLine("Save feature not yet implemented");
            }

            else
            {
                Console.WriteLine("choose 1 to play a disc\nchoose 2 to save the current game\nchoose 3 to view help\nconnect enough discs in a line to win");
                Console.WriteLine("");
            }
        }
    }
}
class Program
{
   static void Main()
    {
        Console.WriteLine("Welcome to LineUp!");

        int rows = GameMenu.PromptForNumber("Enter number of rows", 4);
        int columns = GameMenu.PromptForNumber("Enter number of columns", 4);

        int gameMode = GameMenu.PromptForMenuOption(
            "Choose game mode:\n1. Human vs Human\n2. Human vs Computer",
            1,
            2
        );

        Game game = new Game(rows, columns, gameMode);
        game.Run();
    }
}
