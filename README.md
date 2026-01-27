# BitShift - Binary Logic Mobile Game

A minimalist 2048-style game with binary number mechanics, built with .NET MAUI and SkiaSharp.

## Concept

BitShift combines the addictive merge mechanics of 2048 with binary logic operations. Players swipe to merge matching tiles, with each tile displaying both decimal and binary representations. Future versions will include bit shift operators and logic gates for strategic gameplay.

## Current Features (POC)

- **4x4 Grid**: Classic 2048-style gameplay
- **Binary Display**: Each tile shows both decimal value and 8-bit binary representation
- **Smooth Animations**: Scale animations for new/merged tiles using SkiaSharp
- **Touch Controls**: Swipe in any direction to move tiles
- **Score Tracking**: Points awarded for each merge
- **Clean Architecture**: Separation of concerns with Models, Game Logic, and Rendering
- **Minimalist Design**: Professional aesthetic inspired by Threes and Monument Valley

## Tech Stack

- **.NET MAUI 8.0**: Cross-platform mobile framework
- **SkiaSharp**: Hardware-accelerated 2D graphics rendering
- **C# 12**: Latest language features
- **SOLID Principles**: Clean, testable, maintainable code

## How to Run

### Prerequisites
- Visual Studio 2022 (17.8 or later) with MAUI workload installed
- OR Visual Studio Code with .NET MAUI extension
- .NET 8.0 SDK

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/mcarthey/BitShift.git
   cd BitShift
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Run on your preferred platform:
   
   **Android:**
   ```bash
   dotnet build -t:Run -f net8.0-android
   ```
   
   **iOS (Mac only):**
   ```bash
   dotnet build -t:Run -f net8.0-ios
   ```
   
   **Windows:**
   ```bash
   dotnet build -t:Run -f net8.0-windows10.0.19041.0
   ```

4. Or simply open `BitShift.sln` in Visual Studio and press F5

## Game Controls

- **Swipe Left/Right/Up/Down**: Move all tiles in that direction
- **New Game Button**: Reset the board and start fresh
- **Operator Button**: (Coming soon) Apply bit shift operations

## Reusable Architecture

This project is designed with reusability in mind for future game development:

### Reusable Components

1. **Grid/Cell System** (`GameBoard.cs`)
   - Generic grid management
   - Can be adapted for match-3, puzzle, tower defense games

2. **SkiaSharp Rendering Pipeline** (`MainPage.xaml.cs`)
   - `DrawTile()` method pattern
   - Animation framework
   - Color scheme management

3. **Touch Input Handler**
   - Swipe gesture detection
   - Configurable threshold
   - Direction detection

4. **Game State Management**
   - Score tracking
   - Game over detection
   - Save/load ready structure

## Future Enhancements

### Phase 1: Core Mechanics
- [ ] Bit shift operators (<< and >>)
- [ ] Logic gate operations (AND, OR, XOR)
- [ ] Operator cooldown system
- [ ] Tutorial/onboarding

### Phase 2: Meta-Progression
- [ ] Skill tree system
- [ ] Prestige mechanics
- [ ] Offline/idle earnings
- [ ] Power-ups and boosters

### Phase 3: Monetization & Polish
- [ ] Ad integration (rewarded video, interstitials)
- [ ] In-app purchases
- [ ] Daily challenges
- [ ] Leaderboards
- [ ] Sound effects and music
- [ ] Haptic feedback
- [ ] Particle effects for big combos

## Visual Style

The game uses a clean, minimalist aesthetic:
- Beige/tan color palette (inspired by 2048)
- Rounded corners on all tiles
- Smooth scale animations
- Color-coded tiles based on value
- Binary representation in monospace font
- High contrast for readability

## Color Scheme

Tiles use a progressive color system:
- 2-4: Light beige/cream (dark text)
- 8-64: Orange gradient (white text)
- 128-2048: Yellow/gold gradient (white text)
- Board background: Muted brown (#BBADA0)
- Empty cells: Light brown (#CDC1B4)

## Architecture Notes

The codebase follows SOLID principles:

- **Single Responsibility**: Each class has one job
  - `GameTile`: Represents tile data
  - `GameBoard`: Manages game state and logic
  - `MainPage`: Handles rendering and input

- **Open/Closed**: Easy to extend
  - Add new operators without modifying core logic
  - Add new tile types or animations

- **Dependency Injection Ready**
  - Game logic separated from rendering
  - Easy to add services (audio, analytics, etc.)

## Performance

- 60 FPS rendering on most devices
- Hardware-accelerated SkiaSharp drawing
- Efficient grid updates (only changed cells redraw)
- No garbage collection pressure during gameplay

## License

MIT License - Free to use for learning and commercial projects

## Author

Mark McArthey  
[Learned Geek Consulting](https://learnedgeek.com)  
Building this as part of the "developer bucket list" - proving you can make professional-looking mobile games with .NET MAUI!

---

**Status**: ✅ POC Complete - Playable and looks professional!

Ready to iterate on meta-progression and operator mechanics.
