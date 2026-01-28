using BitShift.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace BitShift;

public partial class MainPage : ContentPage
{
	private GameBoard _gameBoard;
	private DateTime _lastUpdate;
	private SKPoint? _touchStartPoint;
	private const float MinSwipeDistance = 50f;
	private const float TapThreshold = 15f;

	// Cached layout info for hit testing
	private float _tileSize;
	private float _offsetX;
	private float _offsetY;
	private float _padding = 10f;
	
	// Color scheme inspired by 2048
	private readonly Dictionary<int, SKColor> _tileColors = new()
	{
		{ 2, SKColor.Parse("#EEE4DA") },
		{ 4, SKColor.Parse("#EDE0C8") },
		{ 8, SKColor.Parse("#F2B179") },
		{ 16, SKColor.Parse("#F59563") },
		{ 32, SKColor.Parse("#F67C5F") },
		{ 64, SKColor.Parse("#F65E3B") },
		{ 128, SKColor.Parse("#EDCF72") },
		{ 256, SKColor.Parse("#EDCC61") },
		{ 512, SKColor.Parse("#EDC850") },
		{ 1024, SKColor.Parse("#EDC53F") },
		{ 2048, SKColor.Parse("#EDC22E") },
	};

	public MainPage()
	{
		InitializeComponent();
		_gameBoard = new GameBoard();
		_lastUpdate = DateTime.Now;
		
		// Start animation loop
		Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
		{
			GameCanvas.InvalidateSurface();
			return true;
		});
	}

	private void OnCanvasViewPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.Transparent);

		var info = e.Info;
		var gridSize = 4;

		// Calculate tile size to fit the canvas
		var availableSize = Math.Min(info.Width, info.Height);
		_tileSize = (availableSize - (_padding * (gridSize + 1))) / gridSize;

		// Center the grid - cache for hit testing
		_offsetX = (info.Width - availableSize) / 2;
		_offsetY = (info.Height - availableSize) / 2;

		// Draw empty grid cells
		for (int row = 0; row < gridSize; row++)
		{
			for (int col = 0; col < gridSize; col++)
			{
				var x = _offsetX + _padding + col * (_tileSize + _padding);
				var y = _offsetY + _padding + row * (_tileSize + _padding);

				DrawEmptyCell(canvas, x, y, _tileSize);
			}
		}

		// Draw tiles
		var grid = _gameBoard.GetGrid();
		var selected = _gameBoard.SelectedTile;
		for (int row = 0; row < gridSize; row++)
		{
			for (int col = 0; col < gridSize; col++)
			{
				var tile = grid[row, col];
				if (tile != null)
				{
					var x = _offsetX + _padding + col * (_tileSize + _padding);
					var y = _offsetY + _padding + row * (_tileSize + _padding);
					var isSelected = selected.HasValue && selected.Value.Row == row && selected.Value.Col == col;

					DrawTile(canvas, tile, x, y, _tileSize, isSelected);
				}
			}
		}

		// Update score
		ScoreLabel.Text = _gameBoard.Score.ToString();

		// Update operator button cooldown state
		UpdateOperatorButtons();
	}

	private void DrawEmptyCell(SKCanvas canvas, float x, float y, float size)
	{
		using var paint = new SKPaint
		{
			Color = SKColor.Parse("#CDC1B4"),
			IsAntialias = true,
			Style = SKPaintStyle.Fill
		};

		var rect = new SKRect(x, y, x + size, y + size);
		canvas.DrawRoundRect(rect, 8, 8, paint);
	}

	private void DrawTile(SKCanvas canvas, GameTile tile, float x, float y, float size, bool isSelected = false)
	{
		// Get color for this value
		var color = _tileColors.ContainsKey(tile.Value)
			? _tileColors[tile.Value]
			: SKColor.Parse("#3C3A32");

		// Scale animation for new/merged tiles
		var scale = 1.0f;
		if (tile.IsNew || tile.IsMerged)
		{
			var timeSinceUpdate = (DateTime.Now - _lastUpdate).TotalSeconds;
			if (timeSinceUpdate < 0.15)
			{
				scale = 0.8f + (float)(timeSinceUpdate / 0.15 * 0.2);
			}
		}

		canvas.Save();

		// Apply scale transform
		var centerX = x + size / 2;
		var centerY = y + size / 2;
		canvas.Translate(centerX, centerY);
		canvas.Scale(scale);
		canvas.Translate(-centerX, -centerY);

		var rect = new SKRect(x, y, x + size, y + size);

		// Draw selection highlight (glowing border)
		if (isSelected)
		{
			using var glowPaint = new SKPaint
			{
				Color = SKColor.Parse("#FFD700"),
				IsAntialias = true,
				Style = SKPaintStyle.Stroke,
				StrokeWidth = 6
			};
			var glowRect = new SKRect(x - 3, y - 3, x + size + 3, y + size + 3);
			canvas.DrawRoundRect(glowRect, 10, 10, glowPaint);
		}

		// Draw tile background
		using (var paint = new SKPaint
		{
			Color = color,
			IsAntialias = true,
			Style = SKPaintStyle.Fill
		})
		{
			canvas.DrawRoundRect(rect, 8, 8, paint);
		}

		// Draw decimal value (larger)
		var decimalTextColor = tile.Value <= 4 ? SKColor.Parse("#776E65") : SKColors.White;
		using (var textPaint = new SKPaint
		{
			Color = decimalTextColor,
			IsAntialias = true,
			TextSize = size * 0.4f,
			TextAlign = SKTextAlign.Center,
			Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
		})
		{
			var decimalText = tile.Value.ToString();
			var textY = centerY + textPaint.TextSize * 0.15f;
			canvas.DrawText(decimalText, centerX, textY, textPaint);
		}

		// Draw binary value (smaller, below)
		using (var binaryPaint = new SKPaint
		{
			Color = decimalTextColor.WithAlpha(180),
			IsAntialias = true,
			TextSize = size * 0.15f,
			TextAlign = SKTextAlign.Center,
			Typeface = SKTypeface.FromFamilyName("Courier New", SKFontStyle.Normal)
		})
		{
			var binaryText = tile.BinaryString;
			var binaryY = centerY + size * 0.35f;
			canvas.DrawText(binaryText, centerX, binaryY, binaryPaint);
		}

		canvas.Restore();
	}

	private void OnCanvasViewTouch(object? sender, SKTouchEventArgs e)
	{
		switch (e.ActionType)
		{
			case SKTouchAction.Pressed:
				_touchStartPoint = e.Location;
				e.Handled = true;
				break;

			case SKTouchAction.Released:
				if (_touchStartPoint.HasValue)
				{
					var delta = e.Location - _touchStartPoint.Value;
					var absX = Math.Abs(delta.X);
					var absY = Math.Abs(delta.Y);
					var distance = Math.Max(absX, absY);

					if (distance > MinSwipeDistance)
					{
						// Swipe detected - move tiles
						SwipeDirection direction;

						if (absX > absY)
						{
							direction = delta.X > 0 ? SwipeDirection.Right : SwipeDirection.Left;
						}
						else
						{
							direction = delta.Y > 0 ? SwipeDirection.Down : SwipeDirection.Up;
						}

						_gameBoard.ClearSelection();
						_lastUpdate = DateTime.Now;
						_gameBoard.Move(direction);
						GameCanvas.InvalidateSurface();
					}
					else if (distance < TapThreshold)
					{
						// Tap detected - select tile
						HandleTileTap(_touchStartPoint.Value);
					}

					_touchStartPoint = null;
				}
				e.Handled = true;
				break;
		}
	}

	private void HandleTileTap(SKPoint location)
	{
		// Convert touch location to grid coordinates
		var gridSize = 4;

		for (int row = 0; row < gridSize; row++)
		{
			for (int col = 0; col < gridSize; col++)
			{
				var x = _offsetX + _padding + col * (_tileSize + _padding);
				var y = _offsetY + _padding + row * (_tileSize + _padding);

				if (location.X >= x && location.X <= x + _tileSize &&
					location.Y >= y && location.Y <= y + _tileSize)
				{
					// Tapped on this cell
					var currentSelection = _gameBoard.SelectedTile;

					// Toggle selection: tap same tile to deselect
					if (currentSelection.HasValue &&
						currentSelection.Value.Row == row &&
						currentSelection.Value.Col == col)
					{
						_gameBoard.ClearSelection();
					}
					else
					{
						_gameBoard.SelectTile(row, col);
					}

					GameCanvas.InvalidateSurface();
					return;
				}
			}
		}

		// Tapped outside grid - clear selection
		_gameBoard.ClearSelection();
		GameCanvas.InvalidateSurface();
	}

	private void OnNewGameClicked(object? sender, EventArgs e)
	{
		_gameBoard.InitializeGame();
		_lastUpdate = DateTime.Now;
		GameCanvas.InvalidateSurface();
	}

	private void OnLeftShiftClicked(object? sender, EventArgs e)
	{
		if (_gameBoard.ApplyLeftShift())
		{
			_lastUpdate = DateTime.Now;
			GameCanvas.InvalidateSurface();
		}
	}

	private void OnRightShiftClicked(object? sender, EventArgs e)
	{
		if (_gameBoard.ApplyRightShift())
		{
			_lastUpdate = DateTime.Now;
			GameCanvas.InvalidateSurface();
		}
	}

	private void UpdateOperatorButtons()
	{
		var hasSelection = _gameBoard.SelectedTile.HasValue;
		var canUse = _gameBoard.CanUseOperator();
		var cooldownRemaining = _gameBoard.GetCooldownRemaining();

		// Update button states on UI thread
		MainThread.BeginInvokeOnMainThread(() =>
		{
			// Left shift button - enabled when tile selected, cooldown ready, and not at max
			var canLeftShift = hasSelection && canUse;
			if (hasSelection)
			{
				var grid = _gameBoard.GetGrid();
				var sel = _gameBoard.SelectedTile!.Value;
				var tile = grid[sel.Row, sel.Col];
				if (tile != null && tile.Value >= 2048)
					canLeftShift = false;
			}

			LeftShiftButton.IsEnabled = canLeftShift;
			LeftShiftButton.Text = canUse ? "<< x2" : $"<< ({cooldownRemaining:F0}s)";

			// Right shift button - enabled when tile selected, cooldown ready, and not at min
			var canRightShift = hasSelection && canUse;
			if (hasSelection)
			{
				var grid = _gameBoard.GetGrid();
				var sel = _gameBoard.SelectedTile!.Value;
				var tile = grid[sel.Row, sel.Col];
				if (tile != null && tile.Value <= 2)
					canRightShift = false;
			}

			RightShiftButton.IsEnabled = canRightShift;
			RightShiftButton.Text = canUse ? ">> /2" : $">> ({cooldownRemaining:F0}s)";
		});
	}
}
