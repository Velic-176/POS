using POS.domain.valueobjects;
namespace POS.domain.entities;
public class Criterion
{
    public readonly Guid Id;
    public string Name{get; private set;}
    public Score MaxScore{get; private set;}
    public Weight Weight{get; private set;}

    private Criterion(Guid id, string name, Score max_score, Weight weight)
    {
        Id = id;
        Name = name;
        MaxScore = max_score;
        Weight = weight;
    }

    static public Criterion? Create(Guid id, string name, float max_score, float weight)
    {
        if (name == null || name.Length > 500 || id == Guid.Empty || !IsNamevalid(name)) return null;


        Score? m_s = Score.Create(max_score);
        Weight? w = Weight.Create(weight);
        if (m_s == null || w == null)
            return null;

        return new Criterion(id, name, m_s, w);
    }

    public static bool IsNamevalid(string name)
    {
        if (name.Length > 50)
            return false;

        foreach (char c in name)
        {
            if (!char.IsLetter(c) && !char.IsDigit(c) && !"., ".Contains(c))
                return false;
        }
        return true;
    }

    public bool ChangeName(string name)
    {
        if (!IsNamevalid(name)) return false;
           
        Name = name;
        return true;
    }

    public bool ChangeMaxScore(float val)
    {
        Score? a = Score.Create(val);
        if (a == null)
            return false;

        MaxScore = a;
        return true;
    }

    public bool ChangeWeight(float val)
    {
        Weight? a = Weight.Create(val);
        if (a == null)
            return false;

        Weight = a;
        return true;
    }
}