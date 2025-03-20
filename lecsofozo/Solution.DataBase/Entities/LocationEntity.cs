namespace Solution.Database.Entities;

[Table("Location")]
public class LocationEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Street { get; set; }

    [Required]
    [MaxLength(10)]
    public string HouseNumber { get; set; }

    [ForeignKey("City")]
    public uint PostalCode { get; set; }
    public virtual CityEntity City { get; set; }
}
