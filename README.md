# Simple 3D Platformer

Basic 3D platformer I made in Unity. Just a simple game where you jump around and collect stuff.

## What it does

- **Rectangle Character**: Play as a blue rectangular character that can move and jump in 3D space
- **Collectible Coins**: Collect yellow spinning coins to increase your score
- **Dangerous Bombs**: Avoid red pulsing bombs that reset your position when touched
- **Multiple Platforms**: Jump between randomly generated platforms of different sizes
- **Boundaries**: Invisible walls prevent you from falling off the level
- **Score System**: Collect all coins to win the game
- **Lives System**: (Future enhancement)

## Controls

- **WASD**: Move forward, backward, left, and right
- **Spacebar**: Jump
- **Arrow Keys**: Alternative movement controls


## How to run

Open in Unity, hit play. WASD to move, space to jump. Collect coins, avoid bombs.

## Project Structure

```
Simple3DPlatformer/
├── Assets/
│   ├── Scripts/
│   │   ├── PlayerController.cs     # Player movement and physics
│   │   ├── GameManager.cs          # Game state and UI management
│   │   ├── Coin.cs                 # Coin behavior and animation
│   │   ├── Bomb.cs                 # Bomb behavior and effects
│   │   ├── LevelSetup.cs           # Procedural level generation
│   │   └── CameraFollow.cs         # Camera controls
│   ├── Scenes/
│   │   └── GameScene.unity         # Main game scene
│   ├── Materials/                  # (Auto-generated materials)
│   └── Prefabs/                    # (For future prefab storage)
└── ProjectSettings/                # Unity project configuration
