using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.application.usecases;

public class ChangeStudentCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist,
        InvalidArg
    }
    private readonly IStudentRepository _students_repo;

    public ChangeStudentCase(IStudentRepository students)
    {
        _students_repo = students;
    }

    public Result Change(Guid student, string name)
    {
        var st = _students_repo.GetById(student);
        if (st == null)
            return Result.DoNotExist;

        if (!st.ChangeName(name))
            return Result.InvalidArg;

        if (!_students_repo.Update(st))
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
            Result.InvalidArg => "InvalidArg",
            _ => "UnknownError",
        };
    }
}

public class ChangeCriterionCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist,
        InvalidArg
    }
    private readonly IRubricatorRepository _rubricator_repo;
    private readonly IEvaluationRepository _evaluations_repo;

    public ChangeCriterionCase(IRubricatorRepository rubr, IEvaluationRepository evaluations)
    {
        _rubricator_repo = rubr;
        _evaluations_repo = evaluations;
    }

    public Result Change(Guid criterion, string name, float max_score, float weight)
    {
        var rubricator = _rubricator_repo.GetRub();
        if (rubricator == null)
            return Result.DoNotExist;

        var cr = rubricator.GetById(criterion);
        if (cr == null)
            return Result.DataBaseError;

        var old_max = cr.MaxScore.Value;
        if(!cr.ChangeName(name) || !cr.ChangeMaxScore(max_score) || !cr.ChangeWeight(weight))
            return Result.InvalidArg;

        if (!_rubricator_repo.Update(rubricator))
            return Result.DataBaseError;

        // если макс балл опустился, то все оценки -> min(new_max, value)
        if (old_max > max_score)
        {
            var evals = _evaluations_repo.GetAll();
            if (evals == null)
            return Result.DataBaseError;

            foreach (var eval in evals)
            {
                if (eval.Id.CriterionId == criterion)
                {
                    eval.ChangeScore(Math.Min(eval.Score.Value, max_score));
                    if (!_evaluations_repo.Update(eval))
                        // TODO: согласовать бд. механизм транзакций
                        return Result.DataBaseError;
                }
            }
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
            Result.InvalidArg => "InvalidArg",
            _ => "UnknownError",
        };
    }
}

public class ChangeEvaluationCase
{
    public enum Result
    {
        Success,
        DataBaseError,
        DoNotExist,
        InvalidArg
    }
    private readonly IRubricatorRepository _rubricator_repo;
    private readonly IEvaluationRepository _evaluations_repo;

    public ChangeEvaluationCase(IRubricatorRepository rubr, IEvaluationRepository evaluations)
    {
        _rubricator_repo = rubr;
        _evaluations_repo = evaluations;
    }

    public Result Change(Evaluation.EvaluationId id, float score, string comment)
    {
        var eval = _evaluations_repo.GetById(id);
        if (eval == null)
            return Result.DoNotExist;

        var criterion = _rubricator_repo.GetById(id.CriterionId);
        if (criterion == null)
            return Result.DoNotExist;

        if (score > criterion.MaxScore.Value)
            return Result.InvalidArg;

        if (!eval.ChangeScore(score) || !eval.ChangeComment(comment))
            return Result.InvalidArg;

        if (!_evaluations_repo.Update(eval))
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
            Result.InvalidArg => "InvalidArg",
            _ => "UnknownError",
        };
    }
}