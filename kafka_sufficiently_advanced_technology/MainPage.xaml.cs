using kafka_sufficiently_advanced_technology.ViewModels;

namespace kafka_sufficiently_advanced_technology;

public partial class MainPage : ContentPage
{
    private MainViewModel ViewModel => (MainViewModel)BindingContext;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // ── RadioButton handlers ──────────────────────────────────────────────

    private void OnJsonModeChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) ViewModel.SetJsonMode();
    }

    private void OnClassModeChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) ViewModel.SetClassMode();
    }

    // ── Add broker ────────────────────────────────────────────────────────

    private async void OnAddBrokerClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync(
            "Add Broker",
            "Broker name:",
            placeholder: "e.g. Local Dev",
            maxLength: 60);

        if (string.IsNullOrWhiteSpace(name)) return;

        var servers = await DisplayPromptAsync(
            "Add Broker",
            "Bootstrap servers:",
            placeholder: "e.g. localhost:9092",
            initialValue: "localhost:9092",
            maxLength: 200);

        if (string.IsNullOrWhiteSpace(servers)) return;

        ViewModel.AddBroker(name.Trim(), servers.Trim());
    }
}
