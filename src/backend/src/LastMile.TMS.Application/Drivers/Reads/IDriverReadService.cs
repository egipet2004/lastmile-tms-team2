namespace LastMile.TMS.Application.Drivers.Reads;

public interface IDriverReadService
{
    IQueryable<DriverReadModel> GetDrivers(Guid? depotId = null);
}
