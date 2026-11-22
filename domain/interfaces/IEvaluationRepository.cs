using POS.domain.entities;
namespace POS.domain.interfaces;

public interface IEvaluationRepository
{
    Evaluation? GetById(Evaluation.EvaluationId id);
    List<Evaluation>? GetByStudent(Guid Student);
    List<Evaluation>? GetAll();
    bool Add(Evaluation eval);
    bool Update(Evaluation eval);
    bool Delete(Evaluation.EvaluationId id);
}