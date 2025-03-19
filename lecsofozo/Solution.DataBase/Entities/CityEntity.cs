namespace Solution.Database.Entities;

[Table("City")]
public class CityEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public uint PostalCode { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
}
