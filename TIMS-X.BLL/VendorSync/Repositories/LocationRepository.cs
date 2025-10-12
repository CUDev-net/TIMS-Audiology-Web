using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.VendorSync.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetLocations();
    }

    public class LocationRepository : ILocationRepository
    {
        private readonly IPracticeUnitOfWork _practiceUnitOfWork;
        private readonly ISiteUnitOfWork _siteUnitOfWork;

        public LocationRepository(IPracticeUnitOfWork practiceUnitOfWork, ISiteUnitOfWork siteUnitOfWork)
        {
            _practiceUnitOfWork = practiceUnitOfWork;
            _siteUnitOfWork = siteUnitOfWork;
        }

        public async Task<List<Location>> GetLocations()
        {
            var locations = new List<Location>();
            var practice = await _practiceUnitOfWork.GetPracticeSummary();
            var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
            var sites = await _siteUnitOfWork.GetSiteSummaries();

            foreach (var site in sites)
                locations.Add(new Location
                {
                    PracticeId = practiceId,
                    Id = site.Id,
                    Name = site.Name,
                    Address1 = site.Address1,
                    Address2 = site.Address2,
                    City = site.City,
                    State = site.State,
                    ZipCode = site.Zip,
                    Phone = site.Phone,
                    Fax = site.FaxNumber,
                    Active = site.Inactive ? "False" : "True",
                    MonStart = site.MonStart?.ToLongTimeString(),
                    MonEnd = site.MonEnd?.ToLongTimeString(),
                    TuesStart = site.TuesStart?.ToLongTimeString(),
                    TuesEnd = site.TuesEnd?.ToLongTimeString(),
                    WedStart = site.WedStart?.ToLongTimeString(),
                    WedEnd = site.WedEnd?.ToLongTimeString(),
                    ThurStart = site.ThurStart?.ToLongTimeString(),
                    ThurEnd = site.ThurEnd?.ToLongTimeString(),
                    FriStart = site.FriStart?.ToLongTimeString(),
                    FriEnd = site.FriEnd?.ToLongTimeString(),
                    SatStart = site.SatStart?.ToLongTimeString(),
                    SatEnd = site.SatEnd?.ToLongTimeString(),
                    SunStart = site.SunStart?.ToLongTimeString(),
                    SunEnd = site.SunEnd?.ToLongTimeString(),
                });
            return locations;
        }
    }
}