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
           
            if (int.TryParse(Console.ReadLine(), out menuOption) && 
                menuOption >= minOption && menuOption <= maxOption)
            {
                Console.WriteLine();
                return menuOption;
            }

            Console.WriteLine("Invalid input. Please try again.\n");
        }
    }

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

    private readonly int WinCondition;

    public Grid(int rows, int cols) 
    {
        // Enforce assignment grid constraints
        if (rows < 6)
            throw new ArgumentException("Rows must be at least 6.");

        if (cols < 7)
            throw new ArgumentException("Columns must be at least 7.");

        if (rows > cols)
            throw new ArgumentException("Grid cannot have more rows than columns.");
        
        Rows = rows;
        Columns = cols;

        // Win length scales with grid size
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
        for (int row = 0; row < Rows; row++)
        {
            // Convert array index to display row number (bottom row = 1)
            int displayedRowNumber = Rows - row;
            Console.Write($"{displayedRowNumber}".PadLeft(2) + " ");

            for (int col = 0; col < Columns; col++)
            {
                Console.Write($"| {cells[row, col]} ");
            }

            Console.WriteLine("|");
        }

        Console.Write("   ");
        for (int col = 1; col <= Columns; col++)
        {
            Console.Write($"  {col} ");
        }
        Console.WriteLine();
    }

    // Return true only when there are no empty cells left in the grid
    public bool CheckDraw()
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < Columns; col++)
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
                bool match = true; 

                for (int i = 0; i < WinCondition; i++)
                {
                    if (cells[row, col + i] != disc)
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
         
        // Search from the bottom row upward and place the disc in the lowest available cell in the column
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

    // After explosions, compact each column so remaining discs fall downward
    public void ApplyGravity()
    {
        for (int col = 0; col < Columns; col++)
        {
            int writeRow = Rows - 1;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (cells[row, col] != ' ')
                {
                    cells[writeRow, col] = cells[row, col];

                    if (writeRow != row)
                    {
                        cells[row, col] = ' ';
                    }

                    writeRow--;
                }
            }

            for (int row = writeRow; row >= 0; row--)
            {
                cells[row, col] = ' ';
            }
        }
    }

    public void ClearExplosionArea(int row, int column)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newRow = row + i;
                int newCol = column + j;

                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                {
                    cells[newRow, newCol] = ' ';
                }
            }
        }
    }

    // Magnetic discs become ordinary discs after landing and may pull the nearest
    // matching ordinary disc one row upward
    public void ApplyMagneticEffect(int row, int column, char player)
    {
        char ordinaryDisc = player == '@' ? '@' : '#';

        cells[row, column] = ordinaryDisc;

        if (row == Rows - 1)
        {
            return;
        }

        for (int r = row + 1; r < Rows; r++)
        {
            if (cells[r, column] == ordinaryDisc)
            {
                if (r == row + 1)
                {
                    return;
                }

                char temp = cells[r - 1, column];
                cells[r - 1, column] = ordinaryDisc;
                cells[r, column] = temp;

                return;
            }
        }
    }

    public bool IsColumnFull(int column)
    {
        int colIndex = column - 1;
        return cells[0, colIndex] != ' ';
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

// Player subclasses define how a turn is taken in different game modes
abstract class Player
{
    public char Symbol { get; protected set; }

    protected Player(char symbol)
    {
        Symbol = symbol;
    }

    public abstract void TakeTurn(Game game);
}

class HumanPlayer : Player
{
    public HumanPlayer(char symbol) : base(symbol) { }

    public override void TakeTurn(Game game)
    {
        game.HandleTurn();
    }
}

class ComputerPlayer : Player
{
    public ComputerPlayer(char symbol) : base(symbol) { }

    public override void TakeTurn(Game game)
    {
        game.MakeComputerMove();
    }
}
class Game
{
    private static readonly Random random = new Random();

    private Grid grid;

    private char currentPlayer;
    private int gameMode;
    private bool gameOver;

    private Player player1;
    private Player player2;

    private int player1ExplodingUsed = 0;
    private int player1MagneticUsed = 0;
    private int player2ExplodingUsed = 0;
    private int player2MagneticUsed = 0;

    private int player1OrdinaryLeft;
    private int player2OrdinaryLeft;

    private const int SPECIALS_PER_TYPE = 2;
    private const int IMPLEMENTED_SPECIAL_TYPES = 2; 

    public Game(int rows, int columns, int selectedGameMode)
    {
        grid = new Grid(rows, columns); 
        currentPlayer = '@'; // Player 1 always starts
        gameMode = selectedGameMode;
        gameOver = false;

        if (gameMode == 2)
        {
            player1 = new HumanPlayer('@');
            player2 = new ComputerPlayer('#');
        }
        else
        {
            player1 = new HumanPlayer('@');
            player2 = new HumanPlayer('#');
        }

        int discsPerPlayer = (rows * columns) / 2;
        int specialDiscsPerPlayer = SPECIALS_PER_TYPE * IMPLEMENTED_SPECIAL_TYPES; 
        int ordinaryDiscsPerPlayer = discsPerPlayer - specialDiscsPerPlayer;

        player1OrdinaryLeft = ordinaryDiscsPerPlayer;
        player2OrdinaryLeft = ordinaryDiscsPerPlayer;
    }

    private Player GetCurrentPlayerObject()
    {
        if (currentPlayer == '@')
        {
            return player1;
        }

        return player2;
    }
    private void ApplyDiscUsage(char discType)
    {
        if (currentPlayer == '@')
        {
            if (discType == 'O')
                player1OrdinaryLeft--;
            else if (discType == 'E')
                player1ExplodingUsed++;
            else if (discType == 'M')
                player1MagneticUsed++;
        }
        else
        {
            if (discType == 'O')
                player2OrdinaryLeft--;
            else if (discType == 'E')
                player2ExplodingUsed++;
            else if (discType == 'M')
                player2MagneticUsed++;
        }
    }

    // This is separate from CheckDraw(): exploding discs can leave empty spaces
    // even when both players have no discs remaining
    private bool NoDiscsLeftForBothPlayers()
    {
        bool player1Out = player1OrdinaryLeft == 0 &&
                          player1ExplodingUsed >= SPECIALS_PER_TYPE &&
                          player1MagneticUsed >= SPECIALS_PER_TYPE;


        bool player2Out = player2OrdinaryLeft == 0 &&
                          player2ExplodingUsed >= SPECIALS_PER_TYPE &&
                          player2MagneticUsed >= SPECIALS_PER_TYPE;

        return player1Out && player2Out;
    }

    // Polymorphism: the current player decides how to take its turn
    public void Run()
    {
        while (!gameOver)
        {
            Console.Clear();
            grid.Display();
            Console.WriteLine($"\nTurn is {GetPlayerName()} ({currentPlayer})\n");

            Player activePlayer = GetCurrentPlayerObject();
            activePlayer.TakeTurn(this);

            if (gameMode == 2 && currentPlayer == '@')
            {
                Thread.Sleep(700);
            }

            if (grid.CheckWin(currentPlayer))
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine($"{GetPlayerName()} wins!");
                gameOver = true;
            }
            else if (grid.CheckDraw())
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine("Game is a draw!");
                gameOver = true;
            }
            else if (NoDiscsLeftForBothPlayers())
            {
                Console.Clear();
                grid.Display();
                Console.WriteLine("No discs remaining. Game is a draw!");
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

    private string GetPlayerName()
    {
        if (currentPlayer == '@')
        {
            return "Player 1";
        }
        else
        {
            if (gameMode == 2)
            {
                return "Computer";
            }
            else
            {
                return "Player 2";
            }
        }
    }

    private bool MakeMove()
    {
        bool validInput = false;

        while (!validInput)
        {
            int columnNumber = GameMenu.PromptForNumber("Enter column number", 1);

            if (columnNumber > grid.Columns)
            {
                Console.WriteLine($"Invalid column. Try between 1 and {grid.Columns}");
                continue;
            }

            char discType = PromptForDiscType();

            if (discType == '\0')
            {
                return false;
            }

            char discSymbol = GetDiscSymbol(discType);

            int row = grid.DropDiscAndReturnRow(columnNumber, discSymbol);

            if (row == -1)
            {
                Console.WriteLine("The column is full. Try again.\n");
            }
            else
            {
                ApplyDiscUsage(discType);

                if (discType == 'O')
                {
                    Console.Clear();
                    grid.Display();
                }
                else
                {
                    ShowMoveFrames(discType, row, columnNumber);
                }

                validInput = true;
            }
        }

        return true;
    }
    // Handles one player's turn until a valid move is made
    public void HandleTurn()
    {
        bool turnComplete = false;

        while (!turnComplete)
        {
            int userSelection = GameMenu.PromptForMenuOption("1. Make a move\n2. Save game\n3. Help", 1, 3);
            Console.WriteLine();

            if (userSelection == 1)
            {
                turnComplete = MakeMove();
            }
            else if (userSelection == 2)
            {
                SaveGame();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Help Menu\n");

                Console.WriteLine("OPTIONS:");
                Console.WriteLine("1 = Make a move");
                Console.WriteLine("2 = Save the current game");
                Console.WriteLine("3 = View this help menu\n");
                
                Console.WriteLine("HOW TO PLAY:");
                Console.WriteLine("Choose a column number to drop a disc");
                Console.WriteLine("Get enough discs in a row to win\n");

                Console.WriteLine("DISC TYPES:");
                Console.WriteLine("- O = Ordinary disc (standard drop)");
                Console.WriteLine("- E = Exploding disc (2 per player: removes surrounding discs on impact, then disappears)");
                Console.WriteLine("- M = Magnetic disc (2 per player: pulls the nearest matching disc upward after landing)\n");

                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);

                Console.Clear();
                grid.Display();
                Console.WriteLine($"\nTurn is {GetPlayerName()} ({currentPlayer})\n");
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

            if (discType == 'B')
            {
                Console.WriteLine("Boring disc is not implemented in this version.");
                return;
            }

            if (discType != 'O' && discType != 'E' && discType != 'M')
            {
                Console.WriteLine($"Invalid disc type '{discType}'. Use O, E, or M.");
                return;
            }

            if (discType == 'O')
            {
                if (currentPlayer == '@' && player1OrdinaryLeft <= 0)
                {
                    Console.WriteLine("Player @ has no ordinary discs left.");
                    return;
                }

                if (currentPlayer == '#' && player2OrdinaryLeft <= 0)
                {
                    Console.WriteLine("Player # has no ordinary discs left.");
                    return;
                }
            }
            else if (discType == 'E')
            {
                if (currentPlayer == '@' && player1ExplodingUsed >= SPECIALS_PER_TYPE)
                {
                    Console.WriteLine("Player @ has no exploding discs left.");
                    return;
                }

                if (currentPlayer == '#' && player2ExplodingUsed >= SPECIALS_PER_TYPE)
                {
                    Console.WriteLine("Player # has no exploding discs left.");
                    return;
                }
            }
            else if (discType == 'M')
            {
                if (currentPlayer == '@' && player1MagneticUsed >= SPECIALS_PER_TYPE)
                {
                    Console.WriteLine("Player @ has no magnetic discs left.");
                    return;
                }

                if (currentPlayer == '#' && player2MagneticUsed >= SPECIALS_PER_TYPE)
                {
                    Console.WriteLine("Player # has no magnetic discs left.");
                    return;
                }
            }

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

            ApplyDiscUsage(discType);

            if (discType == 'O')
            {
                Console.Clear();
                grid.Display();
            }
            else
            {
                ShowMoveFrames(discType, row, columnNumber);
            }

            Console.WriteLine();

            if (grid.CheckWin(currentPlayer))
            {
                Console.WriteLine($"{GetPlayerName()} wins!");
                return;
            }

            if (grid.CheckDraw())
            {
                Console.WriteLine("Game is a draw!");
                return;
            }

            if (NoDiscsLeftForBothPlayers())
            {
                Console.WriteLine("No discs remaining. Game is a draw!");
                return;
            }

            SwitchPlayer();
        }
    }
    private char PromptForDiscType()
    {
        while (true)
        {
            // Use '\0' to signal that the current player has no legal disc choices left
            if ((currentPlayer == '@' && player1OrdinaryLeft == 0 && player1ExplodingUsed >= SPECIALS_PER_TYPE && player1MagneticUsed >= SPECIALS_PER_TYPE) || 
                (currentPlayer == '#' && player2OrdinaryLeft == 0 && player2ExplodingUsed >= SPECIALS_PER_TYPE && player2MagneticUsed >= SPECIALS_PER_TYPE))
            {
                return '\0';
            }

            Console.WriteLine("Choose disc type:");
            Console.WriteLine("O = Ordinary");
            Console.WriteLine("E = Exploding");
            Console.WriteLine("M = Magnetic");
            Console.Write("> ");

            string input = Console.ReadLine().Trim().ToUpper();

            // Ordinary discs can only be chosen if any remain
            if (input == "O")
            {
                if (currentPlayer == '@' && player1OrdinaryLeft > 0)
                    return 'O';
                if (currentPlayer == '#' && player2OrdinaryLeft > 0)
                    return 'O';

                Console.WriteLine("No ordinary discs left.\n"); 
                continue;
            }

            // Special discs can only be used up to SPECIALS_PER_TYPE times per player
            if (input == "E")
            {
                if (currentPlayer == '@' && player1ExplodingUsed < SPECIALS_PER_TYPE)
                    return 'E';
                if (currentPlayer == '#' && player2ExplodingUsed < SPECIALS_PER_TYPE)
                    return 'E';

                Console.WriteLine("No exploding discs left.\n");
                continue;
            }

            if (input == "M")
            {
                if (currentPlayer == '@' && player1MagneticUsed < SPECIALS_PER_TYPE)
                    return 'M';
                if (currentPlayer == '#' && player2MagneticUsed < SPECIALS_PER_TYPE)
                    return 'M';

                Console.WriteLine("No magnetic discs left.\n");
                continue;
            }

            if (input == "B")
            {
                Console.WriteLine("Boring disc is not implemented in this version.\n");
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

    private List<char> GetAvailableComputerDiscTypes()
    {
        List<char> availableDiscTypes = new List<char>();

        if (player2OrdinaryLeft > 0)
            availableDiscTypes.Add('O');

        if (player2ExplodingUsed < SPECIALS_PER_TYPE)
            availableDiscTypes.Add('E');

        if (player2MagneticUsed < SPECIALS_PER_TYPE)
            availableDiscTypes.Add('M');

        return availableDiscTypes;
    }

    // Temporarily simulate a move, check whether it would win, then restore the original grid state
    private bool WouldMoveWin(char discType, int column)
    {
        string[] originalGrid = grid.GetGridLines();

        char discSymbol = GetDiscSymbol(discType);
        int row = grid.DropDiscAndReturnRow(column, discSymbol);

        if (row == -1)
        {
            return false;
        }

        if (discType == 'E')
        {
            grid.ClearExplosionArea(row, column - 1);
            grid.ApplyGravity();
        }
        else if (discType == 'M')
        {
            grid.ApplyMagneticEffect(row, column - 1, currentPlayer);
        }

        bool isWinningMove = grid.CheckWin(currentPlayer);

        grid.LoadGridLines(originalGrid);

        return isWinningMove;
    }

    // Display intermediate board states so special disc effects can be seen step by step
    private void ShowMoveFrames(char discType, int row, int columnNumber)
    {
        Console.Clear();
        grid.Display();

        if (discType == 'M')
        {
            Thread.Sleep(700);
            grid.ApplyMagneticEffect(row, columnNumber - 1, currentPlayer);

            Console.Clear();
            grid.Display();
        }
        else if (discType == 'E')
        {
            Thread.Sleep(800);
            grid.ClearExplosionArea(row, columnNumber - 1);

            Console.Clear();
            grid.Display();

            Thread.Sleep(800);
            grid.ApplyGravity();

            Console.Clear();
            grid.Display();
        }
    }

    // Computer takes an immediate winning move if one exists; otherwise it picks a random legal move
    public void MakeComputerMove()
    {
        List<int> validColumns = GetValidColumns();

        if (validColumns.Count == 0)
        {
            return;
        }

        List<char> availableDiscTypes = GetAvailableComputerDiscTypes();

        if (availableDiscTypes.Count == 0)
        {
            return;
        }

        int chosenColumn = -1;
        char chosenDiscType = '\0';

        foreach (char discType in availableDiscTypes)
        {
            foreach (int col in validColumns)
            {
                if (WouldMoveWin(discType, col))
                {
                    chosenDiscType = discType;
                    chosenColumn = col;
                    break;
                }
            }

            if (chosenColumn != -1)
            {
                break;
            }
        }

        if (chosenColumn == -1)
        {
            chosenDiscType = availableDiscTypes[random.Next(availableDiscTypes.Count)];
            chosenColumn = validColumns[random.Next(validColumns.Count)];
        }

        char discSymbol = GetDiscSymbol(chosenDiscType);
        int finalRow = grid.DropDiscAndReturnRow(chosenColumn, discSymbol);

        ApplyDiscUsage(chosenDiscType);

        if (chosenDiscType == 'O')
        {
            Console.Clear();
            grid.Display();
            Console.WriteLine($"\nComputer plays {chosenDiscType}{chosenColumn}");
        }
        else
        {
            ShowMoveFrames(chosenDiscType, finalRow, chosenColumn);
            Console.WriteLine($"\nComputer plays {chosenDiscType}{chosenColumn}");
        }

        Thread.Sleep(1000);
    }

    // Save the full game state in a fixed line order so it can be reconstructed later
    private void SaveGame()
    {
        List<string> lines = new List<string>();
        lines.Add(player1ExplodingUsed.ToString());
        lines.Add(player1MagneticUsed.ToString());
        lines.Add(player2ExplodingUsed.ToString());
        lines.Add(player2MagneticUsed.ToString());

        lines.Add(player1OrdinaryLeft.ToString());
        lines.Add(player2OrdinaryLeft.ToString());

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

        int p1O = int.Parse(lines[4]);
        int p2O = int.Parse(lines[5]);

        int rows = int.Parse(lines[6]);
        int columns = int.Parse(lines[7]);
        int gameMode = int.Parse(lines[8]);
        char currentPlayer = lines[9][0];

        Game loadedGame = new Game(rows, columns, gameMode);
        loadedGame.currentPlayer = currentPlayer;

        loadedGame.player1ExplodingUsed = p1E;
        loadedGame.player1MagneticUsed = p1M;
        loadedGame.player2ExplodingUsed = p2E;
        loadedGame.player2MagneticUsed = p2M;

        loadedGame.player1OrdinaryLeft = p1O;
        loadedGame.player2OrdinaryLeft = p2O;

        string[] gridLines = new string[rows];
        for (int i = 0; i < rows; i++)
        {
            gridLines[i] = lines[i + 10];
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
            if (!File.Exists("savegame.txt"))
            {
                Console.WriteLine("No save file found.");
                return;
            }

            game = Game.LoadGame("savegame.txt");
            game.Run();
            return;
        }

        int rows;
        int columns;

        while (true)
        {
            rows = GameMenu.PromptForNumber("Enter number of rows", 6);
            columns = GameMenu.PromptForNumber("Enter number of columns", 7);

            if (rows > columns)
            {
                Console.WriteLine("Grid cannot have more rows than columns.\n");
                continue;
            }

            break;
        }

        int gameMode = GameMenu.PromptForMenuOption(
            "Choose game mode:\n1. Human vs Human\n2. Human vs Computer\n3. Testing Mode",
            1,
            3
        );

        game = new Game(rows, columns, gameMode);

        if (gameMode == 3)
        {
            Console.WriteLine("Enter testing sequence:");
            Console.Write("> ");
            string moveSequence = Console.ReadLine();

            game.RunTestMode(moveSequence);
        }
        else
        {
            game.Run();
        }
    }
}