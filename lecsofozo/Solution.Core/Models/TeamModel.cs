namespace Solution.Core.Models;

public class TeamModel : IObjectValidator<uint>
{
    public uint Id { get; set; }

    public string PublicId { get; set; }

    public ValidatableObject<string> Name { get; set; }

    public ICollection<ParticipantModel> Participants { get; set; }

    public TeamModel()
    {
        Name = new ValidatableObject<string>();
        Participants = new List<ParticipantModel>();

        AddValidators();
    }

    public TeamModel(TeamEntity entity)
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.Name.Value = entity.Name;
        this.Participants = new List<ParticipantModel>();
    }

    public TeamEntity ToEntity()
    {
        return new TeamEntity
        {
            PublicId = this.PublicId,
            Name = this.Name.Value,
            Participants = this.Participants.Select(x => x.ToEntity()).ToList()
        };
    }

    public void ToEntity(TeamEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.Name = this.Name.Value;
        entity.Participants = this.Participants.Select(x => x.ToEntity()).ToList();
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name is required." });
    }
}
