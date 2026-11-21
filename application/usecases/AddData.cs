using System.Runtime.ExceptionServices;
using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.application.usecases;

public class AddStudentCase
{
    public enum Result
    {
        Success,
        InvalidArgument,
        DataBaseError,
    }
    private readonly IStudentRepository _students_repo;

    public AddStudentCase(IStudentRepository students)
    {
        _students_repo = students;
    }

    public Result Add(string name)
    {
        Guid id;
        do {
        id = Guid.NewGuid();
        } while (_students_repo.GetById(id) != null);

        Student? st = Student.Create(id, name);
        if (st == null)
            return Result.InvalidArgument;

        if (!_students_repo.Add(st))
            return Result.DataBaseError;

        return Result.Success;
    }
}

public class AddCriterionCase
{
    public enum Result
    {
        Success,
        InvalidArgument,
        DataBaseError,
        AlreadyExists,
    }
    private readonly IRubricatorRepository _rubricator_repo;

    public AddCriterionCase(IRubricatorRepository criteria)
    {
        _rubricator_repo = criteria;
    }

    public Result Add(string name, float max_score, float weight)
    {
        Guid id;
        do {
        id = Guid.NewGuid();
        } while (_rubricator_repo.GetById(id) != null);

        // создание критерия 
        Criterion? cr = Criterion.Create(id, name, max_score, weight);
        if (cr == null) 
            return Result.InvalidArgument;

        // получение рубрикатора
        Rubricator? rubricator = _rubricator_repo.GetRub();
        if (rubricator == null)
            return Result.DataBaseError;

        // обновление рубрикатора
        var res = rubricator.AddCriterion(cr);
        if(res != Rubricator.Result.Success)
        {
            if (res == Rubricator.Result.CriterionAlreadyExists)
                return Result.AlreadyExists;
            if (res == Rubricator.Result.InvalidArgument)
                return Result.InvalidArgument;
        }

        // обновление бд
        if (!_rubricator_repo.Update(rubricator))
            return Result.DataBaseError;

        return Result.Success;
    }
}

public class AddEvaluationCase
{
    public enum Result
    {
        Success,
        InvalidArgument,
        DataBaseError,
        AlreadyExists,
        DoNotExist,
    }
    private readonly IEvaluationRepository _evaluations_repo;
    private readonly IStudentRepository _students_repo;
    private readonly IRubricatorRepository _rubricator_repo;

    public AddEvaluationCase(IEvaluationRepository evaluations, IStudentRepository students, IRubricatorRepository criteria)
    {
        _evaluations_repo = evaluations;
        _students_repo = students;
        _rubricator_repo = criteria;
    }

    public Result Add(Guid student, Guid criterion, float score, string comment)
    {
        // проверка на существование студента
        if (_students_repo.GetById(student) == null) 
            return Result.DoNotExist;

        // проверка на существование критерия
        var cr = _rubricator_repo.GetById(criterion);
        if (cr == null) 
            return Result.DoNotExist;

        // проверка на существование оценки у этого студента по этому критерию
        var existed = _evaluations_repo.GetByStudent(student);
        if (existed == null)
            return Result.DataBaseError;

        foreach (var evaluation in existed)
        {
            if (evaluation.Id.CriterionId == criterion)
                return Result.AlreadyExists;
        }

        DateTime dt = DateTime.Now;
        // создание объекта оценки
        var eval = Evaluation.Create(student, criterion, score, cr.MaxScore, dt, comment);
        if (eval == null)
            return Result.InvalidArgument;

        if (!_evaluations_repo.Add(eval))
            return Result.DataBaseError;

        return Result.Success;
    }
}