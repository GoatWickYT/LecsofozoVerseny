namespace Solution.Core.Models;

public class JudgeModel
{
    public uint Id { get; set; }
    public string PublicId { get; set; }
    public ValidatableObject<string> Name { get; set; }
    public ValidatableObject<string> Email { get; set; }
    public string? ImageId { get; set; }
    public string? WebContentLink { get; set; }
    public ValidatableObject<string> PhoneNumber { get; set; }

    public JudgeModel()
    {
        Name = new ValidatableObject<string>();
        Email = new ValidatableObject<string>();
        PhoneNumber = new ValidatableObject<string>();

        AddValidators();
    }

    public JudgeModel(JudgeEntity entity)
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.Name.Value = entity.Name;
        this.Email.Value = entity.Email;
        this.ImageId = entity.ImageId;
        this.WebContentLink = entity.WebContentLink;
        this.PhoneNumber.Value = entity.PhoneNumber;
    }

    public JudgeEntity ToEntity()
    {
        return new JudgeEntity
        {
            PublicId = this.PublicId,
            Name = this.Name.Value,
            Email = this.Email.Value,
            ImageId = this.ImageId,
            WebContentLink = this.WebContentLink,
            PhoneNumber = this.PhoneNumber.Value
        };
    }

    public void ToEntity(JudgeEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.Name = this.Name.Value;
        entity.Email = this.Email.Value;
        entity.ImageId = this.ImageId;
        entity.WebContentLink = this.WebContentLink;
        entity.PhoneNumber = this.PhoneNumber.Value;
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name is required." });
        this.Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Email is required." });
        this.PhoneNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Phone number is required." });
    }
}
