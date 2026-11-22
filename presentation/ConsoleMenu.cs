using POS.application.models;
using POS.application.usecases;
using POS.domain.entities;
// using System.Collections;
using System.Globalization;

public class ConsoleMenu
{
    private readonly AddStudentCase _addStudent;
    private readonly AddCriterionCase _addCriterion;
    private readonly AddEvaluationCase _addEvaluation;

    private readonly ChangeStudentCase _changeStudent;
    private readonly ChangeCriterionCase _changeCriterion;
    private readonly ChangeEvaluationCase _changeEvaluation;

    private readonly DeleteStudentCase _deleteStudent;
    private readonly DeleteCriterionCase _deleteCriterion;
    private readonly DeleteEvaluationCase _deleteEvaluation;

    private readonly GetStudentsCase _getStudents;
    private readonly GetCriteriaCase _getCriteria;
    private readonly GetEvaluationsCase _getEvaluations;

    private readonly CalculateResultScore _calculateResult;
    private readonly ImportEvaluationsCase _importEvaluations;
    private readonly ExportResultsCase _exportResults;

    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleMenu(AddStudentCase addStudent,
        AddCriterionCase addCriterion,
        AddEvaluationCase addEvaluation,
        ChangeStudentCase changeStudent,
        ChangeCriterionCase changeCriterion,
        ChangeEvaluationCase changeEvaluation,
        DeleteStudentCase deleteStudent,
        DeleteCriterionCase deleteCriterion,
        DeleteEvaluationCase deleteEvaluation,
        GetStudentsCase getStudents,
        GetCriteriaCase getCriteria,
        GetEvaluationsCase getEvaluations,
        CalculateResultScore calculateResult,
        ImportEvaluationsCase importEvaluations,
        ExportResultsCase exportResults,
        TextReader input, TextWriter output)
    {
        _addStudent = addStudent;
        _addCriterion = addCriterion;
        _addEvaluation = addEvaluation;

        _changeStudent = changeStudent;
        _changeCriterion = changeCriterion;
        _changeEvaluation = changeEvaluation;

        _deleteStudent = deleteStudent;
        _deleteCriterion = deleteCriterion;
        _deleteEvaluation = deleteEvaluation;

        _getStudents = getStudents;
        _getCriteria = getCriteria;
        _getEvaluations = getEvaluations;

        _calculateResult = calculateResult;
        _importEvaluations = importEvaluations;
        _exportResults = exportResults;

        _input = input;
        _output = output;
    }

    public void Run()
    {
        try
        {
            while (true)
            {
                PrintMainMenu();
                int command = ReadInt("Введите номер команды: ", 8);
                _output.Write("\n");

                switch (command)
                {
                    case 0: return;
                    case 1: PrintStudents(); break;
                    case 2: StudentMenu(); break;
                    case 3: PrintCriteria(); break;
                    case 4: CriteriaMenu(); break;
                    case 5: PrintResults(); break;
                    case 6: ImportData(); break;
                    case 7: ExportData(); break;
                    default: break;
                }
            }
        }
        catch (EndOfStreamException)
        {
            _output.WriteLine("\nВвод завершён. Завершение.");
        }
    }

    void PrintMainMenu()
    {
        _output.WriteLine("==========ПОС==========");
        _output.WriteLine("1. Список студентов");
        _output.WriteLine("2. Меню студента");
        _output.WriteLine("3. Список критериев");
        _output.WriteLine("4. Меню критериев");
        _output.WriteLine("5. Вывести результаты");
        _output.WriteLine("----------------");
        _output.WriteLine("6. Импорт данных об оценках");
        _output.WriteLine("7. Экспорт результатов");
        _output.WriteLine("----------------");
        _output.WriteLine("0. Завершить работу");
        _output.Write("\n");
    }

    void ImportData()
    {
        string path = ReadPath("Введите путь до файла (.json/.csv): ");
        _output.Write("\n");

        int res = _importEvaluations.Import(path);
        _output.WriteLine($"Добавленно {res} новых записей.");
    }

