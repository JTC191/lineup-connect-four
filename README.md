**LineUp – C# Console Game**

A console-based strategy game inspired by Connect Four, developed in C# using object-oriented programming principles.

**Academic Context**

Developed for QUT's Object-Oriented Design and Development unit.
Result: 14.5/15

**Overview**

This project was developed as part of a university assignment to apply object-oriented design and implement a complete interactive system.

**Screenshot**

<img width="315" height="389" alt="image" src="https://github.com/user-attachments/assets/e02fe5dd-17ae-4ede-85fe-98a8581b17d3" />


**Features**

- Human vs Human and Human vs Computer gameplay  
- Special disc mechanics:
  - Exploding discs (remove surrounding discs)
  - Magnetic discs (shift discs within a column)
- Save and load functionality using file I/O  
- Testing mode for validating game scenarios  
- Dynamic win condition based on grid size
- Computer opponent checks for an immediate winning move before selecting a random legal move

**Technologies Used**

- C#
- .NET Console Application
- Object-Oriented Programming (OOP)
  - Inheritance
  - Polymorphism
  - Abstraction
- File I/O for saving and loading game state  

**Core Components**

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

1. Clone or download the repository
2. Open the solution in Visual Studio  
3. Build and run the project  
4. Follow the console prompts to start or load a game

**Testing Mode**

Testing mode accepts comma-separated moves consisting of a disc type and column number.

Example:

`O1, O2, E3, M4`

- 'O' - ordinary disc
- 'E' - exploding disc
- 'M' - magnetic disc

**Future Improvements**

- Improve computer decision-making logic (currently selects random valid moves unless a winning move is available)
- Enhance user interface and visual feedback  
- Expand game mechanics or modes
- Refactor disc behaviour into separate classes using polymorphism
