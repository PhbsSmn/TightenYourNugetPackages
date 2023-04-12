using Microsoft.Extensions.DependencyInjection;

namespace DigitalDiary.MemoryRepository;

/// <summary>
/// Extension class for easy configuration
/// </summary>
public static class DigitalDiaryBuilderExtensions
{
    /// <summary>
    /// This configures the Digital Diary out of the box for you.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection ConfigureDigitalDiary(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDigitalDiaryRepository, Repository>();
        return serviceCollection;
    }
}