namespace Solution.Database.Entities;

[Table("Judge")]
public class JudgeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public string PublicId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string? ImageId { get; set; }

    public string? WebContentLink { get; set; }

    [Required]
    [MaxLength(14)]
    public string PhoneNumber { get; set; } // 06 70 123 4567

    public virtual ICollection<RaceEntity> Races { get; set; }
}
