using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafka_sufficiently_advanced_technology.Models;
using kafka_sufficiently_advanced_technology.Services;

namespace kafka_sufficiently_advanced_technology.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IKafkaService _kafkaService;
    private readonly INugetBrowserService _nugetService;

    [ObservableProperty]
    private ObservableCollection<object> _browserItems = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTopicSelected))]
    [NotifyCanExecuteChangedFor(nameof(ViewMessageCommand))]
    private TopicItemViewModel? _selectedTopic;

    public bool IsTopicSelected => SelectedTopic is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsClassMode))]
    private bool _useJsonMode = true;

    public bool IsClassMode => !UseJsonMode;

    [ObservableProperty]
    private ObservableCollection<NugetPackage> _nugetPackages = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPackageSelected))]
    private NugetPackage? _selectedPackage;

    public bool IsPackageSelected => SelectedPackage is not null;

    [ObservableProperty]
    private ObservableCollection<string> _nugetVersions = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVersionSelected))]
    private string? _selectedVersion;

    public bool IsVersionSelected => SelectedVersion is not null;

    [ObservableProperty]
    private ObservableCollection<NugetClass> _nugetClasses = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsClassSelected))]
    private NugetClass? _selectedClass;

    public bool IsClassSelected => SelectedClass is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPartitionOffset))]
    private bool _takeFirst = true;

    public bool ShowPartitionOffset => !TakeFirst;

    [ObservableProperty]
    private string _partition = string.Empty;

    [ObservableProperty]
    private string _offset = string.Empty;

    [ObservableProperty]
    private string _partitionRangeHint = string.Empty;

    [ObservableProperty]
    private string _offsetRangeHint = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessageOutput))]
    private string _messageOutput = string.Empty;

    public bool HasMessageOutput => !string.IsNullOrEmpty(MessageOutput);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ViewMessageCommand))]
    private bool _isLoadingMessage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatus))]
    private string _statusMessage = string.Empty;

    public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

    [ObservableProperty]
    private bool _isStatusError;

    private readonly List<BrokerItemViewModel> _allBrokers = [];
    private readonly Dictionary<BrokerItemViewModel, List<TopicItemViewModel>> _brokerTopics = [];

    public MainViewModel(IKafkaService kafkaService, INugetBrowserService nugetService)
    {
        _kafkaService = kafkaService;
        _nugetService = nugetService;
        Title = "Kafka SAT";
        LoadDefaultBrokers();
    }

    private void LoadDefaultBrokers()
    {
        var configs = new BrokerConfig[]
        {
            new() { Name = "Local Dev",  BootstrapServers = "localhost:9092" },
            new() { Name = "Staging",    BootstrapServers = "staging-kafka.internal:9092" },
            new() { Name = "Production", BootstrapServers = "prod-kafka.internal:9092" },
        };

        foreach (var config in configs)
        {
            var vm = new BrokerItemViewModel(config);
            _allBrokers.Add(vm);
            BrowserItems.Add(vm);
        }
    }

    partial void OnSearchTextChanged(string value) => RebuildFlatList();

    private void RebuildFlatList()
    {
        BrowserItems.Clear();
        foreach (var broker in _allBrokers)
        {
            BrowserItems.Add(broker);
            if (!broker.IsExpanded) continue;
            if (!_brokerTopics.TryGetValue(broker, out var topics)) continue;

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? topics
                : topics.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var topic in filtered)
                BrowserItems.Add(topic);
        }
    }

    [RelayCommand]
    private async Task ToggleBroker(BrokerItemViewModel broker)
    {
        if (broker.IsExpanded)
        {
            broker.IsExpanded = false;
            RebuildFlatList();
            return;
        }

        broker.IsExpanded = true;
        broker.IsLoadingTopics = true;
        try
        {
            if (!_brokerTopics.ContainsKey(broker))
            {
                var names = await _kafkaService.GetTopicsAsync(broker.BootstrapServers);
                _brokerTopics[broker] = names
                    .Select(n => new TopicItemViewModel(n, broker))
                    .ToList();
            }
            RebuildFlatList();
        }
        catch (Exception ex)
        {
            broker.IsExpanded = false;
            ShowStatus($"Could not load topics: {ex.Message}", isError: true);
        }
        finally
        {
            broker.IsLoadingTopics = false;
        }
    }

    [RelayCommand]
    private async Task SelectTopic(TopicItemViewModel topic)
    {
        if (SelectedTopic is not null)
            SelectedTopic.IsSelected = false;

        SelectedTopic = topic;
        topic.IsSelected = true;

        // Reset right-panel state
        MessageOutput = string.Empty;
        StatusMessage = string.Empty;
        Partition = string.Empty;
        Offset = string.Empty;
        PartitionRangeHint = string.Empty;
        OffsetRangeHint = string.Empty;
        NugetPackages.Clear();
        NugetVersions.Clear();
        NugetClasses.Clear();
        SelectedPackage = null;
        SelectedVersion = null;
        SelectedClass = null;

        if (IsClassMode)
            await LoadAllPackagesAsync();

        if (!TakeFirst)
            await LoadTopicRangesAsync();
    }

    partial void OnTakeFirstChanged(bool value)
    {
        if (!value && SelectedTopic is not null)
            _ = LoadTopicRangesAsync();
    }

    private async Task LoadTopicRangesAsync()
    {
        if (SelectedTopic is null) return;
        try
        {
            var (minP, maxP, minO, maxO) = await _kafkaService.GetTopicRangesAsync(
                SelectedTopic.ParentBroker.BootstrapServers,
                SelectedTopic.Name);

            PartitionRangeHint = $"(0 – {maxP})";
            OffsetRangeHint    = $"(0 – {maxO:N0})";
        }
        catch (Exception ex)
        {
            ShowStatus($"Could not load ranges: {ex.Message}", isError: true);
        }
    }

    partial void OnUseJsonModeChanged(bool value)
    {
        // Switched to class mode and we haven't loaded packages yet
        if (!value && SelectedTopic is not null && NugetPackages.Count == 0)
            _ = LoadAllPackagesAsync();
    }

    private async Task LoadAllPackagesAsync()
    {
        IsBusy = true;
        NugetPackages.Clear();
        NugetVersions.Clear();
        NugetClasses.Clear();
        SelectedPackage = null;
        SelectedVersion = null;
        SelectedClass = null;

        try
        {
            var packages = await _nugetService.GetAllPackagesAsync();
            foreach (var p in packages)
                NugetPackages.Add(p);
        }
        catch (Exception ex)
        {
            ShowStatus($"Failed to load packages: {ex.Message}", isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedPackageChanged(NugetPackage? value)
    {
        NugetVersions.Clear();
        NugetClasses.Clear();
        SelectedVersion = null;
        SelectedClass = null;

        if (value is not null)
            _ = LoadVersionsAsync(value.Id);
    }

    private async Task LoadVersionsAsync(string packageId)
    {
        IsBusy = true;
        try
        {
            var versions = await _nugetService.GetVersionsAsync(packageId);
            foreach (var v in versions)
                NugetVersions.Add(v);
        }
        catch (Exception ex)
        {
            ShowStatus($"Failed to load versions: {ex.Message}", isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedVersionChanged(string? value)
    {
        NugetClasses.Clear();
        SelectedClass = null;

        if (value is not null && SelectedPackage is not null)
            _ = LoadClassesAsync(SelectedPackage.Id, value);
    }

    private async Task LoadClassesAsync(string packageId, string version)
    {
        IsBusy = true;
        try
        {
            var classes = await _nugetService.GetClassesAsync(packageId, version);
            foreach (var c in classes)
                NugetClasses.Add(c);
        }
        catch (Exception ex)
        {
            ShowStatus($"Failed to load classes: {ex.Message}", isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void SetJsonMode()  { UseJsonMode = true;  }
    public void SetClassMode() { UseJsonMode = false; }

    [RelayCommand(CanExecute = nameof(CanViewMessage))]
    private async Task ViewMessage()
    {
        if (SelectedTopic is null) return;

        int?  partitionValue = null;
        long? offsetValue    = null;

        if (!TakeFirst)
        {
            if (!string.IsNullOrWhiteSpace(Partition) && int.TryParse(Partition, out var p))
                partitionValue = p;
            if (!string.IsNullOrWhiteSpace(Offset) && long.TryParse(Offset, out var o))
                offsetValue = o;
        }

        IsLoadingMessage = true;
        MessageOutput = string.Empty;
        ShowStatus("Fetching message…", isError: false);

        try
        {
            string result;
            if (UseJsonMode)
            {
                result = await _kafkaService.ConsumeRawMessageAsync(
                    SelectedTopic.ParentBroker.BootstrapServers,
                    SelectedTopic.Name,
                    TakeFirst,
                    partitionValue,
                    offsetValue);
            }
            else
            {
                if (SelectedClass is null)
                {
                    ShowStatus("Please select a class for deserialization.", isError: true);
                    return;
                }
                result = await _kafkaService.ConsumeAndDeserializeAsync(
                    SelectedTopic.ParentBroker.BootstrapServers,
                    SelectedTopic.Name,
                    SelectedClass.AssemblyName,
                    SelectedClass.FullName,
                    TakeFirst,
                    partitionValue,
                    offsetValue);
            }

            MessageOutput = result;
            ShowStatus("Message fetched successfully.", isError: false);
        }
        catch (Exception ex)
        {
            ShowStatus($"Error: {ex.Message}", isError: true);
        }
        finally
        {
            IsLoadingMessage = false;
        }
    }

    private bool CanViewMessage() => SelectedTopic is not null && !IsLoadingMessage;

    public void AddBroker(string name, string bootstrapServers)
    {
        var vm = new BrokerItemViewModel(new BrokerConfig
        {
            Name = name,
            BootstrapServers = bootstrapServers,
        });
        _allBrokers.Add(vm);
        RebuildFlatList();
    }

    private void ShowStatus(string message, bool isError)
    {
        StatusMessage = message;
        IsStatusError = isError;
    }
}
