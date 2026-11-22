using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.application.usecases;

public class DeleteStudentCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist
    }
    // public Result Res{get; private set;}
    private readonly IStudentRepository _students_repo;
    private readonly DeleteEvaluationCase _delete_evals;

    public DeleteStudentCase(IStudentRepository students, DeleteEvaluationCase delete)
    {
        _students_repo = students;
        _delete_evals = delete;
        // Res = Result.Success;
    }

    public Result DeleteById(Guid id)
    {
        var res = _delete_evals.DeleteByStudent(id);
        if (res != DeleteEvaluationCase.Result.Success)
        {
            // TODO: согласовать бд. механизм транзакций
            if (res == DeleteEvaluationCase.Result.DoNotExist)
                return Result.DoNotExist;
            return Result.DataBaseError; 
        }

        if(!_students_repo.Delete(id))
            return Result.DataBaseError;
        
        return Result.Success;
    }

    public string GetError(Result res)
    {
        return res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.DoNotExist => "DoNotExist",
            _ => "UnknownError",
        };
    }
}

public class DeleteCriterionCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        CriterionDoesNotExist,
        EvaluationDoesNotExist
    }

    private readonly IRubricatorRepository _rubricator_repo;
    private readonly IEvaluationRepository _evaluations_repo;
    private readonly DeleteEvaluationCase _delete_evals;

    public DeleteCriterionCase(IRubricatorRepository repo, IEvaluationRepository evaluations, DeleteEvaluationCase delete)
    {
        _rubricator_repo = repo;
        _evaluations_repo = evaluations;
        _delete_evals = delete;
    }

    public Result DeleteById(Guid id)
    {
        var rubricator = _rubricator_repo.GetRub();
        if (rubricator == null)
            return Result.DataBaseError;

        if (!rubricator.Contains(id))
            return Result.CriterionDoesNotExist;

        var evals = _evaluations_repo.GetAll();
        if (evals == null)
            return Result.DataBaseError;

        foreach (var eval in evals)
        {
            if (eval.Id.CriterionId == id)
            {
                var res = _delete_evals.DeleteById(eval.Id);
                if (res != DeleteEvaluationCase.Result.Success)
                {
                    // TODO: согласовать бд. механизм транзакций
                    return Result.EvaluationDoesNotExist;
                }
            }
        }

        rubricator.RemoveCriterion(id);
        if (!_rubricator_repo.Update(rubricator))
        {
            // TODO: согласовать бд. механизм транзакций
            return Result.DataBaseError;
        }

        return Result.Success;
    }

    public string GetError(Result res)
    {
        return res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.CriterionDoesNotExist => "CriterionDoesNotExist",
            Result.EvaluationDoesNotExist => "EvaluationDoesNotExist",
            _ => "UnknownError",
        };
    }
}

public class DeleteEvaluationCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist,
    }
    private readonly IEvaluationRepository _evaluations_repo;
    private readonly IStudentRepository _students_repo;

    public DeleteEvaluationCase(IEvaluationRepository evaluations, IStudentRepository students)
    {
        _evaluations_repo = evaluations;
        _students_repo = students;
    }

    public Result DeleteById(Evaluation.EvaluationId id)
    {
        if(!_evaluations_repo.Delete(id))
            return Result.DoNotExist;

        return Result.Success;
    }

    public Result DeleteByStudent(Guid student)
    {
        if (_students_repo.GetById(student) == null)
        {
            return Result.DoNotExist;
        }

        var evals = _evaluations_repo.GetByStudent(student);
        if (evals == null)
            return Result.DataBaseError;
        if (evals.Count == 0)
            return Result.Success;
        
        foreach(var eval in evals)
        {
            DeleteById(eval.Id);
            // TODO: согласовать бд. механизм транзакций
        }

        return Result.Success;
    }

    public string GetError(Result res)
    {
        return res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.DoNotExist => "DoNotExist",
            _ => "UnknownError",
        };
    }
}