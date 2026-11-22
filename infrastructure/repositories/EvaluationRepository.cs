using POS.domain.entities;
using POS.domain.interfaces;
using POS.domain.valueobjects;

namespace POS.infrastructure.repositories;

public class EvaluationRepository : IEvaluationRepository
{
    private readonly string _path;

    private class EvaluationData
    {
        public Guid StudentId {get; set;}
        public Guid CriterionId {get; set;}
        public float Score {get; set;}
        public DateTime Date {get; set;}
        public string Comment {get; set;} = "";
    }

    public EvaluationRepository(string path = "evaluations.json")
    {
        _path = path;
    }

    private List<Evaluation>? Load()
    {
        var data = JsonFileStorage.Read<List<EvaluationData>>(_path);

        if (data == null)
            return File.Exists(_path) ? null : [];

        List<Evaluation> evaluations = [];

        foreach (var item in data)
        {
            var evaluation = Evaluation.Create(item.StudentId, item.CriterionId, item.Score,
                Score.Create(float.MaxValue)!, item.Date, item.Comment);

            if (evaluation == null)
                return null;

            evaluations.Add(evaluation);
        }

        return evaluations;
    }

    private bool Save(List<Evaluation> evaluations)
    {
        var data = evaluations.Select(e => 
        new EvaluationData{StudentId = e.Id.StudentId, CriterionId = e.Id.CriterionId, 
            Score = e.Score.Value, Date = e.Date, Comment = e.Comment}).ToList();

        return JsonFileStorage.Write(_path, data);
    }

    public Evaluation? GetById(Evaluation.EvaluationId id)
    {
        var evaluations = Load();

        return evaluations?.FirstOrDefault(e =>
            e.Id.StudentId == id.StudentId && e.Id.CriterionId == id.CriterionId);
    }

    public List<Evaluation>? GetByStudent(Guid student)
    {
        var evaluations = Load();
        return evaluations?.Where(e => e.Id.StudentId == student).ToList();
    }

    public List<Evaluation>? GetAll()
    {
        return Load();
    }

    public bool Add(Evaluation eval)
    {
        var evaluations = Load();
        if (evaluations == null || evaluations.Any(e =>
            e.Id.StudentId == eval.Id.StudentId && e.Id.CriterionId == eval.Id.CriterionId))
            return false;

        evaluations.Add(eval);
        return Save(evaluations);
    }

    public bool Update(Evaluation eval)
    {
        var evaluations = Load();
        if (evaluations == null)
            return false;

        int index = evaluations.FindIndex(e =>
            e.Id.StudentId == eval.Id.StudentId && e.Id.CriterionId == eval.Id.CriterionId);

        if (index == -1)
            return false;

        evaluations[index] = eval;
        return Save(evaluations);
    }

    public bool Delete(Evaluation.EvaluationId id)
    {
        var evaluations = Load();
        if (evaluations == null)
            return false;

        var evaluation = evaluations.FirstOrDefault(e =>
            e.Id.StudentId == id.StudentId && e.Id.CriterionId == id.CriterionId);

        if (evaluation == null)
            return false;

        evaluations.Remove(evaluation);
        return Save(evaluations);
    }
}