    void ExportData()
    {
        string path = ReadPath("Введите путь до/название файла: ");
        _output.Write("\n");

        var res = _exportResults.Export(path);
        if (res != ExportResultsCase.Result.Success)
        {
            _output.WriteLine($"Error: {_exportResults.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно экспортированы.\n");
    }

    void PrintResults()
    {
        var students = _getStudents.GetAll();
        if (students == null)
        {
            _output.WriteLine($"Error: {_getStudents.GetError()}");
            return;
        }

        _output.WriteLine("       Студент       | Итоговый Балл");
        foreach (var st in students)
        {
            var result = _calculateResult.Evaluate(st.Id);
            if (result == null)
            {
                _output.WriteLine($"Error: {_calculateResult.GetError()}");
                continue;
            }

            _output.WriteLine($"{st.Name} | {result} / 10");
        }
        _output.Write("\n");
    }

    void StudentMenu()
    {
        while (true)
        {
            _output.WriteLine("==========Студенты==========");
            _output.WriteLine("1. Список студентов");
            _output.WriteLine("2. Изменить данные студента");
            _output.WriteLine("3. Добавить студента");
            _output.WriteLine("4. Удалить студента");
            _output.WriteLine("5. Список оценок студента");
            _output.WriteLine("0. Назад");
            _output.Write("\n");
            int command = ReadInt("Введите номер команды: ", 6);
            _output.Write("\n");

            switch (command)
            {
                case 0: return;
                case 1: PrintStudents(); break;
                case 2:
                    {
                        var st = FindStudent();
                        if (st != null)
                            ChangeStudent(st);
                        break;
                    }

                case 3:
                    {
                        AddStudent();
                        break;
                    }

                case 4:
                    {
                        var st = FindStudent();
                        _output.Write("\n");
                        if (st != null)
                            DeleteStudent(st);
                        break;
                    }
                case 5:
                    {


                        var st = FindStudent();
                        _output.Write("\n");
                        if (st != null)
                        {
                            PrintEvaluations(st);
                            EvaluationMenu(st);
                        }
                        break;
                    }
                default: break;
            }
        }
    }

    void AddStudent()
    {
        _output.WriteLine("\nВведите данные:");
        string name = ReadString("ФИО: ");
        _output.Write("\n");

        var res = _addStudent.Add(name);
        if (res != AddStudentCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_addStudent.GetError(res)}\n");
            return;
        }
    }

