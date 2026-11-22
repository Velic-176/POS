using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.infrastructure.repositories;

public class RubricatorRepository : IRubricatorRepository
{
    private readonly string _path;

    private class CriterionData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public float MaxScore { get; set; }
        public float Weight { get; set; }
    }

    public RubricatorRepository(string path = "rubricator.json")
    {
        _path = path;
    }

    private Rubricator? Load()
    {
        var data = JsonFileStorage.Read<List<CriterionData>>(_path);

        if (data == null)
            return File.Exists(_path) ? null : new Rubricator();

        List<Criterion> criteria = [];

        foreach (var item in data)
        {
            var criterion = Criterion.Create(item.Id, item.Name, item.MaxScore, item.Weight);

            if (criterion == null)
                return null;

            criteria.Add(criterion);
        }

        return new Rubricator(criteria);
    }

    private bool Save(Rubricator rubricator)
    {
        var data = rubricator.Criteria.Values.Select(c => 
            new CriterionData{Id = c.Id, Name = c.Name, MaxScore = c.MaxScore.Value, 
            Weight = c.Weight.Value}).ToList();

        return JsonFileStorage.Write(_path, data);
    }

    public Rubricator? GetRub()
    {
        return Load();
    }

    public Criterion? GetById(Guid id)
    {
        var rubricator = Load();
        return rubricator?.GetById(id);
    }

    public List<Criterion>? GetAll()
    {
        var rubricator = Load();
        return rubricator?.Criteria.Values.ToList();
    }

    public bool Update(Rubricator rb)
    {
        return Save(rb);
    }
}
