using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ITaxGroupRepository
{
    Task<TaxGroup> Add(TaxGroup taxGroup);
    void Delete(int id);
    Task<TaxGroup> Get(int id);
    Task<List<TaxGroup>> GetAll(bool includeInactive);
    Task<TaxGroup> Update(TaxGroup taxGroup);
}

public class TaxGroupRepository : ITaxGroupRepository
{
    private readonly ITaxGroupUnitOfWork _taxGroupUnitOfWork;
    private readonly ITaxItemUnitOfWork _taxItemUnitOfWork;

    public TaxGroupRepository(ITaxGroupUnitOfWork taxGroupUnitOfWork, ITaxItemUnitOfWork taxItemUnitOfWork)
    {
        _taxGroupUnitOfWork = taxGroupUnitOfWork;
        _taxItemUnitOfWork = taxItemUnitOfWork;
    }

    public async Task<TaxGroup> Add(TaxGroup taxGroup)
    {
        return await _taxGroupUnitOfWork.Add(taxGroup);
    }

    public async Task<TaxGroup> Get(int id)
    {
        var x = await _taxGroupUnitOfWork.GetTaxGroups(x => x.Id == id, null, x => x.Include(t => t.TaxItems));
        return x.FirstOrDefault();
    }

    public async Task<List<TaxGroup>> GetAll(bool includeInactive)
    {
        return await _taxGroupUnitOfWork.GetTaxGroups(x => includeInactive || !x.Inactive);
    }

    public async Task<TaxGroup> Update(TaxGroup taxGroup)
    {
        var current = await Get(taxGroup.Id);
        current.Name = taxGroup.Name;
        current.Description = taxGroup.Description;
        current.QuickBookId = taxGroup.QuickBookId;
        current.DateQuickBookModified = taxGroup.DateQuickBookModified;
        current.Inactive = taxGroup.Inactive;
        // Added
        foreach (var taxGroupTaxItem in taxGroup.TaxItems)
        {
            var exists = current.TaxItems.Any(i => i.Id == taxGroupTaxItem.Id);
            if (!exists)
            {
                // Get one from the context to keep EF happy
                var taxItem = await _taxItemUnitOfWork.GetTaxItem(taxGroupTaxItem.Id);
                current.TaxItems.Add(taxItem);
            }
        }

        // Removed
        var toRemove = new List<TaxItem>();
        foreach (var taxGroupTaxItem in current.TaxItems)
        {
            var exists = taxGroup.TaxItems.Any(i => i.Id == taxGroupTaxItem.Id);
            if (!exists) toRemove.Add(taxGroupTaxItem);
        }

        foreach (var taxItem in toRemove) current.TaxItems.Remove(taxItem);
        return await _taxGroupUnitOfWork.Update(current);
    }

    public void Delete(int id)
    {
        _taxGroupUnitOfWork.Delete(id);
    }
}