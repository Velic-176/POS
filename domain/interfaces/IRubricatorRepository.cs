using POS.domain.entities;
namespace POS.domain.interfaces;

public interface IRubricatorRepository
{
    Rubricator? GetRub();
    Criterion? GetById(Guid id);
    List<Criterion>? GetAll();
    bool Update(Rubricator rb);
}