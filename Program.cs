using System;

class GameMenu
{
    public static int PromptForUser(string userRequest)
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
}
class Program
{
    static void Main()
    {
        int mainChoice = GameMenu.PromptForUser("Press 1. Load Game\nPress 2. New Game");

        if (mainChoice == 2)
        {
            int gameMode = GameMenu.PromptForUser("Press 1. Human vs Human\nPress 2. Human vs Computer");
        }
    }
}
