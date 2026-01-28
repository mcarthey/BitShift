namespace BitShift.Models;

public class GameBoard
{
    private const int GridSize = 4;
    private const int MaxTileValue = 2048;
    private const int MinTileValue = 2;
    private GameTile?[,] _grid;
    private Random _random;

    public int Score { get; private set; }
    public bool GameOver { get; private set; }

    // Tile selection for operator application
    public (int Row, int Col)? SelectedTile { get; private set; }

    // Cooldown system for operators
    public DateTime LastOperatorUse { get; private set; } = DateTime.MinValue;
    public TimeSpan OperatorCooldown { get; } = TimeSpan.FromSeconds(5);

    public GameBoard()
    {
        _grid = new GameTile[GridSize, GridSize];
        _random = new Random();
        InitializeGame();
    }

    public void InitializeGame()
    {
        _grid = new GameTile[GridSize, GridSize];
        Score = 0;
        GameOver = false;
        
        // Start with two tiles
        AddRandomTile();
        AddRandomTile();
    }

    public GameTile?[,] GetGrid() => _grid;

    private void AddRandomTile()
    {
        var emptyCells = new List<(int row, int col)>();
        
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (_grid[row, col] == null)
                    emptyCells.Add((row, col));
            }
        }

        if (emptyCells.Count == 0)
            return;

        var (r, c) = emptyCells[_random.Next(emptyCells.Count)];
        
        // 90% chance of 2, 10% chance of 4
        int value = _random.NextDouble() < 0.9 ? 2 : 4;
        _grid[r, c] = new GameTile(value, r, c);
    }

    public bool Move(SwipeDirection direction)
    {
        bool moved = false;
        
        // Clear merge flags
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (_grid[row, col] != null)
                {
                    _grid[row, col]!.IsMerged = false;
                    _grid[row, col]!.IsNew = false;
                }
            }
        }

        switch (direction)
        {
            case SwipeDirection.Left:
                moved = MoveLeft();
                break;
            case SwipeDirection.Right:
                moved = MoveRight();
                break;
            case SwipeDirection.Up:
                moved = MoveUp();
                break;
            case SwipeDirection.Down:
                moved = MoveDown();
                break;
        }

        if (moved)
        {
            AddRandomTile();
            CheckGameOver();
        }

        return moved;
    }

    private bool MoveLeft()
    {
        bool moved = false;
        
        for (int row = 0; row < GridSize; row++)
        {
            int writePos = 0;
            
            for (int col = 0; col < GridSize; col++)
            {
                if (_grid[row, col] != null)
                {
                    var tile = _grid[row, col]!;
                    
                    if (writePos > 0 && _grid[row, writePos - 1] != null && 
                        _grid[row, writePos - 1]!.Value == tile.Value && 
                        !_grid[row, writePos - 1]!.IsMerged)
                    {
                        // Merge
                        _grid[row, writePos - 1]!.Value *= 2;
                        _grid[row, writePos - 1]!.IsMerged = true;
                        Score += _grid[row, writePos - 1]!.Value;
                        _grid[row, col] = null;
                        moved = true;
                    }
                    else
                    {
                        // Move
                        if (col != writePos)
                        {
                            _grid[row, writePos] = tile;
                            _grid[row, writePos]!.Column = writePos;
                            _grid[row, col] = null;
                            moved = true;
                        }
                        writePos++;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveRight()
    {
        bool moved = false;
        
        for (int row = 0; row < GridSize; row++)
        {
            int writePos = GridSize - 1;
            
            for (int col = GridSize - 1; col >= 0; col--)
            {
                if (_grid[row, col] != null)
                {
                    var tile = _grid[row, col]!;
                    
                    if (writePos < GridSize - 1 && _grid[row, writePos + 1] != null && 
                        _grid[row, writePos + 1]!.Value == tile.Value && 
                        !_grid[row, writePos + 1]!.IsMerged)
                    {
                        // Merge
                        _grid[row, writePos + 1]!.Value *= 2;
                        _grid[row, writePos + 1]!.IsMerged = true;
                        Score += _grid[row, writePos + 1]!.Value;
                        _grid[row, col] = null;
                        moved = true;
                    }
                    else
                    {
                        // Move
                        if (col != writePos)
                        {
                            _grid[row, writePos] = tile;
                            _grid[row, writePos]!.Column = writePos;
                            _grid[row, col] = null;
                            moved = true;
                        }
                        writePos--;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveUp()
    {
        bool moved = false;
        
        for (int col = 0; col < GridSize; col++)
        {
            int writePos = 0;
            
            for (int row = 0; row < GridSize; row++)
            {
                if (_grid[row, col] != null)
                {
                    var tile = _grid[row, col]!;
                    
                    if (writePos > 0 && _grid[writePos - 1, col] != null && 
                        _grid[writePos - 1, col]!.Value == tile.Value && 
                        !_grid[writePos - 1, col]!.IsMerged)
                    {
                        // Merge
                        _grid[writePos - 1, col]!.Value *= 2;
                        _grid[writePos - 1, col]!.IsMerged = true;
                        Score += _grid[writePos - 1, col]!.Value;
                        _grid[row, col] = null;
                        moved = true;
                    }
                    else
                    {
                        // Move
                        if (row != writePos)
                        {
                            _grid[writePos, col] = tile;
                            _grid[writePos, col]!.Row = writePos;
                            _grid[row, col] = null;
                            moved = true;
                        }
                        writePos++;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveDown()
    {
        bool moved = false;
        
        for (int col = 0; col < GridSize; col++)
        {
            int writePos = GridSize - 1;
            
            for (int row = GridSize - 1; row >= 0; row--)
            {
                if (_grid[row, col] != null)
                {
                    var tile = _grid[row, col]!;
                    
                    if (writePos < GridSize - 1 && _grid[writePos + 1, col] != null && 
                        _grid[writePos + 1, col]!.Value == tile.Value && 
                        !_grid[writePos + 1, col]!.IsMerged)
                    {
                        // Merge
                        _grid[writePos + 1, col]!.Value *= 2;
                        _grid[writePos + 1, col]!.IsMerged = true;
                        Score += _grid[writePos + 1, col]!.Value;
                        _grid[row, col] = null;
                        moved = true;
                    }
                    else
                    {
                        // Move
                        if (row != writePos)
                        {
                            _grid[writePos, col] = tile;
                            _grid[writePos, col]!.Row = writePos;
                            _grid[row, col] = null;
                            moved = true;
                        }
                        writePos--;
                    }
                }
            }
        }
        
        return moved;
    }

    private void CheckGameOver()
    {
        // Check if there are any empty cells
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (_grid[row, col] == null)
                    return;
            }
        }

        // Check if any adjacent tiles can merge
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var tile = _grid[row, col]!;
                
                // Check right
                if (col < GridSize - 1 && _grid[row, col + 1]!.Value == tile.Value)
                    return;
                
                // Check down
                if (row < GridSize - 1 && _grid[row + 1, col]!.Value == tile.Value)
                    return;
            }
        }

        GameOver = true;
    }

    /// <summary>
    /// Selects a tile at the given position for operator application.
    /// Returns true if a tile exists at the position.
    /// </summary>
    public bool SelectTile(int row, int col)
    {
        if (row < 0 || row >= GridSize || col < 0 || col >= GridSize)
            return false;

        if (_grid[row, col] == null)
        {
            SelectedTile = null;
            return false;
        }

        SelectedTile = (row, col);
        return true;
    }

    /// <summary>
    /// Clears the current tile selection.
    /// </summary>
    public void ClearSelection()
    {
        SelectedTile = null;
    }

    /// <summary>
    /// Checks if an operator can be used (cooldown elapsed).
    /// </summary>
    public bool CanUseOperator()
    {
        return DateTime.Now - LastOperatorUse >= OperatorCooldown;
    }

    /// <summary>
    /// Returns remaining cooldown time in seconds.
    /// </summary>
    public double GetCooldownRemaining()
    {
        var elapsed = DateTime.Now - LastOperatorUse;
        var remaining = OperatorCooldown - elapsed;
        return remaining.TotalSeconds > 0 ? remaining.TotalSeconds : 0;
    }

    /// <summary>
    /// Applies left shift (<<) to the selected tile.
    /// Doubles the tile value. Returns false if at max or no selection.
    /// </summary>
    public bool ApplyLeftShift()
    {
        if (!SelectedTile.HasValue || !CanUseOperator())
            return false;

        var (row, col) = SelectedTile.Value;
        var tile = _grid[row, col];

        if (tile == null || tile.Value >= MaxTileValue)
            return false;

        // Left shift = multiply by 2
        tile.Value <<= 1;
        tile.IsMerged = true; // Trigger animation
        LastOperatorUse = DateTime.Now;
        ClearSelection();

        // Award points for the shift (half the new value)
        Score += tile.Value / 2;

        CheckGameOver();
        return true;
    }

    /// <summary>
    /// Applies right shift (>>) to the selected tile.
    /// Halves the tile value. Returns false if at min or no selection.
    /// </summary>
    public bool ApplyRightShift()
    {
        if (!SelectedTile.HasValue || !CanUseOperator())
            return false;

        var (row, col) = SelectedTile.Value;
        var tile = _grid[row, col];

        if (tile == null || tile.Value <= MinTileValue)
            return false;

        // Right shift = divide by 2
        tile.Value >>= 1;
        tile.IsMerged = true; // Trigger animation
        LastOperatorUse = DateTime.Now;
        ClearSelection();

        return true;
    }
}

public enum SwipeDirection
{
    Left,
    Right,
    Up,
    Down
}
