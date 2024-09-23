using CCI.Model;
using CCI.Model.Options;

namespace CCIIdentity;

public static class OptionCollection
{
    public static IServiceCollection AddOptionCollection(this IServiceCollection services,
           IConfiguration configuration)
    {
        return services
                .Configure<LogOption>(option => configuration.GetSection(LogOption.Position).Bind(option))
                .Configure<SmtpOption>(option => configuration.GetSection(SmtpOption.Position).Bind(option))
                .Configure<CloudinaryOption>(option => configuration.GetSection(CloudinaryOption.Position).Bind(option))
            ;
    }
}
