using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalLifeManager.Models;

public class Habit : ISoftDelete
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;
    
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public string? UserId { get; set; }
    
    public AppUser AppUser { get; set; }
    
    public int CurrentStreak { get; set; }
    public List<HabitEntry> Entries { get; set; } = new();
}