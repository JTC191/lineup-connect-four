**LineUp – C# Console Game**

A console-based strategy game inspired by Connect Four, developed in C# using object-oriented programming principles.

**Overview**

This project was developed as part of a university assignment to apply object-oriented design and implement a complete interactive system.

**Features**

- Human vs Human and Human vs Computer gameplay  
- Special disc mechanics:
  - Exploding discs (remove surrounding discs)
  - Magnetic discs (shift discs within a column)
- Save and load functionality using file I/O  
- Testing mode for validating game scenarios  
- Dynamic win condition based on grid size  

**Technologies Used**

- C#
- .NET Console Application
- Object-Oriented Programming (OOP)
  - Inheritance
  - Polymorphism
  - Abstraction
- File I/O for saving and loading game state  

**Project Structure**

- `Game` – controls game flow, player turns, and logic  
- `Grid` – manages board state, win conditions, and mechanics  
- `Player` – abstract class with Human and Computer implementations  
- `GameMenu` – handles user input and validation  

**Key Concepts Demonstrated**

- Designing a modular system using object-oriented principles  
- Managing application state across multiple interacting components  
- Implementing rule-based mechanics (explosion effects, gravity, magnetic behaviour)  
- Developing algorithms for win detection across multiple directions  
- Validating user input and handling edge cases  

**How to Run**

1. Open the project in Visual Studio  
2. Build and run the solution  
3. Follow the console prompts to start a new game or load a saved game  

**Future Improvements**

- Improve computer decision-making logic (currently selects random valid moves unless a winning move is available)
- Enhance user interface and visual feedback  
- Expand game mechanics or modes
- Refactor disc behaviour into separate classes using polymorphism
