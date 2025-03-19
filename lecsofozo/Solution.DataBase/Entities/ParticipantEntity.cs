namespace Solution.Database.Entities;

[Table("Participant")]
public class ParticipantEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public string Name { get; set; }

    [ForeignKey("Team")]
    public uint TeamId { get; set; }
    public virtual TeamEntity Team { get; set; }
}
