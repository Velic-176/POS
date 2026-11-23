using POS.application.interfaces;
using POS.application.models;
using POS.domain.entities;
using POS.domain.interfaces;

namespace POS.application.usecases;

public class ImportEvaluationsCase
{
    private readonly IEvaluationRepository _evaluations_repo;
    private readonly IStudentRepository _students_repo;
    private readonly IRubricatorRepository _rubricator_repo;
    private readonly IEvaluationFactory _factory;

    public ImportEvaluationsCase(IEvaluationRepository evaluations, IStudentRepository students, IRubricatorRepository criteria, IEvaluationFactory fact)
    {
        _evaluations_repo = evaluations;
        _students_repo = students;
        _rubricator_repo = criteria;
        _factory = fact;
    }

    public int Import(string path)
    {
        IEvaluationImporter? importer = _factory.Create(path);

        if (importer == null)
            return 0;

        List<EvaluationImportData>? data = importer.Import(path);

        if (data == null)
            return 0;

        int added = 0;

        foreach (EvaluationImportData item in data)
        {
            // проверка существования студента и при необходимости создание
            Student? student = _students_repo.GetById(item.StudentId);

            if (student == null)
            {
                student = Student.Create(item.StudentId, item.StudentName);

                if (student == null || !_students_repo.Add(student))
                    continue;
            }

            // проверка существование критерия
            Criterion? criterion = _rubricator_repo.GetById(item.CriterionId);

            if (criterion == null)
                continue;

            // создание оценки
            Evaluation? evaluation = Evaluation.Create(
                item.StudentId,
                item.CriterionId,
                item.Score,
                criterion.MaxScore,
                item.Date,
                item.Comment
            );

            if (evaluation == null)
                continue;

            if (_evaluations_repo.Add(evaluation))
                ++added;
        }

        return added;
    }
}