    void DeleteStudent(Student st)
    {
        _output.WriteLine("Вы уверены, что хотите удалить данные?");
        string answer = "";
        while (true)
        {
            answer = ReadString("да/нет: ");
            if (answer == "да" || answer == "нет")
                break;
            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
        _output.Write("\n");

        if (answer == "нет")
        {
            return;
        }

        var res = _deleteStudent.DeleteById(st.Id);
        if (res != DeleteStudentCase.Result.Success)
        {
            _output.WriteLine("Не удалось удалить данные.");
            _output.WriteLine($"Error: {_deleteStudent.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно удалены.\n");
        return;
    }

    void ChangeStudent(Student st)
    {
        _output.WriteLine("\nВведите новые данные:");
        string name = ReadString("ФИО: ");
        _output.Write("\n");

        var res = _changeStudent.Change(st.Id, name);
        if (res != ChangeStudentCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_changeStudent.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно изменены.\n");
        return;
    }

    Student? FindStudent()
    {
        Guid id = ReadGuid("Введите guid студента: ");
        _output.Write("\n");
        var st = _getStudents.GetById(id);
        if (st == null)
        {
            _output.WriteLine($"Error: {_getStudents.GetError()}\n");
            return null;
        }

        PrintStudent(st);
        return st;
    }

    void CriteriaMenu()
    {
        while (true)
        {
            _output.WriteLine("==========Критерии==========");
            _output.WriteLine("1. Список критериев");
            _output.WriteLine("2. Изменить критерий");
            _output.WriteLine("3. Добавить критерий");
            _output.WriteLine("4. Удалить критерий");
            _output.WriteLine("0. Назад");
            int command = ReadInt("Введите номер команды: ", 5);
            _output.Write("\n");

            switch (command)
            {
                case 0: return;
                case 1: PrintCriteria(); break;
                case 2:
                    {
                        var cr = FindCriterion();
                        if (cr != null)
                            ChangeCriterion(cr);
                        break;
                    }

                case 3:
                    {
                        AddCriterion();
                        break;
                    }

                case 4:
                    {
                        var cr = FindCriterion();
                        if (cr != null)
                            DeleteCriterion(cr);
                        break;
                    }
                default: break;
            }
        }
    }

    void AddCriterion()
    {
        _output.WriteLine("\nВведите данные:");
        string name = ReadString("Название: ");
        float score = ReadFloat("Максимальный балл: ");
        float weight = ReadFloat("Вес от общей оценки (от 0 до 1): ");

        var res = _addCriterion.Add(name, score, weight);
        if (res != AddCriterionCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_addCriterion.GetError(res)}\n");
            return;
        }
    }

    void ChangeCriterion(Criterion cr)
    {
        _output.WriteLine("\nВведите новые данные:");
        string name = ReadString("Название: ");
        float score = ReadFloat("Максимальный балл: ");
        float weight = ReadFloat("Вес от общей оценки (от 0 до 1): ");

        var res = _changeCriterion.Change(cr.Id, name, score, weight);
        if (res != ChangeCriterionCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_changeCriterion.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно изменены.\n");
        return;
    }

    void DeleteCriterion(Criterion cr)
    {
        _output.WriteLine("Вы уверены, что хотите удалить данные?");
        string answer = "";
        while (true)
        {
            answer = ReadString("да/нет: ");
            if (answer == "да" || answer == "нет")
                break;
            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
        _output.Write("\n");

        if (answer == "нет")
        {
            return;
        }

        var res = _deleteCriterion.DeleteById(cr.Id);
        if (res != DeleteCriterionCase.Result.Success)
        {
            _output.WriteLine("Не удалось удалить данные.");
            _output.WriteLine($"Error: {_deleteCriterion.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно удалены.\n");
        return;
    }
    void EvaluationMenu(Student st)
    {
        while (true)
        {
            _output.WriteLine("===========Оценки===========");
            _output.WriteLine("1. Вывести оценки студента");
            _output.WriteLine("2. Добавить оценку");
            _output.WriteLine("3. Изменить оценку");
            _output.WriteLine("0. Назад");
            int command = ReadInt("Введите номер команды: ", 4);
            _output.Write("\n");

            switch (command)
            {
                case 0: return;
                case 1:
                    {
                        PrintEvaluations(st);
                        break;
                    }

                case 2:
                    {
                        var cr = FindCriterion();
                        if (cr != null)
                        {
                            AddEvaluation(cr.Id, st.Id);
                        }
                        break;
                    }

                case 3:
                    {
                        var cr = FindCriterion();
                        if (cr != null)
                        {
                            ChangeEvaluation(cr.Id, st.Id);
                        }
                        break;
                    }
                default: break;
            }
        }
    }

    void AddEvaluation(Guid cr, Guid st)
    {
        _output.WriteLine("\nВведите данные:");
        float score = ReadFloat("оценка: ");
        string comment = ReadString("Комментарий: ");

        var res = _addEvaluation.Add(st, cr, score, comment);
        if (res != AddEvaluationCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_addEvaluation.GetError(res)}\n");
            return;
        }

        _output.WriteLine("Данные успешно записаны.\n");
    }
    void ChangeEvaluation(Guid cr, Guid st)
    {
        _output.WriteLine("\nВведите новые данные:");
        float score = ReadFloat("оценка: ");
        string comment = ReadString("Комментарий: ");

        Evaluation.EvaluationId id;
        id.CriterionId = cr;
        id.StudentId = st;

        var res = _changeEvaluation.Change(id, score, comment);
        if (res != ChangeEvaluationCase.Result.Success)
        {
            _output.WriteLine("Не удалось записать данные.");
            _output.WriteLine($"Error: {_changeEvaluation.GetError(res)}\n");
            return;
        }
        _output.WriteLine("Данные успешно изменены.\n");
        return;
    }

    Criterion? FindCriterion()
    {
        string cr_name = ReadString("Введите критерий: ");
        _output.Write("\n");

        var cr = _getCriteria.GetByName(cr_name);
        if (cr == null)
        {
            _output.WriteLine($"Error: {_getCriteria.GetError()}\n");
            return null;
        }

        return cr;
    }

    void PrintEvaluation(EvaluationInfo eval)
    {
        if (eval.Score == null)
            _output.WriteLine($"{eval.CriterionName} : не оценено");
        else
            _output.WriteLine($"{eval.CriterionName} : {eval.Score} / {eval.MaxScore}");

        _output.WriteLine($"\tВес: {eval.Weight}");

        if (eval.Date == null)
            _output.WriteLine($"\tДата: -");
        else
        {
            CultureInfo culture = new CultureInfo("de-DE");
            _output.WriteLine($"\tDate: {eval.Date?.ToString(culture)}");
        }

        _output.WriteLine($"\tКомментарий: {eval.Comment}");
    }

    void PrintEvaluations(Student st)
    {

        var evals = _getEvaluations.Get(st.Id);
        if (evals == null)
        {
            _output.WriteLine($"Error: {_getEvaluations.GetError()}");
            return;
        }

        foreach (var eval in evals)
        {
            PrintEvaluation(eval);
            // _output.Write("\n");
        }
        _output.Write("\n");
    }

    void PrintStudent(Student st)
    {
        _output.WriteLine($"{st.Id} | {st.Name}");
    }

    void PrintStudents()
    {
        var students = _getStudents.GetAll();
        if (students == null)
        {
            _output.WriteLine($"Error: {_getStudents.GetError()}\n");
            return;
        }

        _output.WriteLine("                Guid                 | Имя");
        foreach (var st in students)
        {
            PrintStudent(st);
        }
        _output.Write("\n");
    }

    void PrintCriteria()
    {
        var criteria = _getCriteria.GetAll();
        if (criteria == null)
        {
            _output.WriteLine($"Error: {_getCriteria.GetError()}");
            return;
        }

        _output.WriteLine("                Guid                 |   Имя   | Макс. Балл | Вес");
        foreach (var cr in criteria)
        {
            PrintCriterion(cr);
        }
        _output.Write("\n");
    }

    void PrintCriterion(Criterion cr)
    {
        _output.WriteLine($"{cr.Id} | {cr.Name} |    {cr.MaxScore.Value}    | {cr.Weight.Value}");
    }

    private string ReadString(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? input = _input.ReadLine();
            if (input == null)
                throw new EndOfStreamException();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            _output.WriteLine("Строка не может быть пустой. Попробуйте снова.");
        }
    }

    private string ReadPath(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? input = _input.ReadLine();
            if (input == null)
                throw new EndOfStreamException();

            if (input != null && input.Length > 0 && input.All(c => char.IsLetter(c) || char.IsDigit(c) || "_\\-./:".Contains(c)))
                return input;

            if (input == "0")
                return "";

            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
    }

    private float ReadFloat(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? input = _input.ReadLine();
            if (input == null)
                throw new EndOfStreamException();

            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) &&
                float.IsFinite(value))
                return value;

            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
    }

    private int ReadInt(string prompt, int max)
    {
        if (max == 0)
            max = int.MaxValue;
        while (true)
        {
            _output.Write(prompt);
            string? input = _input.ReadLine();
            if (input == null)
                throw new EndOfStreamException();

            if (int.TryParse(input, out int value) && value >= 0 && value < max)
                return value;

            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
    }

    private Guid ReadGuid(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? input = _input.ReadLine();
            if (input == null)
                throw new EndOfStreamException();

            if (Guid.TryParse(input, out Guid value) && value != Guid.Empty)
                return value;

            if (input == "0")
                return Guid.Empty;

            _output.WriteLine("Некорректный ввод. Попробуйте снова.");
        }
    }
}