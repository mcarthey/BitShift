namespace BitShift.Models;

public class GameBoard
{
    private const int GridSize = 4;
    private GameTile?[,] _grid;
    private Random _random;
    
    public int Score { get; private set; }
    public bool GameOver { get; private set; }

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
}

public enum SwipeDirection
{
    Left,
    Right,
    Up,
    Down
}
