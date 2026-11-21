using POS.domain.entities;
using POS.domain.interfaces;
using POS.domain.valueobjects;
namespace POS.domain.services;

public class ResultEvalCalcService
{
    public enum Result
    {
        Success,
        StudentDoNotExist,
        InvalidScore,
        CriterionDoNotExist,
        InvalidStrategy
    }
    static readonly int NUM_OF_STRATEGIES = 2;
    
    public int Strategy{get; private set;}
    public Result Res{get; private set;}

    public ResultEvalCalcService(int strategy)
    {
        Strategy = strategy;
        Res = Result.Success;
    }

    public bool ChangeStrategy(int strategy)
    {
        if (strategy > NUM_OF_STRATEGIES || strategy < 1)
        { 
            Res = Result.InvalidStrategy; 
            return false;
        }

        Strategy = strategy;
        Res = Result.Success;
        return true;
    }

    public Score? Evaluate(List<Tuple<float, float, float>> evals)
    {

        Score? res;
        switch(Strategy)
        {
        case(1): res = EvaluationStategyV1(evals); break;
        case(2): res = EvaluationStategyV2(evals); break;
        default: Res = Result.InvalidStrategy; return null;
        }

        if (res != null && res.Value <= 10)
        { 
            Res = Result.Success;
            return res;
        }
        Res = Result.InvalidScore;
        return null;
    }

    // нормированный балл с учётом веса каждой критерии с округлением итога
    private Score? EvaluationStategyV1(List<Tuple<float, float, float>> evals)
    {
        float total_weight = 0;
        float sum = 0;
        foreach(var eval in evals)
        {
            total_weight += eval.Item3;
            sum += eval.Item1 / eval.Item2 * eval.Item3;
        }
        sum /= total_weight;
        Score? s_sum = Score.Create((float)Math.Round(sum, 1));
        if (s_sum == null)
        {
            Res = Result.InvalidScore;
            return null;
        }
        Res = Result.Success;
        return s_sum;
    }

    // округление каждого балла до суммирования
    private Score? EvaluationStategyV2(List<Tuple<float, float, float>> evals)
    {
        float total_weight = 0;
        float sum = 0;
        foreach(var eval in evals)
        {
            total_weight += eval.Item3;
            sum += (float)Math.Round(eval.Item1 / eval.Item2 * eval.Item3, 1);
        }
        sum /= total_weight;
        Score? s_sum = Score.Create(sum);
        if (s_sum == null)
        {
            Res = Result.InvalidScore;
            return null;
        }
        Res = Result.Success;
        return s_sum;
    }
}