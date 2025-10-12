using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ITaxAgencyRepository
{
    Task<TaxAgency> Add(TaxAgency taxAgency);
    void Delete(int id);
    Task<TaxAgency> Get(int id);
    Task<List<TaxAgency>> GetAll(bool includeInactive);
    Task<TaxAgency> Update(TaxAgency taxAgency);
}

public class TaxAgencyRepository : ITaxAgencyRepository
{
    private readonly ITaxAgencyUnitOfWork _taxAgencyUnitOfWork;

    public TaxAgencyRepository(ITaxAgencyUnitOfWork taxAgencyUnitOfWork)
    {
        _taxAgencyUnitOfWork = taxAgencyUnitOfWork;
    }

    public async Task<TaxAgency> Add(TaxAgency taxAgency)
    {
        return await _taxAgencyUnitOfWork.Add(taxAgency);
    }

    public void Delete(int id)
    {
        _taxAgencyUnitOfWork.Delete(id);
    }

    public async Task<TaxAgency> Get(int id)
    {
        return await _taxAgencyUnitOfWork.GetTaxAgency(id);
    }

    public async Task<List<TaxAgency>> GetAll(bool includeInactive)
    {
        return await _taxAgencyUnitOfWork.GetTaxAgencies(x => includeInactive || !x.Inactive);
    }

    public async Task<TaxAgency> Update(TaxAgency taxAgency)
    {
        return await _taxAgencyUnitOfWork.Update(taxAgency);
    }
}