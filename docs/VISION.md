# BitShift: Project Vision Document

## Executive Summary

BitShift is a minimalist mobile puzzle game that combines the addictive mechanics of 2048 with binary/bitwise operations, creating a unique educational-yet-casual gaming experience. The primary goal is simple: **earn the first dollar from a mobile game** while building a reusable game framework for future projects.

This is a "developer bucket list" project—proving the end-to-end mobile game pipeline from development through monetization.

---

## Mission Statement

> Build a polished, profitable mobile game in minimal time using clean architecture principles, maximizing code reusability for future game projects.

---

## Target Audience

### Primary
- **Casual puzzle gamers** (25-45 years old) who enjoy 2048, Threes!, and similar merge games
- **Tech-curious players** interested in binary numbers and programming concepts
- **Students** learning computer science fundamentals

### Secondary
- **Developers** who want to understand bitwise operations through play
- **Educators** looking for interactive tools to teach binary math

### Audience Characteristics
- Short attention spans (3-5 minute play sessions)
- Prefer portrait mode, one-handed gameplay
- Value aesthetic polish and smooth animations
- Willing to watch ads for bonuses, rarely purchase

---

## Core Gameplay

### The Hook
"2048, but you can hack the tiles."

### Base Mechanics (2048 Foundation)
- 4x4 grid of tiles
- Swipe to move all tiles in a direction
- Matching tiles merge (2+2=4, 4+4=8, etc.)
- New tile spawns after each move
- Goal: Reach 2048 (or beyond)
- Game over when no moves remain

### BitShift Twist (The Differentiator)
Each tile displays both its **decimal value** and **8-bit binary representation**:
```
   32
00100000
```

**Bit Shift Operators:**
| Operator | Symbol | Effect | Strategic Use |
|----------|--------|--------|---------------|
| Left Shift | `<<` | Doubles value (×2) | Power up small tiles quickly |
| Right Shift | `>>` | Halves value (÷2) | Create merge opportunities |

**Operator Rules:**
- Tap any tile to select it (golden highlight)
- Press operator button to apply shift
- 5-second cooldown between uses
- Left shift awards points (encourages use)
- Right shift is purely strategic (no points)
- Cannot shift below 2 or above 2048

### Why This Works
1. **Familiar foundation** - 2048 mechanics need no tutorial
2. **Added depth** - Operators create new strategic possibilities
3. **Educational** - Players learn binary through gameplay
4. **Satisfying** - Watching `00000100` become `00001000` is oddly delightful

---

## Visual Design

### Aesthetic Direction
- **Minimalist** - Clean lines, no clutter
- **Warm palette** - Inspired by Threes! and Monument Valley
- **Binary emphasis** - Monospace fonts, terminal-like binary display
- **Professional polish** - Smooth 60 FPS animations

### Color Philosophy
```
Background:  #FAF8EF (warm cream)
Grid:        #BBADA0 (warm brown)
Empty cells: #CDC1B4 (light taupe)

Tiles (value-based gradient):
2:    #EEE4DA    (cream)
4:    #EDE0C8    (light tan)
8:    #F2B179    (orange)
16:   #F59563    (dark orange)
32:   #F67C5F    (coral)
64:   #F65E3B    (red-orange)
128:  #EDCF72    (gold)
256:  #EDCC61    (bright gold)
512:  #EDC850    (yellow-gold)
1024: #EDC53F    (amber)
2048: #EDC22E    (victory gold)
```

### Animation Principles
- **Spawn**: Scale from 0.8→1.0 over 150ms
- **Merge**: Brief pulse effect on combined tile
- **Selection**: Golden glow border (pulsing)
- **Operator**: Visual feedback showing binary shift

---

## Technical Architecture

### Technology Stack
| Component | Choice | Rationale |
|-----------|--------|-----------|
| Framework | .NET MAUI 10.0 | Cross-platform, C# expertise, single codebase |
| Rendering | SkiaSharp | Hardware-accelerated 2D, no Unity overhead |
| Architecture | MVVM-lite | Clean separation, testable, familiar |
| Target Platforms | Android, iOS, Windows, Mac | MAUI's strength |

