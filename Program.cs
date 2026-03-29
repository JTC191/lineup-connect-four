using System.Threading;
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

            Console.WriteLine("Invalid input. Please try again.\n");
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

            Console.WriteLine();
            Console.WriteLine($"Invalid input. Please enter a number. Please enter a number greater than or equal to {minValue}.\n");

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

    public int DropDiscAndReturnRow(int column, char discSymbol)
    {
        int colIndex = column - 1;

        for (int row = Rows - 1; row >=0; row--)
        {
            if (cells[row,colIndex]==' ')
            {
                cells[row, colIndex] = discSymbol;
                return row;
            }
        }

        return -1;
    }

    public void Explode(int row, int column)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // new bounds check if Exploding Disc placement is valid
                int newRow = row + i;
                int newCol = column + j;
                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                {
                    if (cells[newRow, newCol] != ' ')
                    {
                        cells[newRow, newCol] = ' ';
                    }
                }
            }
        }
    }

    public void ApplyMagneticEffect(int row, int column, char player)
    {
        char ordinaryDisc;

        if (player == '@')
        {
            ordinaryDisc = '@';
        }
        else
        {
            ordinaryDisc = '#';
        }

        cells[row, column] = ordinaryDisc;

        if (row == 0)
        {
            return;
        }

        for (int r = row - 1; r >= 0; r--)
        {
            if (cells[r, column] == ordinaryDisc)
            {
                if (r != row - 1)
                {
                    cells[row - 1, column] = ordinaryDisc;
                    cells[r, column] = ' ';
                }
                break;
            }
        }
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
            Console.WriteLine($"\nTurn is {currentPlayer}\n");


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

        while (!validInput)
        {
            int columnNumber = GameMenu.PromptForNumber("Enter column number", 1);

            if (columnNumber < 1 || columnNumber > grid.Columns)
            {
                Console.WriteLine($"Invalid column. Try between 1 and {grid.Columns}");
                continue;
            }

            char discType = PromptForDiscType();
            char discSymbol = GetDiscSymbol(discType);

            int row = grid.DropDiscAndReturnRow(columnNumber, discSymbol);

            if (row == -1)
            {
                Console.WriteLine("The column is full. Try again.");
            }
            else
            {
                Console.Clear();
                grid.Display();

                if (discType == 'E')
                {
                    Thread.Sleep(1000);
                    grid.Explode(row, columnNumber - 1);

                    Console.Clear();
                    grid.Display();
                }
                else if (discType == 'M')
                {
                    Thread.Sleep(1000);
                    grid.ApplyMagneticEffect(row, columnNumber - 1, currentPlayer);

                    Console.Clear();
                    grid.Display();
                }

                validInput = true;
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
            Console.WriteLine("");

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

    public void RunTestMode(string moveSequence)
    {
        Console.WriteLine($"Testing sequence: {moveSequence}");

        string[] testingInput = moveSequence.Split(',');

        foreach (string rawInput in testingInput)
        {
            string inputItem = rawInput.Trim(); 

            if (inputItem.Length < 2)
            {
                Console.WriteLine($"Invalid input: {inputItem}");
                return;
            }

            char discType = char.ToUpper(inputItem[0]);

            if (discType != 'O' && discType != 'E' && discType != 'M')
            {
                Console.WriteLine("Only ordinary discs supported in test mode so far.");
                return;
            }

            // Accounts for if column number is more than 2 digits
            string columnText = inputItem.Substring(1);

            int columnNumber;
            if (!int.TryParse(columnText, out columnNumber))
            {
                Console.WriteLine($"Invalid column: {columnText}");
                return;
            }

            if (columnNumber < 1 || columnNumber > grid.Columns)
            {
                Console.WriteLine($"Column out of bounds: {columnNumber}");
                return;
            }

            int row = grid.DropDiscAndReturnRow(columnNumber, currentPlayer);

            if (row == -1)
            {
                Console.WriteLine($"Column {columnNumber} is full.");
                return;
            }

            Console.Clear();
            grid.Display();
            Console.WriteLine();

            if (grid.CheckWin(currentPlayer))
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
                return;
            }

            if (grid.CheckDraw())
            {
                Console.WriteLine("Game is a draw!");
                return;
            }

            SwitchPlayer();
        }
    }
    private char PromptForDiscType()
    {
        while (true)
        {
            Console.WriteLine("Choose disc type:");
            Console.WriteLine("O = Ordinary");
            Console.WriteLine("E = Exploding");
            Console.WriteLine("M = Magnetic");
            Console.WriteLine("B = Boring");
            Console.Write("> ");

            string input = Console.ReadLine().Trim().ToUpper();

            if (input == "O" || input == "E" || input == "M")
            {
                // Converts string -> char
                return input[0];
            }

            Console.WriteLine("Invalid disc type. Please enter O, E, or M.\n");
        }
    }
    private char GetDiscSymbol(char discType)
    {
        if (currentPlayer == '@')
        {
            switch (discType)
            {
                case 'O': return '@';
                case 'E': return 'E';
                case 'M': return 'M';
            }
        }
        else
        {
            switch (discType)
            {
                case 'O': return '#';
                case 'E': return 'e';
                case 'M': return 'm';
            }
        }

        throw new ArgumentException("Invalid disc type.");
    }
}
class Program
{
   static void Main()
   {
       Console.WriteLine("Welcome to LineUp!\n");

       int rows = GameMenu.PromptForNumber("Enter number of rows", 6);
       int columns = GameMenu.PromptForNumber("Enter number of columns", 7);

       int gameMode = GameMenu.PromptForMenuOption(
           "Choose game mode:\n1. Human vs Human\n2. Human vs Computer\n3. Testing Mode",
           1,
           3
       );

       Game game = new Game(rows, columns, gameMode);
       
       if (gameMode == 3)
       {
            Console.WriteLine("Enter testing sequence: ");
            string sequence = Console.ReadLine();
            game.RunTestMode(sequence);
       }
       else
       {
           game.Run();
       }
       
   }
}
