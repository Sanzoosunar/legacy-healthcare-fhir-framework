using LegacyHealthcareFHIR.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyHealthcareFHIR.Infrastructure;

public class ServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IFileContentReader GetFileContentReader(string extension)
    {
        return _serviceProvider
           .GetServices<IFileContentReader>()
           .SingleOrDefault(x =>
               x.Extension.ToUpperInvariant() == extension.ToUpperInvariant())
           ?? throw new NotSupportedException(
               $"No reader registered for extension: {extension}");
    }
}