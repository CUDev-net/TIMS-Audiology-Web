using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IAudiologistRepository
{
    Task<Audiologist> GetAudiologist(int id);

    Task<List<Audiologist>> GetAudiologists(bool includeInactive);
}

public class AudiologistRepository : IAudiologistRepository
{
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;

    public AudiologistRepository(IProvidersUnitOfWork providersUnitOfWork, IPracticeUnitOfWork practiceUnitOfWork)
    {
        _providersUnitOfWork = providersUnitOfWork;
        _practiceUnitOfWork = practiceUnitOfWork;
    }

    public async Task<Audiologist> GetAudiologist(int id)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
        var provider = await _providersUnitOfWork.GetProviderSummary(id,
            p => p.Include(x => x.User));
        return _MapProviderToAudiologist(provider, practiceId);
    }

    public async Task<List<Audiologist>> GetAudiologists(bool includeInactive)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
        var audiologists = new List<Audiologist>();
        var rawItems = _providersUnitOfWork.GetProviderSummaries(
            u => (includeInactive || !u.Inactive) && !u.Deleted, null, 
            o => o.Include(p => p.User));
        foreach (var provider in await rawItems.ToListAsync())
            audiologists.Add(_MapProviderToAudiologist(provider, practiceId));
        return audiologists;
    }

    private Audiologist _MapProviderToAudiologist(ProviderSummary provider, string practiceId)
    {
        var audiologist = new Audiologist
        {
            PracticeId = practiceId,
            AudiologistId = provider.Id,
            FirstName = provider.FirstName,
            LastName = provider.LastName,
            Status = provider.Inactive ? "Inactive" : "Active",
        };
        return audiologist;
    }
}