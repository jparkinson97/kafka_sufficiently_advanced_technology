namespace kafka_sufficiently_advanced_technology.Models;

public class NugetPackage
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override string ToString() => $"{Id} ({Version})";
}
