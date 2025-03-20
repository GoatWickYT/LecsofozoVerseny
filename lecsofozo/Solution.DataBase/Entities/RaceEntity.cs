namespace Solution.Database.Entities;

[Table("Race")]
public class RaceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public string PublicId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [ForeignKey("Location")]
    public uint LocationId { get; set; }

    public virtual ICollection<TeamEntity> Teams { get; set; }

    public virtual ICollection<PointEntity> Points { get; set; }

    public virtual ICollection<JudgeEntity> Judges { get; set; }
}
