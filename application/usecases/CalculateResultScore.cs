using POS.domain.interfaces;
using POS.domain.entities;
using POS.domain.valueobjects;
using POS.domain.services;

namespace POS.application.usecases;

public class CalculateResultScore
{
    public enum Result
    {
        Success,
        StudentDoNotExist,
        InvalidScore,
        EvalOnUnknownCriterion,
        InvalidStrategy,
        UnknownError,
        DataBaseError
    }
    private readonly IEvaluationRepository _evaluations_repo;
    private readonly IStudentRepository _students_repo;
    private readonly IRubricatorRepository _rubricator_repo;
    private readonly ResultEvalCalcService _calculator;
    public Result Res{get; private set;}

    public CalculateResultScore(ResultEvalCalcService calc, IEvaluationRepository evaluations, IStudentRepository students, IRubricatorRepository criteria)
    {
        _evaluations_repo = evaluations;
        _students_repo = students;
        _rubricator_repo = criteria;
        Res = Result.Success;
        _calculator = calc;
    }

    public Score? Evaluate(Guid student)
    {
        // проверка студента на существование
        if (_students_repo.GetById(student) == null) 
        {
            Res = Result.StudentDoNotExist;
            return null;
        }

        List<Evaluation>? evaluations = _evaluations_repo.GetByStudent(student);
        List<Criterion>? criteria = _rubricator_repo.GetAll();
        if (evaluations == null || criteria == null)
        {
            Res = Result.DataBaseError;
            return null;
        }

        // проверка корректности выставленных оценок (есть ли баллы по несущ. критериям)
        if (evaluations.Count > criteria.Count)
        {
            Res = Result.EvalOnUnknownCriterion;
            return null;
        }

        // преобразование данных к нужному виду для вычисления итогового балла
        List<Tuple<float, float, float>>? evals = ConvertToTuple(evaluations, criteria);
        if (evals == null)
        {
            Res = Result.UnknownError;
            return null;
        }

        Score? score = _calculator.Evaluate(evals);
        if (score == null)
        {
            if (_calculator.Res == ResultEvalCalcService.Result.InvalidScore)
            {
                Res = Result.InvalidScore;
                return null;
            }
            if (_calculator.Res == ResultEvalCalcService.Result.InvalidStrategy)
            {
                Res = Result.InvalidStrategy;
                return null;
            }
        }
        return score;
    }

    // score, max_score, weight
    static List<Tuple<float, float, float>> ConvertToTuple(List<Evaluation> evaluations, List<Criterion> criteria)
    {
        List<Tuple<float, float, float>> res = [];
        foreach (Criterion criterion in criteria)
        {
            foreach (Evaluation eval in evaluations)
            {
                if (criterion.Id == eval.Id.CriterionId)
                {
                    res.Add(new(eval.Score.Value, criterion.MaxScore.Value, criterion.Weight.Value));
                    evaluations.Remove(eval);
                    break;
                }
            }

            res.Add(new(0f, criterion.MaxScore.Value, criterion.Weight.Value));
        }

        return res;
    }
}