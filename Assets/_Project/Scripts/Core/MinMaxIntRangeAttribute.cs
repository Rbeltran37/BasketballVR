using System;

public class MinMaxIntRangeAttribute : Attribute
{
    public MinMaxIntRangeAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int Min { get; private set; }
    public int Max { get; private set; }
}