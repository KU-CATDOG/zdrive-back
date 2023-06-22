namespace ZDrive.Utils;

public record Period
{
    public int Year { get; init; }
    public Semester Semester { get; init; }

    public Period(int year, Semester semester)
    {
        Year = year;
        Semester = semester;
    }

    public Period(string str)
    {
        var semester = (int.Parse(str.Substring(str.IndexOf('-') + 1)) - 1);
        if (semester > 2) throw new FormatException();
        Year = int.Parse(str.Substring(0, str.IndexOf('-')));
        Semester = (Semester)semester;
    }

    // IQueryable에는 특정 메소드만 사용 가능해서 이 메소드는 쿼리 추출에 사용 불가능함
    // 개에바네
    public bool IsWithInPeriod(DateTime date)
    {
        if (Semester == Semester.First)
            return new DateTime(Year, 3, 1) < date && new DateTime(Year, 8, 31) > date;
        else
            return new DateTime(Year, 9, 1) < date && new DateTime(Year + 1, 2, DateTime.IsLeapYear(Year) ? 29 : 28) > date;
    }
}

public enum Semester
{
    First,
    Second
}
