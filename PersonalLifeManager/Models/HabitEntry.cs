using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalLifeManager.Models;

public class HabitEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int HabitId { get; set; }
    
    public Habit Habit { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public bool IsDeleted { get; set; }
}