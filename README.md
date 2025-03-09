# Noah's Game

A fun endless runner platform game built with MonoGame where you play as Noah collecting fruits while running and jumping through an ever-scrolling world.


## Features

- Endless side-scrolling platformer gameplay
- Character featuring Noah's head with animated body parts
- Double jump mechanics for advanced platforming
- Collectible fruits (oranges and bananas) that add to your score
- Parallax scrolling background with mountains and clouds
- High score system
- Smooth animations and physics

## How to Play

- **Move Right**: Press D or Right Arrow (Noah always moves forward, but you can speed up)
- **Jump**: Press W or Space
- **Double Jump**: Press Jump again while in the air
- **Collect Fruits**: Touch oranges and bananas to earn bonus points (+10 each)
- **Score**: Survive as long as possible and collect fruits to increase your score
- **Game Over**: Falling off the bottom of the screen ends the game
- **Restart**: Press Space to play again after game over
- **Quit**: Press Escape to quit the game

## Controls

- **Space/W**: Jump (can be used twice for double jump)
- **D/Right Arrow**: Move faster
- **Escape**: Quit game
- **Space**: Start game/Restart after game over

## Scoring

- Time-based scoring: 1 point per second
- Collect fruits: +10 points each
- High scores are saved for the current session

## Technical Details

Built using:
- MonoGame Framework
- C#
- .NET 8.0

## Installation

1. Make sure you have .NET 8.0 or higher installed
2. Clone the repository
3. Build and run the project:
```bash
dotnet build
dotnet run
```

## Development

The game features:
- Object-oriented design with separate classes for Player, Platform, Food, and Background elements
- Smooth physics-based movement and collisions
- Dynamic platform and collectible generation
- State management for game flow (Title Screen, Playing, Game Over)
- Efficient texture rendering and animation system
- Parallax scrolling background system

## Credits

- Game Design: Noah's Game
- Programming: Platform Game Project
- Character Design: Features Noah's head with animated body