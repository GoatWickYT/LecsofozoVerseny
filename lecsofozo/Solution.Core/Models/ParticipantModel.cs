using CommunityToolkit.Mvvm.ComponentModel;

namespace Solution.Core.Models;

[ObservableObject]
public class ParticipantModel
{
    public uint Id { get; set; }

    public string PublicId { get; set; }

    public string ImageId { get; set; }

    public string WebContentLink { get; set; }

    public uint TeamId { get; set; }

    [ObservableProperty]
    private ImageSource image;

    public ValidatableObject<string> Name { get; protected set; }

    public ParticipantModel()
    {
        Name = new ValidatableObject<string>();

        AddValidators();
    }

    public ParticipantModel(ParticipantEntity entity)
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.ImageId = entity.ImageId;
        this.WebContentLink = entity.WebContentLink;
        this.Name.Value = entity.Name;
        this.TeamId = entity.TeamId;
    }

    public ParticipantEntity ToEntity()
    {
        return new ParticipantEntity
        {
            PublicId = this.PublicId,
            ImageId = this.ImageId,
            WebContentLink = this.WebContentLink,
            Name = this.Name.Value,
            TeamId = this.TeamId
        };
    }

    public void ToEntity(ParticipantEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.ImageId = this.ImageId;
        entity.WebContentLink = this.WebContentLink;
        entity.Name = this.Name.Value;
        this.TeamId = this.TeamId;
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Member name is required." });
    }
}