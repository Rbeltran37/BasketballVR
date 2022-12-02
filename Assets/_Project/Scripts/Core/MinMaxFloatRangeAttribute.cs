using System;

public class MinMaxFloatRangeAttribute : Attribute
{
    public MinMaxFloatRangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float Min { get; private set; }
    public float Max { get; private set; }
}