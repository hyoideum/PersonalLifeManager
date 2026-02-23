namespace PersonalLifeManager.Exceptions;

public class FromToDateException : AppException
{
    public FromToDateException() : base("From date must be before To date")
    {
    }
}