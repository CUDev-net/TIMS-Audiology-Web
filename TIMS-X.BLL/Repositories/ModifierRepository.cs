using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IModifierRepository
{
    Task<Modifier> Add(Modifier modifier);
    void Delete(int id);
    Task<Modifier> Get(int id);
    Task<List<Modifier>> GetAll(bool includeInactive);
    Task<Modifier> Update(Modifier modifier);
}

public class ModifierRepository : IModifierRepository
{
    private readonly IModifierUnitOfWork _modifierUnitOfWork;

    public ModifierRepository(IModifierUnitOfWork modifierUnitOfWork)
    {
        _modifierUnitOfWork = modifierUnitOfWork;
    }

    public async Task<Modifier> Add(Modifier modifier)
    {
        return await _modifierUnitOfWork.Add(modifier);
    }

    public void Delete(int id)
    {
        _modifierUnitOfWork.Delete(id);
    }

    public async Task<Modifier> Get(int id)
    {
        return await _modifierUnitOfWork.GetModifier(id);
    }

    public async Task<List<Modifier>> GetAll(bool includeInactive)
    {
        return await _modifierUnitOfWork.GetModifiers(x => includeInactive || !x.Inactive);
    }

    public async Task<Modifier> Update(Modifier modifier)
    {
        return await _modifierUnitOfWork.Update(modifier);
    }
}