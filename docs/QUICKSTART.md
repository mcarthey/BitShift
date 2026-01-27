# BitShift - Quick Start Guide

Get the game running in under 5 minutes!

## Prerequisites Check

Before you start, make sure you have:
- [ ] Visual Studio 2022 (17.8+) with .NET MAUI workload
- [ ] .NET 8.0 SDK installed
- [ ] Android SDK (for Android testing)
- [ ] An Android emulator or physical device

## Step-by-Step Setup

### 1. Clone the Repository

```bash
git clone https://github.com/mcarthey/BitShift.git
cd BitShift
```

### 2. Open in Visual Studio

**Option A: Double-click**
- Double-click `BitShift.sln` to open in Visual Studio

**Option B: Command line**
```bash
start BitShift.sln
```

### 3. Restore NuGet Packages

Visual Studio should automatically restore packages. If not:
- Right-click solution → Restore NuGet Packages
- Or run: `dotnet restore`

### 4. Select Target Platform

In the Visual Studio toolbar:
- Select target: `Android Emulator` or `Windows Machine`
- Select configuration: `Debug`

### 5. Run the App

**Method 1: Press F5**

**Method 2: Click the green "Play" button**

**Method 3: Command line**
```bash
# Android
dotnet build -t:Run -f net8.0-android

# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

### 6. Play!

Once the app launches:
1. Swipe in any direction to move tiles
2. Merge matching numbers (2+2=4, 4+4=8, etc.)
3. Watch the binary representation update
4. Try to reach 2048!

## Troubleshooting

### "SkiaSharp package not found"
```bash
dotnet restore
# Or
dotnet add package SkiaSharp.Views.Maui.Controls --version 2.88.7
```

### "Android SDK not found"
1. Open Visual Studio Installer
2. Modify → Individual Components
3. Check "Android SDK" components
4. Install

### "No Android emulator available"
1. Tools → Android → Android Device Manager
2. Create a new device (Pixel 5, Android 11+)
3. Start the emulator

### "Build failed - can't find MainPage.xaml.cs"
The file might not be nested under MainPage.xaml in Solution Explorer:
1. Close Visual Studio
2. Delete `.vs` folder in solution directory
3. Reopen solution

### "Touch gestures not working in emulator"
- Use mouse to simulate swipes (click and drag)
- Or enable touch input in emulator settings

## What You're Seeing

### Visual Elements

1. **Header**
   - "BitShift" title
   - Score display (top right)

2. **Game Board**
   - 4x4 grid with tan/beige background
   - Tiles showing decimal numbers (top)
   - Binary representation (bottom, smaller)
   - Colors change based on tile value

3. **Controls**
   - "New Game" button (reset)
   - "Left Shift" button (disabled in POC)

### Game Mechanics

- **Swipe gestures**: Move all tiles in that direction
- **Merging**: Two tiles with same value → combined tile with doubled value
- **Scoring**: Each merge adds points
- **New tiles**: Random 2 or 4 spawns after each move
- **Game over**: Board fills with no possible moves

## Next Steps After Running

### Experiment with the Code

1. **Change tile colors** (`MainPage.xaml.cs`, line ~27)
   ```csharp
   _tileColors[2] = SKColor.Parse("#YOUR_COLOR");
   ```

2. **Modify grid size** (`GameBoard.cs`, line 5)
   ```csharp
   private const int GridSize = 5; // Try 5x5 or 3x3
   ```

3. **Adjust animation speed** (`MainPage.xaml.cs`, line ~144)
   ```csharp
   if (timeSinceUpdate < 0.3) // Change 0.15 to 0.3 for slower
   ```

4. **Change swipe sensitivity** (`MainPage.xaml.cs`, line ~22)
   ```csharp
   private const float MinSwipeDistance = 100f; // Increase for longer swipes
   ```

### Add Your First Feature

Try implementing a "best score" tracker:
1. Add property to `GameBoard.cs`: `public int BestScore { get; private set; }`
2. Update on new high score: `if (Score > BestScore) BestScore = Score;`
3. Display in `MainPage.xaml` next to current score

## Understanding the Code Flow

```
User Swipes
    ↓
OnCanvasViewTouch (MainPage.xaml.cs)
    ↓
Detects SwipeDirection
    ↓
GameBoard.Move(direction)
    ↓
Tiles shift, merge, new tile spawns
    ↓
GameCanvas.InvalidateSurface()
    ↓
OnCanvasViewPaintSurface renders new state
    ↓
SkiaSharp draws tiles with animations
```

## Performance Tips

- Runs at 60 FPS on most modern devices
- If laggy, reduce tile animation complexity
- On older Android devices, disable anti-aliasing:
  ```csharp
  IsAntialias = false
  ```

## File Overview

**Core Game Files:**
- `Models/GameTile.cs` - Tile data structure
- `Models/GameBoard.cs` - Game logic and rules
- `MainPage.xaml.cs` - Rendering with SkiaSharp

**MAUI Boilerplate:**
- `App.xaml.cs` - Application entry point
- `MauiProgram.cs` - Dependency injection setup
- `AppShell.xaml` - Navigation shell

**Resources:**
- `Resources/Styles/` - Colors and styles
- `Resources/AppIcon/` - App icon SVG
- `Resources/Splash/` - Splash screen

## Common Modifications

### Want to add sounds?

1. Add sound files to `Resources/Raw/`
2. Use MAUI's `IMediaPicker` or plugin like `Plugin.Maui.Audio`
3. Play on merge: `audioService.Play("merge.mp3");`

### Want to save high scores?

1. Use `Preferences.Set("BestScore", bestScore);`
2. Load on startup: `var best = Preferences.Get("BestScore", 0);`

### Want to add haptic feedback?

```csharp
#if ANDROID || IOS
HapticFeedback.Perform(HapticFeedbackType.LongPress);
#endif
```

## Resources

- [SkiaSharp Documentation](https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/)
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [2048 Game Rules](https://en.wikipedia.org/wiki/2048_(video_game))

## Getting Help

**Issues with the code?**
- Check ARCHITECTURE.md for design explanations
- Review inline comments in MainPage.xaml.cs
- Search GitHub issues in the repo

**MAUI-specific problems?**
- [.NET MAUI Community Discord](https://aka.ms/dotnet-discord)
- [Stack Overflow - maui tag](https://stackoverflow.com/questions/tagged/maui)

## What's Next?

Once you have it running, check out:
1. **README.md** - Full project overview and features
2. **ARCHITECTURE.md** - Deep dive into design decisions
3. GitHub Issues - See planned features and enhancements

Now go make that first dollar! 💰
