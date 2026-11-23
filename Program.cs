using Microsoft.Extensions.DependencyInjection;

using POS.domain.interfaces;
using POS.domain.services;

using POS.application.interfaces;
using POS.application.usecases;

using POS.infrastructure.repositories;
using POS.infrastructure.Import;
using POS.infrastructure.Export;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();

        // репозитории
        services.AddSingleton<IStudentRepository, StudentRepository>();
        services.AddSingleton<IRubricatorRepository, RubricatorRepository>();
        services.AddSingleton<IEvaluationRepository, EvaluationRepository>();

        // domain services
        services.AddTransient<ResultEvalCalcService>(_ => new ResultEvalCalcService(1));

        // import / export
        services.AddTransient<IEvaluationImporter, CsvEvaluationImporter>();
        services.AddTransient<IEvaluationImporter, JsonEvaluationImporter>();
        services.AddTransient<IEvaluationImporterFactory, EvaluationImporterFactory>();

        services.AddTransient<IEvaluationExporter, CsvResultsExporter>();

        // use cases: добавление
        services.AddTransient<AddStudentCase>();
        services.AddTransient<AddCriterionCase>();
        services.AddTransient<AddEvaluationCase>();

        // use cases: изменение
        services.AddTransient<ChangeStudentCase>();
        services.AddTransient<ChangeCriterionCase>();
        services.AddTransient<ChangeEvaluationCase>();

        // use cases: удаление
        services.AddTransient<DeleteStudentCase>();
        services.AddTransient<DeleteCriterionCase>();
        services.AddTransient<DeleteEvaluationCase>();

        // use cases: получение
        services.AddTransient<GetStudentsCase>();
        services.AddTransient<GetCriteriaCase>();
        services.AddTransient<GetEvaluationsCase>();

        // use cases: расчёт, импорт, экспорт
        services.AddTransient<CalculateResultScore>();
        services.AddTransient<ImportEvaluationsCase>();
        services.AddTransient<ExportResultsCase>();

        // console
        services.AddSingleton<TextReader>(Console.In);
        services.AddSingleton<TextWriter>(Console.Out);
        services.AddTransient<ConsoleMenu>();

        using ServiceProvider provider = services.BuildServiceProvider();

        ConsoleMenu menu = provider.GetRequiredService<ConsoleMenu>();
        menu.Run();
    }
}