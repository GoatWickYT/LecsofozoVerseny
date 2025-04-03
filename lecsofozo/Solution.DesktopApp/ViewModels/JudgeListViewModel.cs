using System.Collections.ObjectModel;

namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class JudgeListViewModel(IJudgeService judgeService) : JudgeModel(), IQueryAttributable
{
    #region life cycle commands

    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);

    #endregion

    public IAsyncRelayCommand PreviousPageCommand => new AsyncRelayCommand(PreviousPage);

    public IAsyncRelayCommand FirstPageCommand => new AsyncRelayCommand(FirstPage);

    public IAsyncRelayCommand NextPageCommand => new AsyncRelayCommand(NextPage);

    public IAsyncRelayCommand LastPageCommand => new AsyncRelayCommand(LastPage);

    public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<string>((id) => OnDeleteAsync(id));

    [ObservableProperty]
    private ObservableCollection<JudgeModel> judges;

    [ObservableProperty]
    private string pageNumber = "page\n1";

    [ObservableProperty]
    private bool previousButtonEnabled = false;

    [ObservableProperty]
    private bool nextButtonEnabled = false;

    [ObservableProperty]
    private bool lastButtonEnabled = false;

    [ObservableProperty]
    private bool firstButtonEnabled = false;

    private int maxPageNumber;

    private int page = 1;

    private async Task OnAppearingAsync()
    {
        maxPageNumber = await judgeService.GetMaxPageNumberAsync();
        await LoadMotorcyclesAsync();
    }

    private async Task OnDisappearingAsync()
    { }

    #region Page Buttons
    private async Task PreviousPage()
    {
        if (page > 1)
        {
            page--;
            PageNumber = $"page\n{page}";
            await LoadMotorcyclesAsync();
            return;
        }
    }

    private async Task FirstPage()
    {
        page = 1;
        PageNumber = $"page\n{page}";
        await LoadMotorcyclesAsync();
    }

    private async Task NextPage()
    {
        if (maxPageNumber < page + 1)
        {
            return;
        }
        page++;
        PageNumber = $"page\n{page}";

        await LoadMotorcyclesAsync();
    }

    private async Task LastPage()
    {
        LastButtonEnabled = false;
        page = await judgeService.GetMaxPageNumberAsync();
        PageNumber = $"page\n{page}";

        await LoadMotorcyclesAsync();
    }

    #endregion

    private async Task OnDeleteAsync(string? id)
    {
        var result = await judgeService.DeleteAsync(id);

        var message = result.IsError ? result.FirstError.Description : "Judge deleted";
        var title = result.IsError ? "Error" : "Success";

        if (!result.IsError)
        {
            var judge = Judges.SingleOrDefault(x => x.PublicId == id);
            Judges.Remove(judge);

            if (judges.Count == 0)
            {
                await LoadMotorcyclesAsync();
            }
        }
        await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
    }

    private async Task LoadMotorcyclesAsync()
    {


        var result = await judgeService.GetPagedAsync(page);

        if (result.IsError)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Judges not loaded!", "OK");
            return;
        }
        var message = result.IsError ? result.FirstError.Description : "Done";

        await EnableButtonsAsync();

        Judges = new ObservableCollection<JudgeModel>(result.Value);
    }

    private async Task EnableButtonsAsync()
    {
        LastButtonEnabled = page != maxPageNumber;
        FirstButtonEnabled = page != 1;
        NextButtonEnabled = page < await judgeService.GetMaxPageNumberAsync();
        PreviousButtonEnabled = page > 1;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        bool hasValue = query.TryGetValue("Id", out object result);

        if (!hasValue)
        {
            return;
        }
        await judgeService.DeleteAsync(result.ToString());
        await Application.Current.MainPage.DisplayAlert("Success", "Judge deleted!", "Ok");
        await LoadMotorcyclesAsync();
    }
}