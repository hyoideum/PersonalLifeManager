namespace PersonalLifeManager.Exceptions;

public class DuplicateEntryException : AppException
{
    public DuplicateEntryException()
        : base("Entry already exists for this habit and date.")
    {
    }
}