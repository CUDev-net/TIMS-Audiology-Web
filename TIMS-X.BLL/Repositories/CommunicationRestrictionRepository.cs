using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ICommunicationRestrictionRepository
{
    Task<CommunicationRestriction> Add(CommunicationRestriction communicationRestriction);
    void Delete(int id);
    Task<CommunicationRestriction> Get(int id);
    Task<List<CommunicationRestriction>> GetAll(bool includeInactive);
    Task<CommunicationRestriction> Update(CommunicationRestriction communicationRestriction);
}

public class CommunicationRestrictionRepository : ICommunicationRestrictionRepository
{
    private readonly ICommunicationRestrictionUnitOfWork _communicationRestrictionUnitOfWork;

    public CommunicationRestrictionRepository(ICommunicationRestrictionUnitOfWork communicationRestrictionUnitOfWork)
    {
        _communicationRestrictionUnitOfWork = communicationRestrictionUnitOfWork;
    }

    public async Task<CommunicationRestriction> Add(CommunicationRestriction communicationRestriction)
    {
        return await _communicationRestrictionUnitOfWork.Add(communicationRestriction);
    }

    public async Task<CommunicationRestriction> Get(int id)
    {
        return await _communicationRestrictionUnitOfWork.GetCommunicationRestriction(id);
    }

    public async Task<List<CommunicationRestriction>> GetAll(bool includeInactive)
    {
        return await _communicationRestrictionUnitOfWork
            .GetCommunicationRestrictions(x => includeInactive || !x.Inactive);
    }

    public async Task<CommunicationRestriction> Update(CommunicationRestriction communicationRestriction)
    {
        return await _communicationRestrictionUnitOfWork.Update(communicationRestriction);
    }

    public void Delete(int id)
    {
        _communicationRestrictionUnitOfWork.Delete(id);
    }
}