### Project Structure
```
BitShift/
├── src/
│   ├── Models/
│   │   ├── GameBoard.cs      # Game state & logic
│   │   └── GameTile.cs       # Individual tile data
│   ├── Views/
│   │   ├── MainPage.xaml     # UI layout
│   │   └── MainPage.xaml.cs  # Rendering & input
│   ├── Platforms/            # Platform-specific code
│   ├── Resources/            # Assets, fonts, icons
│   ├── App.xaml              # Application entry
│   └── MauiProgram.cs        # DI & configuration
├── tests/                    # Unit tests (future)
└── docs/                     # Documentation
```

### Design Principles
1. **Separation of Concerns** - Logic in Models, rendering in Views
2. **Immediate Mode Rendering** - Redraw everything each frame (simple, fast)
3. **No GameObject Soup** - Unlike Unity, no complex hierarchies
4. **Testable Core** - GameBoard is pure logic, easily unit tested
5. **Reusable Patterns** - Color dictionary, animation helpers, touch handling

### Performance Targets
- 60 FPS on mid-range devices (16ms frame budget)
- <100ms touch response
- <2 second cold start
- <50MB installed size

---

## Monetization Strategy

### Philosophy
> Respect the player. Make money through value, not manipulation.

### Revenue Streams (Priority Order)

#### 1. Rewarded Video Ads (Primary)
- **Undo last move** - Watch ad to reverse a mistake
- **Extra operator use** - Skip cooldown with ad
- **Continue after game over** - One more chance

**Why it works:** Player chooses to watch, feels fair, high eCPM.

#### 2. Interstitial Ads (Secondary)
- Show between games (not during)
- Frequency cap: 1 per 3 games maximum
- Skip button after 5 seconds

#### 3. Remove Ads IAP ($2.99)
- One-time purchase removes all ads
- Keeps rewarded ads as optional (player benefits)
- Simple, ethical, expected by users

#### 4. Cosmetic Themes (Future - $0.99 each)
- "Terminal" - Green on black, hacker aesthetic
- "Blueprint" - Technical drawing style
- "Neon" - Cyberpunk glow effects

### What We Won't Do
- No pay-to-win mechanics
- No energy systems or timers
- No loot boxes or gambling mechanics
- No dark patterns or manipulative UX
- No selling user data

### Revenue Projection (Conservative)
| Metric | Value |
|--------|-------|
| Target DAU | 100 |
| Ad views/user/day | 2 |
| eCPM | $5 |
| Daily ad revenue | $1 |
| **Monthly revenue** | **$30** |

**Goal: First $1 within 30 days of launch.**

---

## Development Roadmap

### Phase 1: Core Game (CURRENT)
- [x] 4x4 grid with 2048 mechanics
- [x] Binary display on tiles
- [x] Swipe controls
- [x] Score tracking
- [x] Game over detection
- [x] Tile selection (tap to select)
- [x] Left shift operator (<<)
- [x] Right shift operator (>>)
- [x] 5-second operator cooldown
- [ ] Sound effects
- [ ] Haptic feedback

### Phase 2: Polish & Launch Prep
- [ ] Tutorial/onboarding (3 screens max)
- [ ] High score persistence (local)
- [ ] Settings screen (sound, haptics toggles)
- [ ] App icon and store assets
- [ ] Privacy policy page
- [ ] Rate/review prompt (after 5 games)

### Phase 3: Monetization
- [ ] AdMob integration
- [ ] Rewarded video: Undo move
- [ ] Rewarded video: Skip cooldown
- [ ] Interstitial between games
- [ ] Remove Ads IAP

### Phase 4: Engagement
- [ ] Daily challenges
- [ ] Statistics tracking (games played, best tile, etc.)
- [ ] Achievements/badges
- [ ] Share score to social media

