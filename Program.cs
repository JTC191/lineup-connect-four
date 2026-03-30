using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
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

    public bool IsColumnFull(int column)
    {
        int colIndex = column - 1;
        return cells[0, colIndex] != ' ';
    }

    public void UndoMove(int row, int column)
    {
        cells[row, column] = ' ';
    }

    public string[] GetGridLines()
    {
        string[] lines = new string[Rows];

        for (int row = 0; row < Rows; row++)
        {
            char[] rowChars = new char[Columns];

            for (int col = 0; col < Columns; col++)
            {
                rowChars[col] = cells[row, col];
            }
            lines[row] = new string(rowChars);
        }

        return lines;
    }

    public void LoadGridLines(string[] lines)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                cells[row, col] = lines[row][col];
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

    private int player1ExplodingUsed = 0;
    private int player1MagneticUsed = 0;
    private int player2ExplodingUsed = 0;
    private int player2MagneticUsed = 0;

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

            if (gameMode == 2 && currentPlayer == '#')
            {
                MakeComputerMove();
            }

            else
            {
                HandleTurn(); 
            }

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
                if (discType == 'E')
                {
                    grid.Explode(row, columnNumber - 1);
                }
                else if (discType == 'M')
                {
                    grid.ApplyMagneticEffect(row, columnNumber - 1, currentPlayer);
                }

                Console.Clear();
                grid.Display();

                if (discType == 'E')
                {
                    if (currentPlayer == '@')
                        player1ExplodingUsed++;
                    else
                        player2ExplodingUsed++;
                }
                else if (discType == 'M')
                {
                    if (currentPlayer == '@')
                        player1MagneticUsed++;
                    else
                        player2MagneticUsed++;
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
                SaveGame();
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

            if (discType == 'E')
            {
                if (currentPlayer == '@' && player1ExplodingUsed >= 2)
                {
                    Console.WriteLine("Player @ has no exploding discs left.");
                    return;
                }
                if (currentPlayer == '#' && player2ExplodingUsed >= 2)
                {
                    Console.WriteLine("Player # has no exploding discs left.");
                    return;
                }
            }
            else if (discType == 'M')
            {
                if (currentPlayer == '@' && player1MagneticUsed >= 2)
                {
                    Console.WriteLine("Player @ has no magnetic discs left.");
                    return;
                }
                if (currentPlayer == '#' && player2MagneticUsed >= 2)
                {
                    Console.WriteLine("Player # has no magnetic discs left.");
                    return;
                }
            }

            if (discType != 'O' && discType != 'E' && discType != 'M')
            {
                Console.WriteLine("Only ordinary/exploding/magnetic discs supported in test mode so far.");
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

            char discSymbol = GetDiscSymbol(discType);
            int row = grid.DropDiscAndReturnRow(columnNumber, discSymbol);

            if (row == -1)
            {
                Console.WriteLine($"Column {columnNumber} is full.");
                return;
            }
            if (discType == 'E')
            {
                grid.Explode(row, columnNumber - 1);
            }
            else if (discType == 'M')
            {
                grid.ApplyMagneticEffect(row, columnNumber - 1, currentPlayer);
            }
            if (discType == 'E')
            {
                if (currentPlayer == '@')
                    player1ExplodingUsed++;
                else
                    player2ExplodingUsed++;
            }
            else if (discType == 'M')
            {
                if (currentPlayer == '@')
                    player1MagneticUsed++;
                else
                    player2MagneticUsed++;
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
            Console.Write("> ");

            string input = Console.ReadLine().Trim().ToUpper();

            if (input == "O")
            {
                return 'O';
            }

            if (input == "E")
            {
                if (currentPlayer == '@' && player1ExplodingUsed < 2)
                    return 'E';
                if (currentPlayer == '#' && player2ExplodingUsed < 2)
                    return 'E';

                Console.WriteLine("No exploding discs left.\n");
                continue;
            }

            if (input == "M")
            {
                if (currentPlayer == '@' && player1MagneticUsed < 2)
                    return 'M';
                if (currentPlayer == '#' && player2MagneticUsed < 2)
                    return 'M';

                Console.WriteLine("No magnetic discs left.\n");
                continue;
            }

            Console.WriteLine("Invalid disc type.\n");
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

    private List<int> GetValidColumns()
    {
        List<int> validColumns = new List<int>();

        for (int col = 1; col <= grid.Columns; col++)
        {
            if (!grid.IsColumnFull(col))
            {
                validColumns.Add(col);
            }
        }

        return validColumns;
    }

    private void MakeComputerMove()
    {
        List<int> validColumns = GetValidColumns();

        int chosenColumn = -1;

        foreach (int col in validColumns)
        {
            int row = grid.DropDiscAndReturnRow(col, currentPlayer);

            if (grid.CheckWin(currentPlayer))
            {
                chosenColumn = col;
                grid.UndoMove(row, col - 1); 
                break;
            }

            grid.UndoMove(row, col - 1); 
        }

        if (chosenColumn == -1)
        {
            Random rand = new Random();
            chosenColumn = validColumns[rand.Next(validColumns.Count)];
        }

        int finalRow = grid.DropDiscAndReturnRow(chosenColumn, currentPlayer);

        Console.WriteLine($"Computer plays column {chosenColumn}");
        Thread.Sleep(1000);

        Console.Clear();
        grid.Display();
    }

    private void SaveGame()
    {
        List<string> lines = new List<string>();
        lines.Add(player1ExplodingUsed.ToString());
        lines.Add(player1MagneticUsed.ToString());
        lines.Add(player2ExplodingUsed.ToString());
        lines.Add(player2MagneticUsed.ToString());

        lines.Add(grid.Rows.ToString());
        lines.Add(grid.Columns.ToString());
        lines.Add(gameMode.ToString());
        lines.Add(currentPlayer.ToString());

        lines.AddRange(grid.GetGridLines());

        File.WriteAllLines("savegame.txt", lines);

        Console.WriteLine("Game saved.");
    }

    public static Game LoadGame(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);

        int p1E = int.Parse(lines[0]);
        int p1M = int.Parse(lines[1]);
        int p2E = int.Parse(lines[2]);
        int p2M = int.Parse(lines[3]);

        int rows = int.Parse(lines[4]);
        int columns = int.Parse(lines[5]);
        int gameMode = int.Parse(lines[6]);
        char currentPlayer = lines[7][0];


        Game loadedGame = new Game(rows, columns, gameMode);
        loadedGame.currentPlayer = currentPlayer;

        loadedGame.player1ExplodingUsed = p1E;
        loadedGame.player1MagneticUsed = p1M;
        loadedGame.player2ExplodingUsed = p2E;
        loadedGame.player2MagneticUsed = p2M;

        string[] gridLines = new string[rows];
        for (int i = 0; i < rows; i++)
        {
            gridLines[i] = lines[i + 8];
        }

        loadedGame.grid.LoadGridLines(gridLines);

        return loadedGame;
    }
}
class Program
{
   static void Main()
   {
        Console.WriteLine("Welcome to LineUp!\n");

        int startOption = GameMenu.PromptForMenuOption(
            "1. New Game\n2. Load Game",
            1,
            2
        );

        Game game;

        if (startOption == 2)
        {
            game = Game.LoadGame("savegame.txt");
        }
        else
        {
            int rows = GameMenu.PromptForNumber("Enter number of rows", 6);
            int columns = GameMenu.PromptForNumber("Enter number of columns", 7);

            int gameMode = GameMenu.PromptForMenuOption(
                "Choose game mode:\n1. Human vs Human\n2. Human vs Computer\n3. Testing Mode",
                1,
                3
            );
            
            game = new Game(rows, columns, gameMode);
        }

        game.Run();
    }
}
