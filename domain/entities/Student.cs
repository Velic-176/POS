namespace POS.domain.entities;

public class Student
{
    public readonly Guid Id;
    public string Name { get; private set; }

    private Student(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    static public Student? Create(Guid id, string name)
    {
        if (!IsNameValid(name) || id == Guid.Empty) return null; // чтобы прерывать создание в корне. Заготовка под будущий Result и логи

        return new Student(id, name);
    }

    public bool ChangeName(string new_name)
    {
        if (!IsNameValid(new_name)) return false;

        Name = new_name;
        return true;
    }

    public static bool IsNameValid(string name)
    {
        // проверка на существование
        if (name == null || name == "")
            return false;

        // проверка на корректные символы
        foreach (char c in name)
        {
            if (!char.IsWhiteSpace(c) && !char.IsLetter(c) && c != '-')
                return false;
        }

        // проверка на количество слов (ФИО || ФИ)
        char[] separators = [' '];
        var splited = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = splited.Length;
        if (wordCount < 2 || wordCount > 3)
            return false;


        // проверка на заглавные
        foreach (string word in splited)
        {
            if (!char.IsUpper(word[0]))
                return false;
        }

        return true;
    }
}
