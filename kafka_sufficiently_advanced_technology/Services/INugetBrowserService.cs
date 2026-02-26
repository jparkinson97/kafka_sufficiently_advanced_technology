using kafka_sufficiently_advanced_technology.Models;

namespace kafka_sufficiently_advanced_technology.Services;

public interface INugetBrowserService
{
    Task<IEnumerable<NugetPackage>> GetAllPackagesAsync();
    Task<IEnumerable<string>> GetVersionsAsync(string packageId);
    Task<IEnumerable<NugetClass>> GetClassesAsync(string packageId, string version);
}
