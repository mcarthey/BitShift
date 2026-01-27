namespace BitShift.Models;

public class GameTile
{
    public int Value { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsNew { get; set; }
    public bool IsMerged { get; set; }

    public string BinaryString => Convert.ToString(Value, 2).PadLeft(8, '0');
    
    public GameTile(int value, int row, int column)
    {
        Value = value;
        Row = row;
        Column = column;
        IsNew = true;
    }

    public GameTile Clone()
    {
        return new GameTile(Value, Row, Column)
        {
            IsNew = IsNew,
            IsMerged = IsMerged
        };
    }
}
