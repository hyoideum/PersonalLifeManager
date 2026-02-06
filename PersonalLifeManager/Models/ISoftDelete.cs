namespace PersonalLifeManager.Models;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}