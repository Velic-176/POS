using POS.application.models;
using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.application.usecases;

public class GetStudentsCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist
    }
    public Result Res{get; private set;}
    private readonly IStudentRepository _students_repo;

    public GetStudentsCase(IStudentRepository students)
    {
        _students_repo = students;
        Res = Result.Success;
    }

    public List<Student>? GetAll()
    {
        var list = _students_repo.GetAll();
        if (list == null)
        {
            Res = Result.DataBaseError;
            return null;
        }
        Res = Result.Success;
        return list;
    }

    public Student? GetById(Guid student)
    {
        var st = _students_repo.GetById(student);
        if (st == null)
        {
            Res = Result.DoNotExist;
            return null;
        }
        Res = Result.Success;
        return st;
    }

    public string GetError()
    {
        return Res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.DoNotExist => "DoNotExist",
            _ => "UnknownError",
        };
    }
}

public class GetCriteriaCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist
    }
    public Result Res{get; private set;}
    private readonly IRubricatorRepository _rubricator_repo;

    public GetCriteriaCase(IRubricatorRepository repo)
    {
        _rubricator_repo = repo;
        Res = Result.Success;
    }

    public List<Criterion>? GetAll()
    {
        var rubricator = _rubricator_repo.GetRub();
        if (rubricator == null)
        {
            Res = Result.DataBaseError;
            return null;
        }
        
        List<Criterion> list = [];
        foreach(var pair in rubricator.Criteria)
            list.Add(pair.Value);

        Res = Result.Success;
        return list;
    }

    public Criterion? GetByName(string name)
    {
        var rubricator = _rubricator_repo.GetRub();
        if (rubricator == null)
        {
            Res = Result.DataBaseError;
            return null;
        }
        
        Criterion? cr = rubricator.GetByName(name);
        if (cr == null)
        {
            Res = Result.DoNotExist;
            return null;
        }
        

        Res = Result.Success;
        return cr;
    }

    public string GetError()
    {
        return Res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.DoNotExist => "DoNotExist",
            _ => "UnknownError",
        };
    }
}

public class GetEvaluationsCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist,
    }
    public Result Res{get; private set;}
    private readonly IEvaluationRepository _evaluation_repo;
    private readonly IStudentRepository _students_repo;
    private readonly IRubricatorRepository _rubricator_repo;

    public GetEvaluationsCase(IEvaluationRepository evals, IStudentRepository students, IRubricatorRepository rubr)
    {
        _evaluation_repo = evals;
        _students_repo = students;
        _rubricator_repo = rubr;
        Res = Result.Success;
    }

    public List<EvaluationInfo>? Get(Guid student)
    {
        if (_students_repo.GetById(student) == null)
        {
            Res = Result.DoNotExist;
            return null;
        }

        var evals = _evaluation_repo.GetByStudent(student); // все оценки данного студента
        var rubricator = _rubricator_repo.GetRub();
        if (evals == null || rubricator == null)
        {
            Res = Result.DataBaseError;
            return null;
        }

        // парсинг данных
        //criterionName, Score, MaxScore, Weight, Date, Comment
        List<EvaluationInfo> list = [];

        var evaluationsByCriterion = evals.ToDictionary(eval => eval.Id.CriterionId);
        foreach(var cr in rubricator.Criteria.Values)
        {
            Evaluation? curr = null;   
            if (!evaluationsByCriterion.TryGetValue(cr.Id, out curr)) // если оценки по критерию нет, то это отобразится
            {
                list.Add(new(CriterionId:cr.Id, CriterionName:cr.Name, MaxScore:cr.MaxScore.Value, Weight:cr.Weight.Value));
            }
            else
                list.Add(new(CriterionId:cr.Id, CriterionName:cr.Name, Score:curr.Score.Value, MaxScore:cr.MaxScore.Value, 
                             Weight:cr.Weight.Value, Date:curr.Date, Comment:curr.Comment));
        }
        Res = Result.Success;
        return list;
    }

    public Evaluation? GetById(Evaluation.EvaluationId id)
    {
        var eval = _evaluation_repo.GetById(id);
        if (eval == null)
        {
            Res = Result.DoNotExist;
            return null;
        }
        Res = Result.Success;
        return eval;
    }

    public string GetError()
    {
        return Res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.DoNotExist => "DoNotExist",
            _ => "UnknownError",
        };
    }
}