using kafka_sufficiently_advanced_technology.Models;

namespace kafka_sufficiently_advanced_technology.Services;

public class MockNugetBrowserService : INugetBrowserService
{
    public async Task<IEnumerable<NugetPackage>> GetAllPackagesAsync()
    {
        await Task.Delay(400);

        return
        [
            new NugetPackage { Id = "MyCompany.Protos",          Version = "1.0.0", Description = "Core company protobuf definitions" },
            new NugetPackage { Id = "MyCompany.Protos.Orders",   Version = "2.1.3", Description = "Order service protobuf definitions" },
            new NugetPackage { Id = "MyCompany.Protos.Payments", Version = "1.5.0", Description = "Payment service protobuf definitions" },
            new NugetPackage { Id = "MyCompany.Protos.Users",    Version = "3.0.0", Description = "User service protobuf definitions" },
        ];
    }

    public async Task<IEnumerable<string>> GetVersionsAsync(string packageId)
    {
        await Task.Delay(200);

        return ["3.2.1", "3.1.0", "3.0.0", "2.5.4", "2.0.0", "1.0.0"];
    }

    public async Task<IEnumerable<NugetClass>> GetClassesAsync(string packageId, string version)
    {
        await Task.Delay(300);

        return
        [
            new NugetClass { FullName = $"{packageId}.OrderCreated",     ShortName = "OrderCreated",     AssemblyName = packageId },
            new NugetClass { FullName = $"{packageId}.OrderUpdated",     ShortName = "OrderUpdated",     AssemblyName = packageId },
            new NugetClass { FullName = $"{packageId}.OrderCancelled",   ShortName = "OrderCancelled",   AssemblyName = packageId },
            new NugetClass { FullName = $"{packageId}.PaymentProcessed", ShortName = "PaymentProcessed", AssemblyName = packageId },
        ];
    }
}
