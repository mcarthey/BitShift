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
		var padding = 10f;
		
		// Calculate tile size to fit the canvas
		var availableSize = Math.Min(info.Width, info.Height);
		var tileSize = (availableSize - (padding * (gridSize + 1))) / gridSize;
		
		// Center the grid
		var offsetX = (info.Width - availableSize) / 2;
		var offsetY = (info.Height - availableSize) / 2;

		// Draw empty grid cells
		for (int row = 0; row < gridSize; row++)
		{
			for (int col = 0; col < gridSize; col++)
			{
				var x = offsetX + padding + col * (tileSize + padding);
				var y = offsetY + padding + row * (tileSize + padding);
				
				DrawEmptyCell(canvas, x, y, tileSize);
			}
		}

		// Draw tiles
		var grid = _gameBoard.GetGrid();
		for (int row = 0; row < gridSize; row++)
		{
			for (int col = 0; col < gridSize; col++)
			{
				var tile = grid[row, col];
				if (tile != null)
				{
					var x = offsetX + padding + col * (tileSize + padding);
					var y = offsetY + padding + row * (tileSize + padding);
					
					DrawTile(canvas, tile, x, y, tileSize);
				}
			}
		}

		// Update score
		ScoreLabel.Text = _gameBoard.Score.ToString();
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

	private void DrawTile(SKCanvas canvas, GameTile tile, float x, float y, float size)
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

		// Draw tile background
		using (var paint = new SKPaint
		{
			Color = color,
			IsAntialias = true,
			Style = SKPaintStyle.Fill
		})
		{
			var rect = new SKRect(x, y, x + size, y + size);
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

					if (Math.Max(absX, absY) > MinSwipeDistance)
					{
						SwipeDirection direction;
						
						if (absX > absY)
						{
							direction = delta.X > 0 ? SwipeDirection.Right : SwipeDirection.Left;
						}
						else
						{
							direction = delta.Y > 0 ? SwipeDirection.Down : SwipeDirection.Up;
						}

						_lastUpdate = DateTime.Now;
						_gameBoard.Move(direction);
						GameCanvas.InvalidateSurface();
					}

					_touchStartPoint = null;
				}
				e.Handled = true;
				break;
		}
	}

	private void OnNewGameClicked(object? sender, EventArgs e)
	{
		_gameBoard.InitializeGame();
		_lastUpdate = DateTime.Now;
		GameCanvas.InvalidateSurface();
	}

	private void OnOperatorClicked(object? sender, EventArgs e)
	{
		// TODO: Implement bit shift operator logic
		// This would allow player to apply left/right shift to selected tile
	}
}
