namespace kafka_sufficiently_advanced_technology.Models;

public class NugetClass
{
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string AssemblyName { get; set; } = string.Empty;

    public override string ToString() => ShortName;
}
