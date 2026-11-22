using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.infrastructure.repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _path;

    private class StudentData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

    public StudentRepository(string path = "students.json")
    {
        _path = path;
    }

    private List<Student>? Load()
    {
        var data = JsonFileStorage.Read<List<StudentData>>(_path);

        if (data == null)
            return File.Exists(_path) ? null : [];

        List<Student> students = [];

        foreach (var item in data)
        {
            var student = Student.Create(item.Id, item.Name);
            if (student == null)
                return null;

            students.Add(student);
        }

        return students;
    }

    private bool Save(List<Student> students)
    {
        var data = students.Select(s => new StudentData{Id = s.Id, Name = s.Name}).ToList();

        return JsonFileStorage.Write(_path, data);
    }

    public Student? GetById(Guid id)
    {
        var students = Load();
        return students?.FirstOrDefault(s => s.Id == id);
    }

    public List<Student>? GetAll()
    {
        return Load();
    }

    public bool Add(Student student)
    {
        var students = Load();
        if (students == null || students.Any(s => s.Id == student.Id))
            return false;

        students.Add(student);
        return Save(students);
    }

    public bool Update(Student student)
    {
        var students = Load();
        if (students == null)
            return false;

        int index = students.FindIndex(s => s.Id == student.Id);
        if (index == -1)
            return false;

        students[index] = student;
        return Save(students);
    }

    public bool Delete(Guid id)
    {
        var students = Load();
        if (students == null)
            return false;

        var student = students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            return false;

        students.Remove(student);
        return Save(students);
    }
}
