using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Users.Common;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Users.Reads;

public sealed class UserReadService(IAppDbContext dbContext) : IUserReadService
{
    public IQueryable<UserManagementUserDto> GetUsers(
        string? search = null,
        bool? isActive = null,
        Guid? depotId = null,
        Guid? zoneId = null)
    {
        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = search.Trim().ToUpperInvariant();
            query = query.Where(u =>
                (u.FirstName + " " + u.LastName).ToUpper().Contains(pattern) ||
                u.Email!.ToUpper().Contains(pattern) ||
                (u.PhoneNumber ?? "").ToUpper().Contains(pattern));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        if (depotId.HasValue)
        {
            query = query.Where(u => u.DepotId == depotId.Value);
        }

        if (zoneId.HasValue)
        {
            query = query.Where(u => u.ZoneId == zoneId.Value);
        }

        return query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new UserManagementUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email!,
                Phone = u.PhoneNumber,
                Role = null,
                IsActive = u.IsActive,
                IsProtected = u.IsSystemAdmin,
                DepotId = u.DepotId,
                DepotName = u.Depot != null ? u.Depot.Name : null,
                ZoneId = u.ZoneId,
                ZoneName = u.Zone != null ? u.Zone.Name : null,
                CreatedAt = u.CreatedAt,
                LastModifiedAt = u.LastModifiedAt
            });
    }
}
