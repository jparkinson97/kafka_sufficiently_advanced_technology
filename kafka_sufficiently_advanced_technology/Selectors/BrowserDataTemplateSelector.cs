using kafka_sufficiently_advanced_technology.ViewModels;

namespace kafka_sufficiently_advanced_technology.Selectors;

public class BrowserDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate BrokerTemplate { get; set; } = null!;
    public DataTemplate TopicTemplate  { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        => item is BrokerItemViewModel ? BrokerTemplate : TopicTemplate;
}
