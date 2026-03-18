class GameMenu
{
    public static int PromptForMenuOption(string userRequest)
    {
        int menuOption;

        while (true)
        {
            Console.WriteLine(userRequest);
            Console.WriteLine();
            Console.Write("> ");

            if (int.TryParse(Console.ReadLine(), out menuOption))
            {
                if (menuOption == 1 || menuOption == 2)
                {
                    Console.WriteLine();
                    return menuOption;
                }
            }

            Console.WriteLine("Invalid input. Please try again.");
            Console.WriteLine();
        }
    }
    public static int PromptForNumber(string userRequest)
    {
        int value;

        while (true)
        {
            Console.WriteLine(userRequest);
            Console.Write("> ");

            if (int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine();
                return value;
            }

            Console.WriteLine("Invalid input. Please enter a number.");
            Console.WriteLine();
        }
    }
}

class Grid
{
    public int Rows { get; }
    public int Cols { get; }

    public Grid(int rows, int cols)
    {
        if (rows < 6)
            throw new ArgumentException("Rows must be at least 6.");

        if (cols < 7)
            throw new ArgumentException("Columns must be at least 7.");

        if (rows > cols)
            throw new ArgumentException("Grid cannot have more rows than columns.");
        
        Rows = rows;
        Cols = cols;
    }
}
class Program
{
    static void Main()
    {
        int mainChoice = GameMenu.PromptForMenuOption("Press 1. Load Game\nPress 2. New Game");

        if (mainChoice == 2)
        {
            int gameMode = GameMenu.PromptForMenuOption("Press 1. Human vs Human\nPress 2. Human vs Computer");
            int rows = GameMenu.PromptForNumber("Enter number of rows (minimum 6)");
            int cols = GameMenu.PromptForNumber("Enter number of columns (minimum 7)");

            Grid grid = new Grid(rows, cols);
        }
    }
}
