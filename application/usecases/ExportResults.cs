using POS.application.interfaces;
using POS.application.models;
using POS.domain.entities;
using POS.domain.interfaces;
// using POS.domain.valueobjects;

namespace POS.application.usecases;

public class ExportResultsCase
{
    public enum Result
    {
        Success,
        SomeInvalidData,
        DataBaseError,
        ExportError

    }
    private readonly IStudentRepository _students_repo;
    private readonly CalculateResultScore _calculator;
    private readonly IEvaluationExporter _exporter;

    public ExportResultsCase(
        IStudentRepository students,
        CalculateResultScore calculator,
        IEvaluationExporter exporter)
    {
        _students_repo = students;
        _calculator = calculator;
        _exporter = exporter;
    }

    public Result Export(string path)
    {
        List<Student>? students = _students_repo.GetAll();

        if (students == null)
            return Result.DataBaseError;

        List<ResultExportData> data = [];

        bool error = false;
        foreach (Student student in students)
        {
            float? result = _calculator.Evaluate(student.Id);

            if (result == null)
            {
                error = true;
                continue;
            }

            data.Add(new ResultExportData(
                student.Id,
                student.Name,
                result.Value
            ));
        }
        if (!_exporter.Export(path, data))
            return Result.ExportError;

        if (error)
            return Result.SomeInvalidData;

        return Result.Success;
    }

    public string GetError(Result res)
    {
        return res switch
        {
            Result.Success => "Success",
            Result.DataBaseError => "DataBaseError",
            Result.SomeInvalidData => "SomeInvalidData",
            Result.ExportError => "ExportError",
            _ => "UnknownError",
        };
    }
}