### Phase 5: Meta-Progression (Future)
- [ ] XP and leveling system
- [ ] Skill tree (unlock new operators)
- [ ] Additional operators: AND, OR, XOR, NOT
- [ ] Prestige system (reset for permanent bonuses)
- [ ] Offline earnings

### Phase 6: Expansion (Future)
- [ ] Different grid sizes (3x3, 5x5, 6x6)
- [ ] Challenge modes (timed, limited moves)
- [ ] Leaderboards (Game Center / Play Games)
- [ ] Cloud save sync

---

## Success Metrics

### Launch Goals (First 30 Days)
| Metric | Target |
|--------|--------|
| Total downloads | 100+ |
| Day 1 retention | >30% |
| Day 7 retention | >10% |
| Revenue | >$1 |
| Crash-free rate | >99% |
| Store rating | >4.0 stars |

### Long-Term Goals (6 Months)
| Metric | Target |
|--------|--------|
| Monthly active users | 500+ |
| Monthly revenue | $50+ |
| Total revenue | $100+ |

### Learning Goals
- [x] Build a complete mobile game
- [ ] Navigate app store submission process
- [ ] Implement mobile ad SDK
- [ ] Handle IAP on both platforms
- [ ] Respond to user feedback/reviews
- [ ] Iterate based on analytics data

---

## Competitive Analysis

### Direct Competitors
| Game | Strengths | Weaknesses | Our Advantage |
|------|-----------|------------|---------------|
| 2048 | Iconic, free | No innovation, ad-heavy | Unique mechanics |
| Threes! | Beautiful, original | Paid ($6), no updates | Free + fresh twist |
| 2048 Puzzle | Polished, many modes | Generic, crowded | Educational angle |

### Indirect Competitors
- Wordle (daily puzzle habit)
- Candy Crush (match mechanics)
- Sudoku apps (number puzzles)

### Differentiation
1. **Binary display** - Unique visual identity
2. **Bit shift operators** - Novel mechanic
3. **Educational value** - Learn while playing
4. **Clean monetization** - No predatory tactics
5. **Open architecture** - Framework for future games

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| No downloads | Medium | High | ASO, social sharing, cross-promotion |
| Low retention | Medium | High | Polish, tutorials, daily hooks |
| Ad revenue too low | High | Medium | Multiple revenue streams, IAP |
| App store rejection | Low | High | Follow guidelines strictly |
| Burnout/abandonment | Medium | High | MVP scope, time-boxed phases |
| Technical issues | Low | Medium | Simple architecture, testing |

---

## Future Vision

BitShift is the first game in a planned series of **minimalist logic games**:

1. **BitShift** - Binary/bit operations (this project)
2. **LogicGates** - AND, OR, NOT puzzle game
3. **HexMerge** - Hexadecimal number merging
4. **StackSort** - Data structure puzzles
5. **RegexRun** - Pattern matching game

All games will share:
- Common rendering engine
- Consistent visual style
- Shared monetization framework
- Cross-promotion opportunities

---

## Appendix: Binary Reference

For players (and developers) unfamiliar with binary:

```
Decimal  Binary      Shift Left (<<)  Shift Right (>>)
------------------------------------------------------
2        00000010    → 4 (00000100)   → 1 (00000001)*
4        00000100    → 8 (00001000)   → 2 (00000010)
8        00001000    → 16 (00010000)  → 4 (00000100)
16       00010000    → 32 (00100000)  → 8 (00001000)
32       00100000    → 64 (01000000)  → 16 (00010000)
64       01000000    → 128 (10000000) → 32 (00100000)
128      10000000    → 256*           → 64 (01000000)
256      100000000   → 512            → 128

* In game, minimum is 2 and maximum is 2048
```

**Fun fact:** Left shifting is equivalent to multiplying by 2, and right shifting is dividing by 2. CPUs use this for fast multiplication!

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-28 | Claude Code | Initial vision document |

---

*"Ship it fast, iterate based on data, make that first dollar."*
