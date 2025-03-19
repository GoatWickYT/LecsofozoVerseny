namespace Solution.Database.Entities;

[Table("Team")]
public class TeamEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<ParticipantEntity> Participants { get; set; }

    public virtual ICollection<RaceEntity> Races { get; set; }

    public virtual ICollection<PointEntity> Points { get; set; }
}
