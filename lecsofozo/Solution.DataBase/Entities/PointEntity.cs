namespace Solution.Database.Entities;

[Table("Point")]
public class PointEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public uint Value { get; set; }

    [ForeignKey("Team")]
    public uint TeamId { get; set; }
    public virtual TeamEntity Team { get; set; }

    [ForeignKey("Race")]
    public uint RaceId { get; set; }
    public RaceEntity Race { get; set; }
}
