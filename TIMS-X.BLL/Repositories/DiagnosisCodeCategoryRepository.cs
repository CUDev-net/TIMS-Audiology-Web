using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IDiagnosisCodeCategoryRepository
{
    Task<DiagnosisCodeCategory> Add(DiagnosisCodeCategory diagnosisCodeCategory);
    void Delete(int id);
    Task<DiagnosisCodeCategory> Get(int id);
    Task<List<DiagnosisCodeCategory>> GetAll(bool includeInactive);
    Task<DiagnosisCodeCategory> Update(DiagnosisCodeCategory diagnosisCodeCategory);
}

public class DiagnosisCodeCategoryRepository : IDiagnosisCodeCategoryRepository
{
    private readonly IDiagnosisCodeCategoryUnitOfWork _diagnosisCodeCategoryUnitOfWork;

    public DiagnosisCodeCategoryRepository(IDiagnosisCodeCategoryUnitOfWork diagnosisCodeCategoryUnitOfWork)
    {
        _diagnosisCodeCategoryUnitOfWork = diagnosisCodeCategoryUnitOfWork;
    }

    public async Task<DiagnosisCodeCategory> Add(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        return await _diagnosisCodeCategoryUnitOfWork.Add(diagnosisCodeCategory);
    }

    public void Delete(int id)
    {
        _diagnosisCodeCategoryUnitOfWork.Delete(id);
    }

    public async Task<DiagnosisCodeCategory> Get(int id)
    {
        return await _diagnosisCodeCategoryUnitOfWork.GetDiagnosisCodeCategory(id);
    }

    public async Task<List<DiagnosisCodeCategory>> GetAll(bool includeInactive)
    {
        return await _diagnosisCodeCategoryUnitOfWork.GetDiagnosisCodeCategories(x => includeInactive || !x.Inactive);
    }

    public async Task<DiagnosisCodeCategory> Update(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        return await _diagnosisCodeCategoryUnitOfWork.Update(diagnosisCodeCategory);
    }
}