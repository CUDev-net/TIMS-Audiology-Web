using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ITaxItemRepository
{
    Task<TaxItem> Add(TaxItem taxItem);
    void Delete(int id);
    Task<TaxItem> Get(int id);
    Task<List<TaxItem>> GetAll(bool includeInactive);
    Task<TaxItem> Update(TaxItem taxItem);
}

public class TaxItemRepository : ITaxItemRepository
{
    private readonly ITaxItemUnitOfWork _taxItemUnitOfWork;

    public TaxItemRepository(ITaxItemUnitOfWork taxItemUnitOfWork)
    {
        _taxItemUnitOfWork = taxItemUnitOfWork;
    }

    public async Task<TaxItem> Add(TaxItem taxItem)
    {
        return await _taxItemUnitOfWork.Add(taxItem);
    }

    public void Delete(int id)
    {
        _taxItemUnitOfWork.Delete(id);
    }

    public async Task<TaxItem> Get(int id)
    {
        var t = await _taxItemUnitOfWork.GetTaxItems(x => x.Id == id, null, x => x.Include(t => t.TaxGroups));
        return t.FirstOrDefault();
    }

    public async Task<List<TaxItem>> GetAll(bool includeInactive)
    {
        return await _taxItemUnitOfWork.GetTaxItems(x => includeInactive || !x.Inactive);
    }

    public async Task<TaxItem> Update(TaxItem taxItem)
    {
        return await _taxItemUnitOfWork.Update(taxItem);
    }
}