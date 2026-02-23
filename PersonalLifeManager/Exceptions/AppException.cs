namespace PersonalLifeManager.Exceptions;

public class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}