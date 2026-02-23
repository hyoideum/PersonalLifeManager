using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalLifeManager.Models;

public class HabitEntry : ISoftDelete
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int HabitId { get; set; }
    
    public Habit Habit { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public string UserId { get; set; } = null!;
    
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
}