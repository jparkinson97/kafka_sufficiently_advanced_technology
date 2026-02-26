using CommunityToolkit.Mvvm.ComponentModel;

namespace kafka_sufficiently_advanced_technology.ViewModels;

public partial class TopicItemViewModel : ObservableObject
{
    public string Name { get; }
    public BrokerItemViewModel ParentBroker { get; }

    [ObservableProperty]
    private bool _isSelected;

    public TopicItemViewModel(string name, BrokerItemViewModel parentBroker)
    {
        Name = name;
        ParentBroker = parentBroker;
    }
}
