using POS.domain.entities;
namespace POS.domain.interfaces;

public interface IStudentRepository
{
    Student? GetById(Guid id);
    List<Student>? GetAll();
    bool Add(Student student);
    bool Update(Student student);
    bool Delete(Guid id);
}