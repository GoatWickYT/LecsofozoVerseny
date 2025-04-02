
using Solution.Services.Services;

namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class CreateOrEditJudgeViewModel(AppDbContext appDbContext,
                                               IGoogleDriveService googleDriveService,
                                               IJudgeService judgeService) : JudgeModel(), IQueryAttributable
{

    #region life cycle commands
    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);
    #endregion

    #region validation commands
    public IRelayCommand NameValidationCommand => new RelayCommand(() => this.Name.Validate());
    public IRelayCommand EmailValidationCommand => new RelayCommand(() => this.Email.Validate());
    public IRelayCommand PhoneNumberValidationCommand => new RelayCommand(() => this.PhoneNumber.Validate());
    #endregion

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IAsyncRelayCommand ImageSelectCommand => new AsyncRelayCommand(OnImageSelectAsync);
    #endregion

    private delegate Task ButtonActionDelegate();
    private ButtonActionDelegate asyncButtonAction;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private ImageSource image;

    private FileResult selectedFile = null;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        bool hasValue = query.TryGetValue("Judge", out object result);

        if (!hasValue)
        {
            asyncButtonAction = OnSaveAsync;
            Title = "Create Judge";
            return;
        }
        
    }

    private async Task OnAppearingAsync() { }
    private async Task OnDisappearingAsync() { }

    private async Task OnSubmitAsync() => await asyncButtonAction();

    private async Task OnSaveAsync()
    {
        if (!IsFormValid())
        {
            return;
        }
        await UploadImageAsync();

        var result = await judgeService.CreateAsync(this);
        var message = result.IsError ? result.FirstError.Description : "Judge saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            ClearForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task OnUpdateAsync()
    {
        if (!IsFormValid())
        {
            return;
        }
        await UploadImageAsync();

        var result = await judgeService.UpdateAsync(this);

        var message = result.IsError ? result.FirstError.Description : "Judge updated.";
        var title = result.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task OnImageSelectAsync()
    {
        selectedFile = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Please select the judge image"
        });

        if (selectedFile is null)
        {
            return;
        }

        var stream = await selectedFile.OpenReadAsync();
        Image = ImageSource.FromStream(() => stream);
    }

    private async Task UploadImageAsync()
    {
        if (selectedFile is null)
        {
            return;
        }

        var imageUploadResult = await googleDriveService.UploadFileAsync(selectedFile);

        var message = imageUploadResult.IsError ? imageUploadResult.FirstError.Description : "Judge image uploaded.";
        var title = imageUploadResult.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");

        this.ImageId = imageUploadResult.IsError ? null : imageUploadResult.Value.Id;
        this.WebContentLink = imageUploadResult.IsError ? null : imageUploadResult.Value.WebContentLink;
    }

    private bool IsFormValid()
    {
        this.Name.Validate();
        this.Email.Validate();
        this.PhoneNumber.Validate();

        return this.Name.IsValid && this.Email.IsValid && this.PhoneNumber.IsValid;
    }

    private void ClearForm()
    {
        this.Name.Value = null;
        this.Email.Value = null;
        this.PhoneNumber.Value = null;
        this.ImageId = null;
        this.WebContentLink = null;
        this.Image = null;
        this.selectedFile = null;
    }
}
