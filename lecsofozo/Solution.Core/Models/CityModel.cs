namespace Solution.Core.Models;

public class CityModel : IObjectValidator<uint>
{
    public uint Id { get; set; }

    public uint PostalCode { get; set; }

    public string Name { get; set; }

    public CityModel()
    {

    }

    public CityModel(uint id, uint postalCode, string name)
    {
        this.Id = id;
        this.PostalCode = postalCode;
        this.Name = name;
    }

    public CityModel(CityEntity entity)
    {
        if (entity is null)
        {
            return;
        }
        this.Id = entity.Id;
        this.PostalCode = entity.PostalCode;
        this.Name = entity.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is CityModel model &&
               this.Id == model.Id &&
               this.PostalCode == model.PostalCode &&
               this.Name == model.Name;
    }
}
