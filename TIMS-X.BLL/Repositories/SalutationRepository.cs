using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ISalutationRepository
{
    Task<Salutation> Add(Salutation salutation);
    void Delete(int id);
    Task<Salutation> Get(int id);
    Task<List<Salutation>> GetAll(bool includeInactive);
    Task<Salutation> Update(Salutation salutation);
}

public class SalutationRepository : ISalutationRepository
{
    private readonly ISalutationUnitOfWork _salutationUnitOfWork;

    public SalutationRepository(ISalutationUnitOfWork salutationUnitOfWork)
    {
        _salutationUnitOfWork = salutationUnitOfWork;
    }

    public async Task<Salutation> Add(Salutation salutation)
    {
        return await _salutationUnitOfWork.Add(salutation);
    }

    public async Task<Salutation> Get(int id)
    {
        return await _salutationUnitOfWork.GetSalutation(id);
    }

    public async Task<List<Salutation>> GetAll(bool includeInactive)
    {
        return await _salutationUnitOfWork.GetSalutations(x => includeInactive || !x.Inactive);
    }

    public async Task<Salutation> Update(Salutation salutation)
    {
        return await _salutationUnitOfWork.Update(salutation);
    }

    public void Delete(int id)
    {
        _salutationUnitOfWork.Delete(id);
    }
}