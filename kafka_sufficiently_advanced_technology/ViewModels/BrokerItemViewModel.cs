using CommunityToolkit.Mvvm.ComponentModel;
using kafka_sufficiently_advanced_technology.Models;

namespace kafka_sufficiently_advanced_technology.ViewModels;

public partial class BrokerItemViewModel : ObservableObject
{
    public BrokerConfig Config { get; }

    public string Name => Config.Name;
    public string BootstrapServers => Config.BootstrapServers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExpandIcon))]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isLoadingTopics;

    public string ExpandIcon => IsExpanded ? "▼" : "▶";

    public BrokerItemViewModel(BrokerConfig config)
    {
        Config = config;
    }
}
