namespace LastMile.TMS.Application.Common.Interfaces;

/// <summary>
/// Seeder that runs on application startup to provision default data.
/// </summary>
public interface IDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
