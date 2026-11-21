namespace POS.domain.valueobjects;
public class Weight
{
    public float Value{get;}
    Weight(float value)
    {
        Value = value;
    }

    public static Weight? Create(float value)
    {
        if (value < 0 || value > 1) return null;
        return new Weight(value);
    }
} 
