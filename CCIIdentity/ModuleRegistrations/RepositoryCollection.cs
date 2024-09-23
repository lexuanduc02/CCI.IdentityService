using CCI.Domain.EF;
using CCI.Repository;
using CCI.Repository.Contractors;
using Microsoft.EntityFrameworkCore;

namespace CCIIdentity;

public static class RepositoryCollection
{
    public static IServiceCollection AddRepositoryCollection(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DataContext>(
                    option => option.UseNpgsql(connectionString));

        return services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUserRepository, UserRepository>()
        ;
    }

}
