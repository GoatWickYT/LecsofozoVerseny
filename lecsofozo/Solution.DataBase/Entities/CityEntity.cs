namespace Solution.Database.Entities;

[Table("City")]
public class CityEntity
{
    [Key]
    public uint PostalCode { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
}
