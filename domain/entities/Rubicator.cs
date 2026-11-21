// using System.Numerics;
using POS.application.usecases;
using POS.domain.valueobjects;
namespace POS.domain.entities;

public class Rubricator
{
    public enum Result
    {
        Success,
        InvalidArgument,
        CriterionAlreadyExists,
        CriteriaNotFound
    }

    public int Count{get; private set;}
    public Dictionary<string, Criterion> Criteria{get; private set;} // {название : Criterion}
    public Result Res{get; private set;}

    public Rubricator()
    {
        Criteria = [];
        Count = 0;
        Res = Result.Success;
    }

    // для импорта уже существующих данных
    // при дубликатах сохраняет первое вхождение
    public Rubricator(List<Criterion> criteria)
    {
        Criteria= [];
        Count = 0;
        if (criteria != null)
        {
            foreach (Criterion criterion in criteria)
            {
                if (criterion == null) continue;
                if (Criteria.TryAdd(criterion.Name, criterion)) ++Count;
            }
        }
    }

    // проверяет согласованность с остальными критериями рубрикатора
    public Result AddCriterion(Criterion criterion)
    {   
        // проверка на уникальность
        bool res = Criteria.TryAdd(criterion.Name, criterion);
        if (!res)
            return Result.CriterionAlreadyExists;

        ++Count;
        return Result.Success;
    }

    public Result RemoveCriterion(Guid id)
    {
        foreach(var criterion in Criteria.Values)
        {
            if (criterion.Id == id)
            {
                if (!Criteria.Remove(criterion.Name)) 
                    return Result.CriteriaNotFound;
                --Count;
                return Result.Success;
            }
        }
        return Result.CriteriaNotFound;
    }

    public bool Contains(Guid id)
    {
        foreach(var criterion in Criteria.Values)
        {
            if (criterion.Id == id)
                return true;
        }
        return false;
    }

    public Criterion? GetById(Guid id)
    {
        foreach(var criterion in Criteria.Values)
        {
            if (criterion.Id == id)
            {
                Res = Result.Success;
                return criterion;   
            }
        }
        Res = Result.CriteriaNotFound;
        return null;
    }
}