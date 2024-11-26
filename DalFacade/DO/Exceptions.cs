namespace DO;
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
public class DalDateFormatWorngException : FormatException //הוספת זריקת שגיאה עבור הזנת תאריך בפורמט שגוי
{
    public DalDateFormatWorngException(string? message) : base(message) { }
}
