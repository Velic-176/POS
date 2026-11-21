namespace POS.domain.valueobjects;
public class Score
{
    public float Value{get; }
    Score(float value)
    {
        Value = value;
    }

    public static Score? Create(float value)
    {
        if (value < 0) return null;
        return new Score(value);
    }
} 
