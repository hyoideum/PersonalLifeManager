namespace PersonalLifeManager.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException()
        : base("You do not have access to this resource.")
    {
    }
}