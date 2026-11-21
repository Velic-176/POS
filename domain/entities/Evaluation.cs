using POS.domain.valueobjects;

namespace POS.domain.entities;

public class Evaluation
{
    public struct EvaluationId
    {
        public Guid StudentId;
        public Guid CriterionId;
    }
    public readonly EvaluationId Id;
    // public readonly Guid StudentId;
    // public readonly Guid CriterionId;
    public Score Score{get; private set;}
    public DateTime Date{get; private set;}
    public string Comment{get; private set;}

    Evaluation(Guid student, Guid criterion, Score score, DateTime date, string comment)
    {
        Id.StudentId = student;
        Id.CriterionId = criterion;
        Score = score;
        Date = date;
        Comment = comment;
    }

    public static Evaluation? Create(Guid student, Guid criterion, float score, Score max_score, DateTime date, string comment)
    {
        if (student == Guid.Empty || criterion == Guid.Empty || score > max_score.Value || date > DateTime.Now)
            return null;

        Score? s = Score.Create(score);
        if (s == null)
            return null;

        return new Evaluation(student, criterion, s, date, comment);
    }

    public bool ChangeScore(float value)
    {
        Score? a = Score.Create(value);
        if (a == null)
            return false;

        Date = DateTime.Now;
        Score = a;
        return true;
    }

    public bool ChangeScore(Score value)
    {
        if (value == null)
            return false;

        Date = DateTime.Now;
        Score = value;
        return true;
    }

    public bool ChangeComment(string new_comment)
    {
        Comment = new_comment;
        return true;
    }
}